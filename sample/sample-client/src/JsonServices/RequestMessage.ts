export class RequestMessage {
    constructor(public method: string, public params: object, public id?: string) {
    }
    public jsonrpc = "2.0";
}