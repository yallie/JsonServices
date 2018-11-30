import { SubscriptionMessage } from './SubscriptionMessage';

export class ClientSubscription {
    public subscriptionId: string;
    public eventName: string;
    public eventHandler: (...args: any) => void;
    public eventFilter?: {
        [key: string]: string;
    }

    public invoke(...args: any) {
        // TODO:
        // 1. handle 'this' context
        // 2. apply eventFilter locally (we might get events matching other subscriber's event filter)
        this.eventHandler(args);
    }

    public createSubscriptionMessage = () => {
        const msg = new SubscriptionMessage();
        msg.Enabled = true;
        msg.EventName = this.eventName;
        msg.EventFilter = this.eventFilter;
        msg.SubscriptionId = this.subscriptionId;
        return msg;
    }

    public createUnsubscriptionMessage = () => {
        const msg = this.createSubscriptionMessage();
        msg.Enabled = false;
        return msg;
    }
}