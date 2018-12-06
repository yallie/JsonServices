export class RequestMessage {
    constructor(method: string, params: object, id?: string) {
        this.method = method;
        this.params = params;
        this.id = id;
    }

    public jsonrpc = "2.0";
    public method: string;
    public params: object;
    public id?: string;
}