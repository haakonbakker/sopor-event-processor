module CloudKit

open FSharp.Data
open System.Net
open System
open System.Security.Cryptography.X509Certificates
open System.Net.Security


let sampleBody = """{
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

let sampleBodyWithSessionID sID = 
    sprintf """{
                "query": {
                    "filterBy": [
            	        {
            	            "fieldName": "sessionIdentifier",
            	            "comparator": "EQUALS",
            	            "fieldValue": {
            	                "value": "%s",
            	                "type": "STRING"
            	            }
            	        }
            	    ],
                    "recordType": "SamplingData",
                    "sortBy": [
                        {
                            "systemFieldName": "createdTimestamp",
                            "ascending": true
                        }
                    ]
                }
            }""" sID

let bodyBucket = """{
    "query": {
        "recordType": "Buckets",
        "sortBy": [
            {
                "systemFieldName": "createdTimestamp",
                "ascending": true
            }
        ]
    }
}"""

let bodyBucketWithFilter sID =
    sprintf  """{
                    "query": {
                        "filterBy": [
                	        {
                	            "fieldName": "sessionIdentifier",
                	            "comparator": "EQUALS",
                	            "fieldValue": {
                	                "value": "%s",
                	                "type": "STRING"
                	            }
                	        }
                	    ],
                        "recordType": "Buckets",
                        "sortBy": [
                            {
                                "systemFieldName": "createdTimestamp",
                                "ascending": true
                            }
                        ]
                    }
                }""" sID

let fullBodyBucket sID contMarker =
    sprintf  """{
                "query": {
                    "filterBy": [
            	        {
            	            "fieldName": "sessionIdentifier",
            	            "comparator": "EQUALS",
            	            "fieldValue": {
            	                "value": "%s",
            	                "type": "STRING"
            	            }
            	        }
            	    ],
                    "recordType": "Buckets",
                    "sortBy": [
                        {
                            "systemFieldName": "createdTimestamp",
                            "ascending": true
                        }
                    ]
                },
                "continuationMarker": "%s"
            }""" sID contMarker

let sessionBody = """{
    "query": {
        "recordType": "Session",
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

let fetch queryBody =
    let token = Environment.GetEnvironmentVariable "CLOUDKIT_TOKEN"
    let url = "https://api.apple-cloudkit.com/database/1/iCloud.com.bakkertechnologies.osa-tracker-watch.watchkitapp.watchkitextension/development/public/records/query?ckAPIToken=" + token
    ServicePointManager.ServerCertificateValidationCallback <- RemoteCertificateValidationCallback certValidator
    Http.RequestString
        (url, httpMethod = "POST", body = TextRequest queryBody,
         headers =
             [ "Content-Type", "application/json"
               "Accept", HttpContentTypes.Json
               "User-Agent", "PostmanRuntime/7.22.0"
               "Host", "api.apple-cloudkit.com"
             ])

let fetchBucket url = 
    ServicePointManager.ServerCertificateValidationCallback <- RemoteCertificateValidationCallback certValidator
    Http.RequestString
        (url, httpMethod = "GET",
         headers =
             [ "Content-Type", "application/json"
               "Accept", HttpContentTypes.Json
               "User-Agent", "PostmanRuntime/7.22.0"
               "Host", "api.apple-cloudkit.com"
               "Accept-Encoding", "gzip, deflate, br"
             ])
