import { IReturnVoid } from './IReturn';

export class SubscriptionMessage implements IReturnVoid {
    public static messageName = "rpc.subscription";
    public getTypeName = () => SubscriptionMessage.messageName;

    public SubscriptionId: string;
	public Enabled: boolean;
	public EventName: string;
	public EventFilter?: {
        [key: string]: string;
    }
}