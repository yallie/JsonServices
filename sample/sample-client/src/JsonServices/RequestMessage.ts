export class RequestMessage {
    constructor(public method: string, public parameters: object, public id?: string) {
    }
    public jsonrpc = "2.0";
}