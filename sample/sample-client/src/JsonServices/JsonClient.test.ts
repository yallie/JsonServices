
describe("JsonClient", () => {
    it("should contain at least one test", () => {
        expect(true).toBeTruthy();
    });
});

// the rest of tests are enabled if JsonServicesSampleServer environment variable is set
const sampleServer = process.env.JsonServicesSampleServer;
const descr = sampleServer ? describe : describe.skip;

// tslint:disable-next-line:no-console
console.log("Describe: " + descr.toString());
