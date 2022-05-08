import { HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { RequestMethodType } from 'src/app/global-services/request/models/request-method';
import { UrlRoutes } from 'src/app/global-services/request/models/url-routes';
import { HttpService } from 'src/app/global-services/request/http.service';
import { IJWTSession } from '../../models/iJwtSession';
import { ILogin } from '../../models/iLogin';
import { IRegister } from '../../models/iRegister';
import { UserSession } from '../../models/iUserSession';
import { RolesController } from '../../models/rolesController';
import { UserRole } from '../../models/userRole';
import { EncryptionService } from '../encrypt-service/encrypt.service';
import { SessionStorageService } from '../sessionStorage-service/session-storage.service';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    constructor(
        private _req: HttpService,
        private _encr: EncryptionService,
        private _cacher: SessionStorageService,
        private _router: Router
    ) {}

    public register(inviteToken: string, userName: string, email: string, phone: string, pass: string): Observable<HttpResponse<unknown>> {
        const encryptedPass: string = this._encr.encryptString(pass);
        
        return this._req.request<unknown, IRegister>({
            url: `${UrlRoutes.user}/api/users/register?invite=${inviteToken}`,
            method: RequestMethodType.post,
            body: { 
                fullname: userName, 
                email: email, 
                passwordHash: encryptedPass,
                phonenumber: phone
            }
        });
    }

    public login(email: string, pass: string): Observable<HttpResponse<unknown>> {
        const encryptedPass: string = this._encr.encryptString(pass);
        
        const ans = this._req.request<IJWTSession, ILogin>({
            url: `${UrlRoutes.user}/api/auth/login`,
            method: RequestMethodType.post,
            body: { email: email, passwordHash: encryptedPass }
        });

        ans.subscribe(resp => {
            if (resp.ok) {
                this._cacher.cacheJWTSession(resp.body ?? { accessToken: '' });

                this._router
                    .navigateByUrl(
                        RolesController.getDefaultRoot(
                            this._cacher.getJWTInfo().role 
                            ?? UserRole.candidate));
            }
        });

        return ans;
    }

    public logout(): void {
        this._cacher.removeJWTSession();
        this._router.navigateByUrl('');
    }

    public isSessionValid(session: IJWTSession): Observable<HttpResponse<unknown>> {
        return this._req.request({
            url: `${UrlRoutes.user}/api/auth/validate-token?token=${this._cacher.getJWTSession().accessToken}`,
            method: RequestMethodType.get
        });
    }

    public checkCurrentSessionValid(): Observable<HttpResponse<unknown>> {
        const session = this._cacher.getJWTSession();
        
        if (session && session.accessToken) {
            return this.isSessionValid(session);
        }

        return of(new HttpResponse({
            status: 400
        }));
    }
}
