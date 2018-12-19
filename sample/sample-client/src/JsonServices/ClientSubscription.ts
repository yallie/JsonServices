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
        msg.Subscriptions = [{
            Enabled: true,
            EventName: this.eventName,
            EventFilter: this.eventFilter,
            SubscriptionId: this.subscriptionId,
        }];
        return msg;
    }

    public createUnsubscriptionMessage = () => {
        const msg = this.createSubscriptionMessage();
        delete msg.Subscriptions[0].EventFilter;
        msg.Subscriptions[0].Enabled = false;
        return msg;
    }
}