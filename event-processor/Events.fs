module Events

open FSharp.Data
open Newtonsoft.Json.Linq
open Newtonsoft.Json
open XPlot.GoogleCharts
open System


type DataInt = {
    value: int
}

type DataStr = {
    value: string
}

type DataDouble = {
    value: double
}

type Fields = {
    aggregatedEventCount : DataInt
    sessionIdentifier : DataStr
    batteryLevel : DataDouble
}

type Created = {
    timestamp : string
    userRecordName : string
    deviceID : string
}

type SamplingData = {
    recordName : string
    recordType : string
    fields : Fields
    created: Created
}

type CloudRecord = {
    records : SamplingData list
}

let getHHMM timestamp =
    let timestampInt64 = (timestamp |> int64)
    let dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestampInt64);
    let dateTime = dateTimeOffset.UtcDateTime;
    dateTime.ToString("HH:mm")

let getSamplingData sessionIdentifier (cloudRecordStr:string) =
    let points =
        (JsonConvert.DeserializeObject<CloudRecord>
            (cloudRecordStr, JsonSerializerSettings(MissingMemberHandling = MissingMemberHandling.Ignore)))
    points.records
    |> List.filter (fun x -> x.fields.sessionIdentifier.value = sessionIdentifier)

let createBatteryChart (samplingData: SamplingData List) =
    samplingData
    |> List.map (fun x -> (getHHMM x.created.timestamp, x.fields.batteryLevel.value))
    |> List.toSeq
    |> Chart.Line 
    |> Chart.WithLabel "Battery Level %"
    |> Chart.WithOptions 
         (Options(title = "Sopor - Sleep Session Battery Life")) 
    |> Chart.WithLegend true
    |> Chart.WithSize (1000, 500)

let createAggregatedEventCount (samplingData: SamplingData List) = 
    samplingData
    |> List.map (fun x -> (getHHMM x.created.timestamp, x.fields.aggregatedEventCount.value))
    |> List.toSeq
    |> Chart.Line 
    |> Chart.WithLabel "Number Of Events"
    |> Chart.WithOptions 
         (Options(title = "Aggregated Events Count"))
    |> Chart.WithLegend true
    |> Chart.WithSize (1000, 500)


