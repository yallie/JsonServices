import { AuthRequest } from './AuthRequest';
import { ICredentials } from './ICredentials';
import { IJsonClient } from './IJsonClient';

export class CredentialsBase implements ICredentials {
    constructor(credentials?: {
        userName: string,
        password: string,
    } | undefined) {
        // initialize parameters if specified
        if (credentials) {
            this.parameters[AuthRequest.userNameKey] = credentials.userName;
            this.parameters[AuthRequest.passwordKey] = credentials.password;
        }
    }

    public async authenticate(client: IJsonClient): Promise<any> {
        const msg = new AuthRequest();
        msg.Parameters = this.parameters;
        await client.call(msg);
    }

    public parameters: {
        [key: string]: string;
    } = {}
}