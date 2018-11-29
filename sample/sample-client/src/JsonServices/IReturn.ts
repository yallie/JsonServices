// marker interfaces for building the strong-typed message apis
export interface IReturnVoid {
    getTypeName?(): string;
    createResponse() : any;
}

export interface IReturn<T> {
    getTypeName?(): string;
    createResponse(): T;
}
