import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SessionStorageService } from '../services/sessionStorage-service/session-storage.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    constructor(private _cacher: SessionStorageService) {}

    public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const sessionToken: string = this._cacher.getJWTSession().accessToken;

        if (sessionToken) {
            const newRequest: HttpRequest<any> = req.clone({
                headers: req.headers.set('authorization', sessionToken)
            });

            return next.handle(newRequest);
        }

        return next.handle(req);
    }
}