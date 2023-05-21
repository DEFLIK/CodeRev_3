export class TestsRunResponse {
    public isCompiledSuccessfully?: boolean;
    public failedTestCases?: Map<string, string>;
    public passedTestCases?: string[];
}