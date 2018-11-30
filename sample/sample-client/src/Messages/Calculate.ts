import { IReturn } from '../JsonServices/IReturn';
import { CalculateResponse } from './CalculateResponse';

export class Calculate implements IReturn<CalculateResponse> {
    public FirstOperand: number;
    public Operation: string;
    public SecondOperand: number;
    public getTypeName() {
        return "JsonServices.Tests.Messages.Calculate"
    }
    public createResponse() {
        return new CalculateResponse();
    }
}

