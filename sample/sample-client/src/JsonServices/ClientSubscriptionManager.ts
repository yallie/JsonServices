import { EventEmitter } from 'eventemitter3';
import { ClientSubscription } from './ClientSubscription';

export class ClientSubscriptionManager {
    private emitter = new EventEmitter();
    private subscriptions: { [subscriptionId: string]: ClientSubscription; } = {};

    public add = (subscription: ClientSubscription) => {
        this.subscriptions[subscription.subscriptionId] = subscription;
        this.emitter.on(subscription.eventName, subscription.invoke, subscription);

        return () => {
            delete this.subscriptions[subscription.subscriptionId];
            this.emitter.off(subscription.eventName, subscription.invoke, subscription);
        }
    }

    public broadcast = (eventName: string, eventArgs: object) => {
        this.emitter.emit(eventName, eventArgs);
    }
}