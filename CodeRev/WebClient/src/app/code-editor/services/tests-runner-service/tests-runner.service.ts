import { Injectable } from '@angular/core';
import { HttpService } from 'src/app/global-services/request/http.service';
import { TestsRunRequest } from '../../models/request/testsRun-request';
import { UrlRoutes } from 'src/app/global-services/request/models/url-routes';
import { RequestMethodType } from 'src/app/global-services/request/models/request-method';
import { TestsRunResponse } from '../../models/response/testsRun-response';
import { Observable, Subject } from 'rxjs';
import { HttpResponse } from "@angular/common/http";
import { RunAttemptsLeftResponse } from "../../models/response/runAttemptsLeft-response";

@Injectable({
    providedIn: 'root'
})
export class TestsRunnerService {

    public get onOutputRefresh$(): Observable<TestsRunResponse> {
        return this._onOutputRefresh$.asObservable();
    }

    private _onOutputRefresh$ = new Subject<TestsRunResponse>();

    constructor(
        private _req: HttpService,
    ) { }

    public run(code: string, taskSolutionId: string): void {
        this._req.request<TestsRunResponse, TestsRunRequest>({
            url: `${UrlRoutes.compiler}/api/tests/run`,
            method: RequestMethodType.post,
            body: new TestsRunRequest(code, taskSolutionId),
            timeout: 10000000
        }).subscribe(resp => {
            if (resp.ok ?? resp.body) {
                this.emitOutput(resp.body ?? new TestsRunResponse());
            } else {
                //todo
            }
        });
    }

    public reduceTaskSolutionAttempt(taskSlnId: string): Observable<HttpResponse<RunAttemptsLeftResponse>> {
        return this._req.request<RunAttemptsLeftResponse>({
            url: `${UrlRoutes.user}/api/tasks/solution/reduce-attempt?id=${taskSlnId}`,
            method: RequestMethodType.post,
            auth: true
        });
    }

    public emitOutput(res: TestsRunResponse): void {
        this._onOutputRefresh$.next(res);
    }
}
