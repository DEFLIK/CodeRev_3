import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { HttpService } from 'src/app/global-services/request/http.service';
import { RequestMethodType } from 'src/app/global-services/request/models/request-method';
import { UrlRoutes } from 'src/app/global-services/request/models/url-routes';
import { CandidateCardInfo } from '../models/candidateCardInfo';
import { CandidateCardInfoResponse } from '../models/response/candidateCardInfo-response';

@Injectable({
    providedIn: 'root'
})
export class ReviewService {

    constructor(
        private _http: HttpService
    ) { }

    public getCards(): Observable<CandidateCardInfo[]> {
        return this._http.request<CandidateCardInfoResponse[]>({
            url: `${UrlRoutes.user}/api/interviews/get-cards`,
            method: RequestMethodType.get,
            auth: true }
        ).pipe(
            map(resp => 
                resp.body
                    ?.map(res => new CandidateCardInfo(res)) ?? []));
    }

    
}
