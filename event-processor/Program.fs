// Learn more about F# at http://fsharp.org

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    let sessionID = "92AC8172-84D5-4F27-95D9-3D6A1E5C1715" // This session had Batchfrequency 5s, and aggregationInfoFrequency 5 min i.e. (60*5s)

    let samplingData = Events.getSamplingData sessionID (CloudKit.fetchSamplingRecords())
    let chart = Events.createBatteryChart samplingData
    chart.Show()

    let chart = Events.createAggregatedEventCount samplingData
    chart.Show()
    0 // return an integer exit code