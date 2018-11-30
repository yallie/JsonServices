import { IReturnVoid } from 'src/JsonServices/IReturn';

export class EventBroadcaster implements IReturnVoid {
    public EventName: string;
    public getTypeName() {
        return "JsonServices.Tests.Messages.EventBroadcaster";
    }
}