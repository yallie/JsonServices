import { IReturnVoid } from './IReturn';

interface ISubscription {
    SubscriptionId: string;
    Enabled: boolean;
    EventName: string;
    EventFilter?: {
        [key: string]: string;
    }
}

export class SubscriptionMessage implements IReturnVoid {
    public static messageName = "rpc.subscription";
    public getTypeName = () => SubscriptionMessage.messageName;
    public Subscriptions: ISubscription[];
}
