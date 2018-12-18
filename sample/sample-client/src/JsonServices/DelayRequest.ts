import { IReturnVoid } from './IReturn';

export class DelayRequest implements IReturnVoid {
    constructor(ms?: number) {
        if (ms) {
            this.Milliseconds = ms;
        }
    }
    public Milliseconds: number;
    public getTypeName() {
        return "JsonServices.Tests.Messages.DelayRequest";
    }
}