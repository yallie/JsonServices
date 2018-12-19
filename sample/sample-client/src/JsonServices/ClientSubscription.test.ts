import { ClientSubscription } from "./ClientSubscription";

describe("ClientSubscription", () => {
    it("should invoke event handler", () => {
        const sub = new ClientSubscription();
        sub.subscriptionId = "1";
        sub.eventName = "MyEvent";

        let fired: boolean = false;
        sub.eventHandler = () => fired = true;
        sub.invoke({});

        expect(fired).toBeTruthy();
    });

    it("should take the filter into account", () => {
        const sub = new ClientSubscription();
        sub.subscriptionId = "2";
        sub.eventName = "AnotherEvent";
        sub.eventFilter = {
            Name: "wa"
        };

        let fired: boolean = false;
        let name: string = "";
        sub.eventHandler = (e: { Name: string }) => {
            fired = true;
            name = e.Name;
        };

        sub.invoke({ Name: "Mark Knopfler" });
        expect(fired).toBeFalsy();

        sub.invoke({ Name: "Mark Twain" });
        expect(fired).toBeTruthy();
        expect(name).toEqual("Mark Twain");

        fired = false;
        sub.invoke({ Name: "Walt Whitman" });
        expect(fired).toBeTruthy();
        expect(name).toEqual("Walt Whitman");
    });

    it("should create subscription and unsubscription messages", () => {
        const csub = new ClientSubscription();
        csub.subscriptionId = "3";
        csub.eventName = "ThirdEvent";
        csub.eventFilter = {
            Name: "wa"
        };

        const subMsg = csub.createSubscriptionMessage();
        expect(subMsg.Subscriptions).toBeDefined();
        expect(subMsg.Subscriptions.length).toEqual(1);

        const sub = subMsg.Subscriptions[0];
        expect(sub.EventName).toEqual(csub.eventName);
        expect(sub.SubscriptionId).toEqual(csub.subscriptionId);
        expect(sub.EventFilter).toEqual(csub.eventFilter);
        expect(sub.Enabled).toEqual(true);

        const unsubMsg = csub.createUnsubscriptionMessage();
        expect(unsubMsg.Subscriptions).toBeDefined();
        expect(unsubMsg.Subscriptions.length).toEqual(1);

        const unsub = unsubMsg.Subscriptions[0];
        expect(unsub.EventName).toEqual(csub.eventName);
        expect(unsub.SubscriptionId).toEqual(csub.subscriptionId);
        expect(unsub.EventFilter).not.toBeDefined();
        expect(unsub.Enabled).toEqual(false);
    });
});
