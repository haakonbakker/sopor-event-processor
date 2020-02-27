module Data
open FSharp.Data
open Newtonsoft.Json.Linq
open Newtonsoft.Json
open XPlot.GoogleCharts
open System
open System.Text.RegularExpressions


let data () = ()

type DataInt = {
    value: int
}

type DataStr = {
    value: string
}

type DataDouble = {
    value: double
}

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

let bucketData () = 
    let bucketsStr = CloudKit.fetchBuckets ()
    let buckets =
        (JsonConvert.DeserializeObject<CloudRecord>
            (bucketsStr, JsonSerializerSettings(MissingMemberHandling = MissingMemberHandling.Ignore)))
    printfn "%s" (buckets.records.[0].ToString())
    let bucket = CloudKit.fetchBucket buckets.records.[0].fields.data.value.downloadURL

    printfn "%s" (Regex.Unescape(bucket))
    buckets


(*
  How to get the bucket data
  1. Fetch buckets
  2. Download the CKAsset
  3. Unescape the random \n chars
  3. Parse and add to correct datatype
  4. Make chart
*)