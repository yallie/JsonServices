import { IReturnVoid } from "./IReturn";

export class LogoutMessage implements IReturnVoid {
    public getTypeName() {
        return "rpc.logout";
    }
}