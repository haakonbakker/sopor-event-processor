module CloudKitProcessor

open Newtonsoft.Json
open System

type DataInt =
    { value: int }

type DataStr =
    { value: string }

type DataDouble =
    { value: double }

// Bucket Types:

type CKAssetValue =
    { fileChecksum: string
      size: int
      downloadURL: string }

type CKAsset =
    { value: CKAssetValue }

type Fields =
    { data: CKAsset
      eventCount: DataInt
      sessionIdentifier: DataStr }

type Bucket =
    { recordName: string
      recordType: string
      fields: Fields }

// type CloudRecord =
//     { records: Bucket list }


type CloudRecord =
    { records: Bucket list
      continuationMarker: string }

type CloudRecordCont =
    { records: Bucket list
      continuationMarker: string }

// SamplingData Types:
type SamplingDataFields =
    { aggregatedEventCount: DataInt
      sessionIdentifier: DataStr
      batteryLevel: DataDouble }

type Created =
    { timestamp: string
      userRecordName: string
      deviceID: string }

type SamplingData =
    { recordName: string
      recordType: string
      fields: SamplingDataFields
      created: Created }

type SamplingDataRecord =
    { records: SamplingData list }

// Session
type SessionData =
    { recordName: string
      recordType: string
      fields: SamplingDataFields
      created: Created }

type SessionDataRecord =
    { records: SessionData list }

let fetchBucketData i filterOut (bucket: Bucket) =
    printf "\rGetting bucket data nr: %d" i
    bucket.fields.data.value.downloadURL
    |> CloudKit.fetchBucket
    |> fun x ->
        (JsonConvert.DeserializeObject<SensorDomain.Event list>
            (x, JsonSerializerSettings(MissingMemberHandling = MissingMemberHandling.Ignore)))
    |> fun events ->
        match filterOut with
        | Some filter -> events |> List.filter (fun x -> (x.sensorName = filter))
        | None -> events

let rec getAllBuckets sessionIdentifier (contMarker: string option) i =
    printf "\rFetching bucket list: %d" i

    match contMarker with
    | Some x -> CloudKit.fetch (CloudKit.fullBodyBucket sessionIdentifier x)
    | None -> CloudKit.fetch (CloudKit.bodyBucketWithFilter sessionIdentifier)
    |> fun x ->
        (JsonConvert.DeserializeObject<CloudRecord>
            (x, JsonSerializerSettings(MissingMemberHandling = MissingMemberHandling.Ignore)))
    |> fun x ->
        (if isNull x.continuationMarker
         then x.records
         else x.records @ (getAllBuckets sessionIdentifier (Some x.continuationMarker) (i + 1)))


let getSamplingData (cloudRecordStr: string) =
    (JsonConvert.DeserializeObject<SamplingDataRecord>
        (cloudRecordStr, JsonSerializerSettings(MissingMemberHandling = MissingMemberHandling.Ignore)))
    |> fun x -> x.records

let getSessionData (cloudRecordStr: string) =
    (JsonConvert.DeserializeObject<SessionDataRecord>
        (cloudRecordStr, JsonSerializerSettings(MissingMemberHandling = MissingMemberHandling.Ignore)))
    |> fun x -> x.records

let getSamplingDataWithSession sessionIdentifier (cloudRecordStr: string) =
    (JsonConvert.DeserializeObject<SamplingDataRecord>
        (cloudRecordStr, JsonSerializerSettings(MissingMemberHandling = MissingMemberHandling.Ignore)))
    |> fun x -> x.records
    |> List.filter (fun x -> x.fields.sessionIdentifier.value = sessionIdentifier)

let fetchSamplingData sID =
    sID
    |> CloudKit.sampleBodyWithSessionID
    |> CloudKit.fetch
    |> getSamplingDataWithSession sID
