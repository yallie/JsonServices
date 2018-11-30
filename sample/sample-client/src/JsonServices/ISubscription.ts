export interface ISubscription {
    eventName: string;
    eventHandler: (...args: any[]) => void;
    eventFilter?: {
        [key: string]: string;
    };
}