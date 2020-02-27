module CloudKit

open Newtonsoft.Json.Linq
open Newtonsoft.Json
open FSharp.Data
open System
open System.Text
open System.Net
open System.Security.Cryptography.X509Certificates
open System.Net.Security

let body = """{
    "query": {
        "recordType": "SamplingData",
        "sortBy": [
            {
                "systemFieldName": "createdTimestamp",
                "ascending": true
            }
        ]
    }
}"""

// Forcing F# to accept the url.
let certValidator _ (cert:X509Certificate) (_:X509Chain) (_:SslPolicyErrors) = true

let fetchSamplingRecords () =
    let url = "https://api.apple-cloudkit.com/database/1/iCloud.com.bakkertechnologies.osa-tracker-watch.watchkitapp.watchkitextension/development/public/records/query?ckAPIToken=09e11a4783f4482769c48c4ec9b70725788178ad22f76b485ef3c2f6a214a0c9"
    let body = HttpRequestBody.TextRequest body
    ServicePointManager.ServerCertificateValidationCallback <- RemoteCertificateValidationCallback certValidator
    Http.RequestString
        (url, httpMethod = "POST", body = body,
         headers =
             [ "Content-Type", "application/json"
               "Accept", HttpContentTypes.Json
               "User-Agent", "PostmanRuntime/7.22.0"
               "Host", "api.apple-cloudkit.com"
             ])