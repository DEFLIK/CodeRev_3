import { HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { HttpService } from 'src/app/global-services/request/http.service';
import { RequestMethodType } from 'src/app/global-services/request/models/request-method';
import { UrlRoutes } from 'src/app/global-services/request/models/url-routes';
import { CandidateCardInfo } from '../models/candidateCardInfo';
import { CandidateCardInfoResponse } from '../models/response/candidateCardInfo-response';
import { InterviewSolutionReviewResponse } from '../models/response/interviewSolutionReview-response';

@Injectable({
    providedIn: 'root'
})
export class ReviewService {

    constructor(
        private _http: HttpService
    ) { }

    public getCards(): Observable<CandidateCardInfo[]> {
        return this._http.request<CandidateCardInfoResponse[]>({
            url: `${UrlRoutes.user}/api/review/cards`,
            method: RequestMethodType.get,
            withCredentials: true,
            auth: true
        }).pipe(
            map(resp => 
                resp.body
                    ?.map(res => new CandidateCardInfo(res)) ?? []));
    }

    public getSolutionReview(slnId: string): Observable<HttpResponse<InterviewSolutionReviewResponse>> {
        return this._http.request<InterviewSolutionReviewResponse>({
            url: `${UrlRoutes.user}/api/review/i-sln-info?id=${slnId}`,
            method: RequestMethodType.get,
            auth: true
        });
    }

    public setTaskGrade(taskId: string, grade: number): Observable<HttpResponse<void>> {
        return this._http.request<void>({
            url: `${UrlRoutes.user}/api/review/put-task-sln-grade?id=${taskId}&grade=${grade}`,
            method: RequestMethodType.put,
            auth: true
        });
    }

    public setInterviewResult(slnId: string, result: number): Observable<HttpResponse<void>> {
        return this._http.request<void>({
            url: `${UrlRoutes.user}/api/review/put-i-sln-result?id=${slnId}&result=${result}`,
            method: RequestMethodType.put,
            auth: true
        });
    }
    
}
