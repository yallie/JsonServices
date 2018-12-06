import { IReturn, IReturnVoid } from "./IReturn";
import { ISubscription } from "./ISubscription";

export interface IJsonClient {
    connect(): Promise<any>;
    disconnect(): void;

    // two-way calls
    call<T>(message: IReturn<T>): Promise<T>;
    call(message: IReturnVoid): Promise<any>;

    // one-way calls
    notify(message: IReturnVoid): void;

    // returns unsubscription method
    subscribe(event: ISubscription): Promise<() => Promise<any>>;
}