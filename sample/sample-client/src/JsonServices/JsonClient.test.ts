import { Calculate } from '../Messages/Calculate';
import { EventBroadcaster } from '../Messages/EventBroadcaster';
import { GetVersion } from '../Messages/GetVersion';
import { IJsonRpcError, JsonClient } from "./JsonClient";

describe("JsonClient", () => {
    it("should contain at least one test", () => {
        expect(true).toBeTruthy();
    });
});

// sample server to connect to
const sampleServerUrl = "ws://127.0.0.1:8765"

// the rest of tests are enabled if JsonServicesSampleServer environment variable is set
// suggested here: https://github.com/facebook/jest/issues/3652
const sampleServer = process.env.JsonServicesSampleServer;
const conditional = sampleServer ? describe : describe.skip;

conditional("JsonClient", () => {
    it("should connect to the sample service", async () => {
        const client = new JsonClient(sampleServerUrl);
        await client.connect();
        client.disconnect();
    });

    it("should call GetVersion service", async () => {
        const client = new JsonClient(sampleServerUrl);
        await client.connect();

        const msg = new GetVersion();
        let result = await client.call(msg);
        expect(result.Version).toEqual("0.01-alpha");

        msg.IsInternal = true;
        result = await client.call(msg);
        expect(result.Version).toEqual("Version 0.01-alpha, build 12345, by yallie");

        client.disconnect();
    });

    it("should call Calculate service and trigger errors", async () => {
        const client = new JsonClient(sampleServerUrl);
        await client.connect();

        // 353 + 181
        const msg = new Calculate();
        msg.FirstOperand = 353;
        msg.Operation = "+";
        msg.SecondOperand = 181;
        let result = await client.call(msg);
        expect(result.Result).toEqual(534);

        // 353 - 181
        msg.Operation = "-";
        result = await client.call(msg);
        expect(result.Result).toEqual(172);

        // 353 # 181 — error
        msg.Operation = "#";
        try {
            result = await client.call(msg);
            fail("Service call 353 # 181 should yield an internal server error");
        } catch (e) {
            expect(e.code).toEqual(-32603);
            expect(e.message).toEqual("Internal server error: Bad operation: #");
            expect(e.data.indexOf("Invalid")).toBeGreaterThan(0);
        }

        // 353 % 0
        msg.Operation = "%";
        msg.SecondOperand = 0;
        try {
            result = await client.call(msg);
            fail("Service call 353 % 0 should yield a division by zero");
        } catch (e) {
            expect(e.code).toEqual(-32603);
            expect(e.message.startsWith("Internal server error")).toBeTruthy(); // error message is locale-specific
            expect(e.data.indexOf("DivideByZero")).toBeGreaterThan(0);
        }

        // 353 * 0
        msg.Operation = "*";
        result = await client.call(msg);
        expect(result.Result).toEqual(0);

        client.disconnect();
    });

    it("should subscribe to and unsubscribe from events", async () => {
        const client = new JsonClient(sampleServerUrl);
        await client.connect();

        let fired = false;
        let resolve: any;
        let promise = new Promise(r => resolve = r);
        const unsubscribe = await client.subscribe({
            eventName: "SomeEvent",
            eventHandler: e => {
                fired = true;
                resolve(true);
            },
        });

        const msg = new EventBroadcaster();
        msg.EventName = "SomeEvent";
        await client.call(msg);

        // should not time out
        setTimeout(() => resolve(false), 500);
        await promise;
        expect(fired).toBeTruthy();

        // reset event handler-related stuff
        fired = false;
        promise = new Promise(r => resolve = r);
        await unsubscribe();

        await client.call(msg);
        setTimeout(() => resolve(false), 500);

        // should time out
        await promise;
        expect(fired).toBeFalsy();

        client.disconnect();
    });

    it("should fire eventFilter when connection is refused", async () => {
        const badUrl = sampleServerUrl.replace("0", "d");
        const client = new JsonClient(badUrl);

        let fired = false;
        let error: Error | IJsonRpcError | null = null;
        client.errorFilter = e => {
            fired = true;
            error = e;
        }

        try {
            await client.connect();
            fail("Connect didn't throw any exceptions.");
        } catch {
            // ignore
        }

        expect(fired).toBeTruthy();
        expect(error).not.toBeNull();
    });

    it("should fire eventFilter when a call resulted in error", async () => {
        const client = new JsonClient(sampleServerUrl);
        await client.connect();

        let fired = false;
        let error: Error | IJsonRpcError | null = null;
        client.errorFilter = e => {
            fired = true;
            error = e;
        }

        // 353 & 181
        const msg = new Calculate();
        msg.FirstOperand = 353;
        msg.Operation = "&";
        msg.SecondOperand = 181;

        try  {
            await client.call(msg);
            fail("call(353 & 181) should have failed");
        } catch {
            // ignore
        }

        expect(fired).toBeTruthy();
        expect(error).not.toBeNull();
        expect((error as any).code).toEqual(-32603);
    });
});