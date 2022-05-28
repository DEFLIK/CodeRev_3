import { CompilationErrorResponse } from './response/compilationError-response';

export class CompilationError {
    public errorCode: string;
    public message: string;
    public startChar: number;
    public endChar: number;
    public startLine: number;
    public endLine: number;

    constructor (resp: CompilationErrorResponse) {
        this.errorCode = resp.errorCode ?? '';
        this.message = resp.message ?? '';
        this.startChar = resp.startChar ?? 0;
        this.endChar = resp.endChar ?? 1;
        this.startLine = resp.startLine ?? 0;
        this.endLine = resp.endLine ?? 0;
    }
}