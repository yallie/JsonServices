import { IJsonClient } from './IJsonClient';
import { IReturn, IReturnVoid } from './IReturn';
import { ISubscription } from './ISubscription';
import { IPendingMessageQueue, PendingMessage } from './PendingMessage';
import { RequestMessage } from './RequestMessage';

export class JsonClient implements IJsonClient {
    constructor(public url: string, private options = {
        reconnect: true,
        reconnectInterval: 5000,
        maxReconnects: 10,
    }) {
    }

    private webSocket?: WebSocket;
    private connected = false;
    private reconnects = 0;
    private pendingMessages: IPendingMessageQueue = {};

    public connectAsync(): Promise<boolean> {
        return new Promise<boolean>((resolve, reject) => {
            // check if already connected
            if (this.webSocket) {
                resolve(true);
                return;
            }

            this.webSocket = new WebSocket(this.url);

            this.webSocket.onopen = () => {
                this.connected = true;
                this.reconnects = 0;
                resolve(true);
            }

            this.webSocket.onclose = closeEvent => {
                this.connected = false;
                this.webSocket = undefined;

                if (closeEvent.code === 1000) {
                    resolve(false);
                    return; // closed normally, don't reconnect
                }

                this.reconnects++;
                if (this.options.reconnect && (this.options.maxReconnects < this.reconnects || this.options.maxReconnects === 0)) {
                    setTimeout(() => this.connectAsync(), this.options.reconnectInterval);
                }

                resolve(false);
            }

            this.webSocket.onmessage = message => {
                // if message is binary data, convert it to string
                let json = typeof(message.data) === "string" ? message.data : "";
                if (message.data instanceof ArrayBuffer) {
                    json = Buffer.from(message.data).toString();
                }

                // parse message and get its data
                let parsedMessage: {
                    id?: string,
                    result?: object,
                    error?: object,
                };

                try {
                    parsedMessage = JSON.parse(json);
                } catch {
                    // TODO: decide how to handle parse errors
                    return;
                }

                // check if it's a reply
                if (parsedMessage.id) {
                    const pending = this.pendingMessages[parsedMessage.id];
                    if (pending) {
                        // clear pending message
                        this.pendingMessages[parsedMessage.id] = undefined;

                        // resolve or reject the promise depending on the parsed message data
                        if (parsedMessage.error) {
                            pending.reject(parsedMessage.error);
                            return;
                        } else {
                            pending.resolve(parsedMessage.result);
                        }
                    }

                    // TODO: decide how to handle unknown responses from server
                    return;
                }

                // it's a notification
                // TODO: fire an event
            }
        });
    }

    public call<T>(message: IReturn<T>): Promise<T>;
    public call(message: IReturnVoid): Promise<any>;
    public call<T>(message: IReturn<T> | IReturnVoid): Promise<any> {
        const name = this.nameOf(message);
        const messageId = this.generateMessageId();
        const msg = new RequestMessage(name, message, messageId);
        const serialized = JSON.stringify(msg);
        alert(serialized);

        // prepare pending message
        const pendingMessage = new PendingMessage(messageId);
        this.pendingMessages[messageId] = pendingMessage;

        // return a promise awaiting the results of the call
        return pendingMessage.promise = new Promise((resolve, reject) => {
            // store resolve/reject callbacks for later use
            pendingMessage.resolve = resolve;
            pendingMessage.reject = reject;

            // fail early if not connected
            if (this.webSocket === undefined || !this.connected) {
                this.pendingMessages[messageId] = undefined;
                reject(new Error("WebSocket not connected"));
                return;
            }

            this.webSocket.send(serialized);
        });
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
    public notify = (message: IReturnVoid): void => {
        throw new Error("Notify is not implemented");
    };

    // returns unsubscription method
    public subscribe<T>(event: ISubscription<T>) {
        if (event) {
            throw new Error("Subscribe is not implemented");
        }

        return () => {
            // nothing here
        };
    };
}

export default JsonClient;