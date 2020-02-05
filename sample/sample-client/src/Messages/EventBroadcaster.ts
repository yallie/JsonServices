import { IReturnVoid } from 'json-services-client'

export class EventBroadcaster implements IReturnVoid {
    public EventName!: string
    public StringArgument!: string // for eventName === "FilteredEvent"
    public getTypeName() {
        return "JsonServices.Tests.Messages.EventBroadcaster"
    }
}