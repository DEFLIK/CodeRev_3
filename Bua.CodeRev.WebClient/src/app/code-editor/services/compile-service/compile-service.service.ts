import { HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from 'src/app/global-services/request/http.service';
import { RequestMethodType } from 'src/app/global-services/request/models/request-method';
import { EntryPoint } from '../../models/entryPoint';
import { ExecutionRequest } from '../../models/executionRequest';
import { ExecutionResult } from '../../models/executionResult';

@Injectable({
    providedIn: 'root'
})
export class CompileService {

    constructor(private _req: HttpService) { }

    public execute(code: string, entry: EntryPoint): Observable<HttpResponse<ExecutionResult>> {
        return this._req.request<ExecutionResult, ExecutionRequest>({
            url: `https://localhost:44343/api/compile/execute`,
            method: RequestMethodType.put,
            body: new ExecutionRequest(code, entry)
        });
    }
}
