import { HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, tap } from 'rxjs';
import { HttpService } from 'src/app/global-services/request/http.service';
import { RequestMethodType } from 'src/app/global-services/request/models/request-method';
import { UrlRoutes } from 'src/app/global-services/request/models/url-routes';
import { InterviewSolutionInfo } from '../models/interviewSolutionInfo';
import { InterviewSolutionInfoResposne } from '../models/response/interviewSolutionInfo-response';
import { LastSavedCodeResponse } from '../models/response/lastSavedCode-response';
import { TaskSolutionInfoResponse } from '../models/response/taskSolutionInfo-response';
import { TaskSolutionInfo } from '../models/taskSolutionInfo';

@Injectable({
    providedIn: 'root'
})
export class ContestService {
    public currentInterview?: InterviewSolutionInfo;
    public currentTask?: TaskSolutionInfo;
    public get isSolutionExpired(): boolean {
        // to fix after continue
        if (!this.currentInterview || this.currentInterview.endTimeMs === -1) {
            return false;
        }

        return this.currentInterview.endTimeMs < Date.now();
    }
    public get isSolutionComplete(): boolean {
        return this.currentInterview?.isSubmittedByCandidate ?? false;
    }

    public get isSolutionInMeet(): boolean {
        return !this.isSolutionComplete && !this.isSolutionExpired && (this.currentInterview?.isSynchronous ?? false);
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
            url: `${UrlRoutes.user}/api/contest/end-task-sln?id=${taskSolutionId}`,
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

    public endSolution(): Observable<HttpResponse<void>> {
        return this._http.request<void>({
            url: `${UrlRoutes.user}/api/contest/end-i-sln?id=${this.currentInterview?.id}`,
            method: RequestMethodType.put,
            auth: true
        }).pipe(
            tap(resp => {
                if (resp.ok && this.currentInterview) {
                    this.currentInterview.isSubmittedByCandidate = true;
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

    public getLastSavedCode(taskSlnId: string): Observable<HttpResponse<LastSavedCodeResponse>> {
        return this._http.request<LastSavedCodeResponse>({
            url: `${UrlRoutes.tracker}/api/v1.0/tracker/get-last-code?taskSolutionId=${taskSlnId}`,
            method: RequestMethodType.get,
            auth: true
        });
    }

    public reduceTaskSolutionAttempt(taskSlnId: string): Observable<HttpResponse<number>> {
        return this._http.request<number>({
            url: `${UrlRoutes.user}/api/tasks/solution/reduce-attempt?id=${taskSlnId}`,
            method: RequestMethodType.post,
            auth: true
        });
    }

    // todo endInterviewSln();
}
