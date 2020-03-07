module Charts
open XPlot.GoogleCharts
open System

let milliOrSeconds (timestamp:string) =
    let timestampInt64 = (timestamp |> int64)
    if timestamp.Length = ("1582991605".Length) then
        DateTimeOffset.FromUnixTimeSeconds(timestampInt64)
    else
        DateTimeOffset.FromUnixTimeMilliseconds(timestampInt64)

let getHHMM timestamp =
    let dateTimeOffset = milliOrSeconds timestamp
    let dateTime = dateTimeOffset.UtcDateTime;
    dateTime.ToString("HH:mm")

let createBatteryChart (samplingData: Data.SamplingData List) =
    let axis = Axis(minValue=0, maxValue=100)
    let options =
        Options
            ( title = "Sopor - Sleep Session Battery Life",
              legend = Legend(position = "bottom"), vAxis=axis )

    samplingData
    |> List.map (fun x -> (getHHMM x.created.timestamp, x.fields.batteryLevel.value))
    |> List.toSeq
    |> Chart.Line 
    |> Chart.WithLabel "Battery Level %"
    |> Chart.WithOptions options
    |> Chart.WithLegend true
    |> Chart.WithSize (1000, 500)

let createAggregatedEventCount (samplingData: Data.SamplingData List) = 
    samplingData
    |> List.map (fun x -> (getHHMM x.created.timestamp, x.fields.aggregatedEventCount.value))
    |> List.toSeq
    |> Chart.Line 
    |> Chart.WithLabel "Number Of Events"
    |> Chart.WithOptions 
         (Options(title = "Aggregated Events Count"))
    |> Chart.WithLegend true
    |> Chart.WithSize (1000, 500)

let createEventChart (events: Sensors.Event List) sensorName = 
    events
    |> List.filter (fun x -> (x.sensorName = sensorName))
    |> List.map (fun x -> (getHHMM x.timestamp, x.event.heartRate))
    |> List.toSeq
    |> Chart.Line 
    |> Chart.WithLabel "Heart Rate"
    |> Chart.WithOptions 
         (Options(title = "Heart Rate"))
    |> Chart.WithLegend true
    |> Chart.WithSize (1000, 500)

let createAccelerometerChart (events: Sensors.Event List) = 
    let x = events
            |> List.filter (fun x -> (x.sensorName = "Accelerometer"))
            |> List.map (fun x -> (getHHMM x.timestamp, x.event.x))

    let y = events
            |> List.filter (fun x -> (x.sensorName = "Accelerometer"))
            |> List.map (fun x -> (getHHMM x.timestamp, x.event.y))
    
    let z = events
            |> List.filter (fun x -> (x.sensorName = "Accelerometer"))
            |> List.map (fun x -> (getHHMM x.timestamp, x.event.z))

    let options =
        Options
            ( title = "Accelerometer", curveType = "function",
              legend = Legend(position = "bottom") )

    [x; y; z]
    |> Chart.Line
    |> Chart.WithLabels ["x"; "y"; "z"]
    |> Chart.WithOptions options
    |> Chart.WithLegend true
    |> Chart.WithSize (1000, 500)