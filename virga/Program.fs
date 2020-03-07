module Program
// Learn more about F# at http://fsharp.org
open System


type Session = {
    sessionIdentifier : string
    created: string
    interalID: int
}

let createSession sessionIdentifier time interalID =
    {sessionIdentifier = sessionIdentifier
     created = time
     interalID = interalID}

let getDate (timestamp: string) =
    let timestampInt64 = (timestamp |> int64)
    let dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestampInt64)
    let dateTime = dateTimeOffset.UtcDateTime
    dateTime.ToString("MM/dd/yyyy HH:mm")

let getSessionList () = 
    CloudKit.fetch CloudKit.sessionBody
    |> CloudKitProcessor.getSessionData
    |> List.mapi (fun i x -> createSession x.fields.sessionIdentifier.value (getDate (Int64.Parse(x.created.timestamp) |> string)) i )
    
let userChoose () =
    printfn "Choose session:" 
    let internalID = Console.ReadLine()
    printfn "Getting data from session: %s" internalID
    internalID

let chooseSession sessionList =
    sessionList
    |> List.map (fun x -> printfn "%d\t- Started: %s - SessionIdentifier: %s" x.interalID x.created x.sessionIdentifier ; x)
    |> fun x -> (userChoose (), x)
    |> fun (id, list) -> list |> (List.find (fun x -> x.interalID = (int id)))

let fetchAllBuckets sessionIdentifier filterOut =
    let buckets = CloudKitProcessor.getAllBuckets sessionIdentifier None 0
    printfn "\nNumber of buckets to fetch: %d" buckets.Length
    buckets
    |> List.filter (fun x -> x.fields.sessionIdentifier.value = sessionIdentifier)
    |> List.mapi (fun i x -> CloudKitProcessor.fetchBucketData i filterOut x)
    |> List.concat

[<EntryPoint>]
let main argv =
    printfn "** Sopor Event Proccessing **"
    printfn "** Loading sessions..."
    let chosenSession = 
        getSessionList () 
        |> chooseSession 

    printfn "Getting data for %d which started at %s" chosenSession.interalID chosenSession.created
    
    // Aggregated data
    let samplingData = CloudKitProcessor.fetchSamplingData chosenSession.sessionIdentifier
    if samplingData.Length = 0 then raise (Exception "Empty Sampling List")
    let batteryChart = Charts.createBatteryChart samplingData
    let eventCountChart = Charts.createAggregatedEventCount samplingData

    // Bucket data
    let events = fetchAllBuckets chosenSession.sessionIdentifier (Some "Heart Rate")
    if events.Length = 0 then raise (Exception "Empty events list")
    let heartRateChart = Charts.createEventChart events "Heart Rate"
    // let accelerometerChart = Charts.createAccelerometerChart events

    // Showing charts
    batteryChart.Show()
    eventCountChart.Show()
    heartRateChart.Show()
    // accelerometerChart.Show()
    0 // return an integer exit code