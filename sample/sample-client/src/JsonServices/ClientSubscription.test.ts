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
        const sub = new ClientSubscription();
        sub.subscriptionId = "3";
        sub.eventName = "ThirdEvent";
        sub.eventFilter = {
            Name: "wa"
        };

        const subMsg = sub.createSubscriptionMessage();
        expect(subMsg.EventName).toEqual(sub.eventName);
        expect(subMsg.SubscriptionId).toEqual(sub.subscriptionId);
        expect(subMsg.EventFilter).toEqual(sub.eventFilter);
        expect(subMsg.Enabled).toEqual(true);

        const unsubMsg = sub.createUnsubscriptionMessage();
        expect(unsubMsg.EventName).toEqual(sub.eventName);
        expect(unsubMsg.SubscriptionId).toEqual(sub.subscriptionId);
        expect(unsubMsg.EventFilter).toEqual(sub.eventFilter);
        expect(unsubMsg.Enabled).toEqual(false);
    });
});
