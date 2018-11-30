// marker interfaces for building the strong-typed message apis
export interface IReturnVoid {
    getTypeName?(): string;
}

export interface IReturn<T> {
    getTypeName?(): string;
    createResponse(): T;
}
