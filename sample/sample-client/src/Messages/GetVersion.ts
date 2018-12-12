import { IReturn } from '../JsonServices/IReturn';
import { GetVersionResponse } from './GetVersionResponse';

export class GetVersion implements IReturn<GetVersionResponse> {
    public IsInternal: boolean;
    public getTypeName() {
        // return "JsonServices.Tests.Messages.GetVersion"
        return "Ultima.VersionChecker";
    }
    public createResponse() {
        return new GetVersionResponse();
    }
}

