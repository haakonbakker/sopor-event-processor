module Data
open Newtonsoft.Json
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

// Bucket Types:

type CKAssetValue = {
  fileChecksum : string
  size : int
  downloadURL : string
}

type CKAsset = {
  value : CKAssetValue
}

type Fields = {
  data: CKAsset
  eventCount : DataInt
  sessionIdentifier: DataStr
}
type Bucket = {
  recordName : string
  recordType : string
  fields : Fields
}

type CloudRecord = {
    records : Bucket list
}

// SamplingData Types:
type SamplingDataFields = {
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
    fields : SamplingDataFields
    created: Created
}

type SamplingDataRecord = {
    records : SamplingData list
}

// Session
type SessionData = {
    recordName : string
    recordType : string
    fields : SamplingDataFields
    created: Created
}

type SessionDataRecord = {
    records : SessionData list
}

let getDate (timestamp:string) =
    let timestampInt64 = (timestamp |> int64)
    let dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestampInt64);
    let dateTime = dateTimeOffset.UtcDateTime;
    dateTime.ToString("MM/dd/yyyy HH:mm")

let bucketData (bucket:Bucket) = 
    let bucket = CloudKit.fetchBucket bucket.fields.data.value.downloadURL
    (JsonConvert.DeserializeObject<Sensors.Event list>
        (bucket, JsonSerializerSettings(MissingMemberHandling = MissingMemberHandling.Ignore)))

let getBuckets sessionIdentifier = 
    let bucketsStr = CloudKit.fetch CloudKit.bodyBucket
    let buckets =
        (JsonConvert.DeserializeObject<CloudRecord>
            (bucketsStr, JsonSerializerSettings(MissingMemberHandling = MissingMemberHandling.Ignore)))
    buckets.records
    |> List.filter (fun x -> x.fields.sessionIdentifier.value = sessionIdentifier)
    |> List.collect bucketData

let getSamplingData (cloudRecordStr:string) =
    let points =
        (JsonConvert.DeserializeObject<SamplingDataRecord>
            (cloudRecordStr, JsonSerializerSettings(MissingMemberHandling = MissingMemberHandling.Ignore)))
    points.records

let getSessionData (cloudRecordStr:string) =
    let points =
        (JsonConvert.DeserializeObject<SessionDataRecord>
            (cloudRecordStr, JsonSerializerSettings(MissingMemberHandling = MissingMemberHandling.Ignore)))
    points.records

let getSamplingDataWithSession sessionIdentifier (cloudRecordStr:string) =
    let points =
        (JsonConvert.DeserializeObject<SamplingDataRecord>
            (cloudRecordStr, JsonSerializerSettings(MissingMemberHandling = MissingMemberHandling.Ignore)))
    points.records
    |> List.filter (fun x -> x.fields.sessionIdentifier.value = sessionIdentifier)
(*
  How to get the bucket data
  1. Fetch buckets DONE
  2. Download the CKAsset DONE
  3. DUMP THE BUCKET DB!!! DONE
  3. Parse and add to correct datatype DONE
  4. Make chart
*)