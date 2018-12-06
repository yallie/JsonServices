import { AuthResponse } from './AuthResponse';
import { IReturn } from './IReturn';

export class AuthRequest implements IReturn<AuthResponse> {
    public static userNameKey = "UserName";
    public static passwordKey = "Password";
    public getTypeName = () => "rpc.authenticate";
    public createResponse = () => new AuthResponse();
    public Parameters: {
        [key: string]: string;
    } = {};
}