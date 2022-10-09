import { HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { SaveChunkResponse } from 'src/app/code-editor/models/response/saveChunk-response';
import { SaveChunk } from 'src/app/code-editor/models/saveChunk';
import { HttpService } from 'src/app/global-services/request/http.service';
import { RequestMethodType } from 'src/app/global-services/request/models/request-method';
import { UrlRoutes } from 'src/app/global-services/request/models/url-routes';
import { CandidateCardInfo } from '../models/candidateCardInfo';
import { InterviewInfo } from '../models/interviewInfo';
import { Invitation } from '../models/invitation';
import { ReviewCommentRequest } from '../models/request/comment-request';
import { InvitationRequest } from '../models/request/invitation-request';
import { CandidateCardInfoResponse } from '../models/response/candidateCardInfo-response';
import { InterviewInfoResponse } from '../models/response/interviewInfo-response';
import { InterviewSolutionReviewResponse } from '../models/response/interviewSolutionReview-response';
import { InvitationResponse } from '../models/response/invitation-response';

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

    public setInterviewGrade(slnId: string, grade: number): Observable<HttpResponse<void>> {
        return this._http.request<void>({
            url: `${UrlRoutes.user}/api/review/put-i-sln-grade?id=${slnId}&grade=${grade}`,
            method: RequestMethodType.put,
            auth: true
        });
    }

    public getInterviews(): Observable<HttpResponse<InterviewInfoResponse[]>> {
        return this._http.request<InterviewInfoResponse[]>({
            url: `${UrlRoutes.user}/api/review/interviews`,
            method: RequestMethodType.get,
            auth: true
        });
    }

    public createInvite(invitation: Invitation): Observable<HttpResponse<InvitationResponse>> {
        return this._http.request<InvitationResponse, InvitationRequest>({
            url: `${UrlRoutes.user}/api/users/create-invitation`,
            method: RequestMethodType.post,
            auth: true,
            body: new InvitationRequest(invitation)
        });
    }

    public setInterviewComment(slnId: string, comment: string): Observable<HttpResponse<void>> {
        return this._http.request<void, ReviewCommentRequest>({
            url: `${UrlRoutes.user}/api/review/put-i-sln-comment?id=${slnId}`,
            method: RequestMethodType.put,
            auth: true,
            body: new ReviewCommentRequest(comment)
        });
    }

    public getVacancies(): Observable<HttpResponse<string[]>> {
        return this._http.request<string[]>({
            url: `${UrlRoutes.user}/api/review/vacancies`,
            method: RequestMethodType.get,
            auth: true,
        });
    }
    
    public getSaves(taskSlnId: string): Observable<HttpResponse<SaveChunkResponse[]>> {
        return this._http.request<SaveChunkResponse[]>({
            url: `${UrlRoutes.tracker}/api/v1.0/tracker/get?taskSolutionId=${taskSlnId}`,
            method: RequestMethodType.get,
            auth: true,
        });
    }

    // 0 - ne prinyat
    // 1 - podumat
    // 2 - prinyat
    public setInterviewResult(taskSlnId: string, result: number): Observable<HttpResponse<void>> {
        return this._http.request<void>({
            url: `${UrlRoutes.user}/api/review/put-i-sln-result?id=${taskSlnId}&result=${result}`,
            method: RequestMethodType.put,
            auth: true,
        });
    }
}
