export class PendingMessage {
    constructor(public id: string, public promise?: Promise<any>) {
    }
}

export interface IPendingMessageQueue {
    [key: string]: PendingMessage;
}