import { EventFilter } from './EventFilter';
import { SubscriptionMessage } from './SubscriptionMessage';

export class ClientSubscription {
    public subscriptionId: string;
    public eventName: string;
    public eventHandler: (eventArgs: object) => void;
    public eventFilter?: {
        [key: string]: string;
    }

    public invoke = (eventArgs: object) => {
        // TODO: handle 'this' context?
        // apply eventFilter locally (we might get events matching other local subscriber's event filter)
        if (EventFilter.matches(this.eventFilter, eventArgs)) {
            this.eventHandler(eventArgs);
        }
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