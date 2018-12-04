export class RequestMessage {
    public jsonrpc = "2.0";
    constructor(public method: string, public params: object, public id?: string) {
    }
}