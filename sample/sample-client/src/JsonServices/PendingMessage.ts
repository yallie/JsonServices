export class PendingMessage {
    constructor(public id: string, public promise?: Promise<any>) {
    }

    public resolve: (result: any) => void;
    public reject: (error: any) => void;
}

export interface IPendingMessageQueue {
    [key: string]: PendingMessage | undefined;
}