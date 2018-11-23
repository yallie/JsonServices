export interface ISubscription<T> {
    eventName: string;
    handler: (eventArgs: T) => void;
    filter?: {
        [key: string]: string;
    };
}