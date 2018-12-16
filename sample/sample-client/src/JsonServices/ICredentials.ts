import JsonClient from './JsonClient';

export interface ICredentials {
    authenticate(client: JsonClient): Promise<any>;
}
