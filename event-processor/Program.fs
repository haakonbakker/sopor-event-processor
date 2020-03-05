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

let getSessionList () = 
    let sessionStr = CloudKit.fetch CloudKit.sessionBody
    let sessions = Data.getSessionData sessionStr
    sessions
    |> List.mapi (fun i x -> createSession x.fields.sessionIdentifier.value (Data.getDate (Int64.Parse(x.created.timestamp) |> string)) i )
    

let chooseSession () =
    printfn "Choose session:" 
    let internalID = Console.ReadLine()
    printfn "Getting data from session: %s" internalID
    internalID


let fetchAllBuckets sessionIdentifier =
    let buckets = Data.getAllBuckets sessionIdentifier None 0
    printfn "\nNumber of buckets to fetch: %d" buckets.Length
    buckets
    |> List.filter (fun x -> x.fields.sessionIdentifier.value = sessionIdentifier)
    |> List.mapi (fun i x -> Data.bucketData i x)
    |> List.concat

[<EntryPoint>]
let main argv =
    printfn "** Sopor Event Proccessing **"
    printfn "** Loading sessions..."
    let sessionList = getSessionList ()
    sessionList
    |> List.map (fun x -> printfn "%d\t- Started: %s - SessionIdentifier: %s" x.interalID x.created x.sessionIdentifier ; x)
    |> ignore


    let session = chooseSession ()

    let chosenSession = 
        sessionList
        |> List.find (fun x -> x.interalID = (int session))

    printfn "Will get data for: %A" chosenSession
    
    let samplingData = Data.getSamplingDataWithSession chosenSession.sessionIdentifier (CloudKit.fetch (CloudKit.sampleBodyWithSessionID chosenSession.sessionIdentifier))
    if samplingData.Length = 0 then raise (Exception "Empty Sampling List")

    let batteryChart = Charts.createBatteryChart samplingData
    let eventCountChart = Charts.createAggregatedEventCount samplingData
    let events = fetchAllBuckets chosenSession.sessionIdentifier
    let heartRateChart = Charts.createEventChart events "Heart Rate"

    batteryChart.Show()
    eventCountChart.Show()
    heartRateChart.Show()
    0 // return an integer exit code