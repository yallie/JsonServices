import { IJsonClient } from './IJsonClient';
import { IReturn, IReturnVoid } from './IReturn';
import { ISubscription } from './ISubscription';
import { PendingMessage, IPendingMessageQueue } from './PendingMessage';
import { RequestMessage } from './RequestMessage';

export class JsonClient { // implements IJsonClient {
    public hello = "Hello!";
    /*
    constructor(public url: string) {
        this.webSocket = new WebSocket(url);
    }

    private webSocket: WebSocket;
    private pendingMessages: IPendingMessageQueue = {};

    public call<T>(message: IReturn<T>): Promise<T>;
    public call(message: IReturnVoid): Promise<any>;
    public call<T>(message: IReturn<T> | IReturnVoid): Promise<any> {
        const name = this.nameOf(message);
        const messageId = this.generateMessageId();
        const msg = new RequestMessage(name, message, messageId);
        const serialized = JSON.stringify(msg);
        alert(serialized);

        const pendingMessage = new PendingMessage(messageId);
        this.pendingMessages[messageId] = pendingMessage;
        this.webSocket.send(serialized);

        var p = new Promise((resolve: any, reject: any) => {
            resolve(true);
        });

        return p;
    }

    // outgoing message ids
    private lastMessageId = 0;
    private generateMessageId() {
        return this.lastMessageId++ + "";
    }

    // stolen from the ServiceStack client
    public nameOf = (o: any) => {
        if (!o) {
            return "null";
        }

        if (typeof o.getTypeName === "function") {
            return o.getTypeName();
        }

        const ctor = o && o.constructor;
        if (ctor === null) {
            throw new Error(`${o} doesn't have constructor`);
        }

        if (ctor.name) {
            return ctor.name;
        }

        const str = ctor.toString();
        return str.substring(9, str.indexOf("(")); // "function ".length == 9
    };

    // one-way calls
    public notify = (message: IReturnVoid): void => { };

    // returns unsubscription method
    public subscribe<T>(event: ISubscription<T>) {
        return () => { };
    };
    */
}

export default JsonClient;