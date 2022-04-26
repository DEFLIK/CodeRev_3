import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RequestService } from 'src/app/global-services/request/request.service';
import { EntryPoint } from '../../models/entryPoint';
import { ExecutionRequest } from '../../models/executionRequest';
import { ExecutionResult } from '../../models/executionResult';

@Injectable({
    providedIn: 'root'
})
export class CompileService {

    constructor(private _req: RequestService) { }

    public execute(code: string, entry: EntryPoint): Observable<ExecutionResult> {
        return this._req.put<ExecutionResult, ExecutionRequest>(
            `https://localhost:44343/api/compile/execute`,
            new ExecutionRequest(code, entry)
        );
    }
}
