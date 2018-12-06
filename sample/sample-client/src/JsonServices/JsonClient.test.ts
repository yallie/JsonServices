import { GetVersion } from '../Messages/GetVersion';
import JsonClient from "./JsonClient";

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
});