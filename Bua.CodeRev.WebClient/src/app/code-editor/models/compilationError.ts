export class CompilationError {
    public errorCode?: string;
    public message?: string;
    public startChar?: number;
    public endChar?: number;
    public startLine?: number;
    public endLine?: number;
}