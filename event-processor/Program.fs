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

let listSessions () = 
    let sessionStr = CloudKit.fetch CloudKit.sessionBody
    let sessions = Data.getSessionData sessionStr
    sessions
    |> List.mapi (fun i x -> createSession x.fields.sessionIdentifier.value (Data.getDate (Int64.Parse(x.created.timestamp) |> string)) i )
    

let chooseSession () =
    printfn "Choose session:" 
    let internalID = Console.ReadLine()
    printfn "Getting data from session: %s" internalID
    internalID

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    let sessionID = "92AC8172-84D5-4F27-95D9-3D6A1E5C1715" // This session had Batchfrequency 5s, and aggregationInfoFrequency 5 min i.e. (60*5s)
    let sessionList = listSessions ()
    sessionList
    |> List.map (fun x -> printfn "%d - Started: %s - SessionIdentifier: %s" x.interalID x.created x.sessionIdentifier)
    |> ignore


    let session = chooseSession ()

    let sessionObj = 
        sessionList
        |> List.find (fun x -> x.interalID = (int session))

    printfn "Will get data for: %A" sessionObj
    
    let samplingData = Data.getSamplingDataWithSession sessionObj.sessionIdentifier (CloudKit.fetch CloudKit.sampleBody)

    let chart = Charts.createBatteryChart samplingData
    chart.Show()

    let chart = Charts.createAggregatedEventCount samplingData
    chart.Show()

    let events = Data.getBuckets sessionObj.sessionIdentifier
    // printfn "%s" (str.ToString())
    let chart2 = Charts.createEventChart events "Heart Rate"
    chart2.Show()
    // printfn "%s" str
    0 // return an integer exit code