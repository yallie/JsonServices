export class AuthResponse {
    public Parameters: {
        [key: string]: string
    };

    public AuthenticatedIdentity: {
        Name: string;
        AuthenticationType: string;
        IsAuthenticated: boolean;
    }
}