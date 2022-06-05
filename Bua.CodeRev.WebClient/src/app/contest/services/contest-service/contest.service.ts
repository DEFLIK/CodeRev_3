import { HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { audit, map, Observable, Subject, tap } from 'rxjs';
import { HttpService } from 'src/app/global-services/request/http.service';
import { RequestMethodType } from 'src/app/global-services/request/models/request-method';
import { UrlRoutes } from 'src/app/global-services/request/models/url-routes';
import { InterviewSolutionInfo } from '../../models/interviewSolutionInfo';
import { InterviewSolutionInfoResposne } from '../../models/response/interviewSolutionInfo-response';
import { TaskSolutionInfoResponse } from '../../models/response/taskSolutionInfo-response';
import { TaskSolutionInfo } from '../../models/taskSolutionInfo';

@Injectable({
    providedIn: 'root'
})
export class ContestService {
    public currentInterview?: InterviewSolutionInfo;
    public currentTask?: TaskSolutionInfo;
    public get isSolutionExpired(): boolean {
        if (!this.currentInterview) {
            return false;
        }

        return this.currentInterview.endTimeMs < Date.now();
    }
    public get taskSelected$(): Observable<TaskSolutionInfo> {
        return this._taskSelected$.asObservable();
    }

    private _taskSelected$ = new Subject<TaskSolutionInfo>();
    
    constructor(private _http: HttpService) { }

    public selectTask(task: TaskSolutionInfo): void {
        this._taskSelected$.next(task);
        this.currentTask = task;
    }

    public startInterview(interviewSolutionId: string): Observable<HttpResponse<void>> {
        return this._http.request<void>({
            url: `${UrlRoutes.user}/api/contest/start-i-sln?id=${interviewSolutionId}`,
            method: RequestMethodType.put,
            auth: true
        }).pipe(tap((resp) => {
            if (resp.ok && resp.body) {
                this.continueInterview(resp.body);
            }
        }));
    }

    public getTasksInfo(interviewSolutionId: string): Observable<HttpResponse<TaskSolutionInfoResponse[]>> {
        return this._http.request<TaskSolutionInfoResponse[]>({
            url: `${UrlRoutes.user}/api/contest/task-slns-info?id=${interviewSolutionId}`,
            method: RequestMethodType.get,
            auth: true
        });
    }

    public endTask(taskSolutionId: string): Observable<HttpResponse<void>> {
        return this._http.request<void>({
            url: `${UrlRoutes.user}/api/contest/end-task-sln?id= ${taskSolutionId}`,
            method: RequestMethodType.put,
            auth: true
        }).pipe(
            tap(resp => {       
                if (resp.ok && this.currentTask) {
                    this.currentTask.isDone = true;
                    this.selectTask(this.currentTask);
                }
            })
        );
    }

    public getInterviewSolutionInfo(): Observable<HttpResponse<InterviewSolutionInfoResposne>> {
        return this._http.request<InterviewSolutionInfoResposne>({
            url: `${UrlRoutes.user}/api/contest/i-sln-info`,
            method: RequestMethodType.get,
            auth: true
        });
    }

    public continueInterview(sln: InterviewSolutionInfo): void {
        this.currentInterview = sln;
    }
}
