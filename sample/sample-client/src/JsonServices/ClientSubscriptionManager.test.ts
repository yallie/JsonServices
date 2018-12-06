import { ClientSubscription } from './ClientSubscription';
import { ClientSubscriptionManager } from "./ClientSubscriptionManager";

describe("ClientSubscriptionManager", () => {
    it("should broadcast events to the added subscriptions", () => {
        const sm = new ClientSubscriptionManager();
        const sub = new ClientSubscription();
        sub.subscriptionId = "1";
        sub.eventName = "MyEvent";

        let handled = false;
        let args: object = {};
        sub.eventHandler = (e: object) => {
            handled = true;
            args = e;
        }

        const unsubscribe = sm.add(sub);
        sm.broadcast("SomeEvent", { name: "Hello" });
        expect(handled).toBeFalsy();
        expect(args).toEqual({});

        sm.broadcast("MyEvent", { name: "Goodbye" });
        expect(handled).toBeTruthy();
        expect(args).toEqual({ name: "Goodbye" });

        handled = false;
        unsubscribe();

        sm.broadcast("MyEvent", { name: "Hello again" });
        expect(handled).toBeFalsy();
        expect(args).toEqual({ name: "Goodbye" });
    });
})
