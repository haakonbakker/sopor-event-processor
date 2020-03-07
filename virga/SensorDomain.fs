module SensorDomain

type EventData = {
    x: double
    y: double
    z: double
    batteryLevel: double
    batteryState : int
    heartRate : double
    unit : string
}

type Event = {
    sessionIdentifier : string
    timestamp : string
    sensorName : string
    event: EventData
}

