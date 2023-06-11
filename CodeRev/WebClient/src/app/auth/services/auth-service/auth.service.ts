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
import { VkSession } from '../../models/vkSession';
import { IVkRegister } from '../../models/iVkRegister';

// eslint-disable-next-line @typescript-eslint/naming-convention
declare var VK:any;

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    public isProcessing: boolean = false;

    constructor(
        private _req: HttpService,
        private _encr: EncryptionService,
        private _cacher: SessionStorageService,
        private _router: Router
    ) {VK.init({ apiId: 51672186 });}

    public register(inviteToken: string, firstName: string, surname: string, email: string, phone: string, pass: string): Observable<HttpResponse<unknown>> {
        this.isProcessing = true;
        const encryptedPass: string = this._encr.encryptString(pass);

        const ans = this._req.request<unknown, IRegister>({
            url: `${UrlRoutes.user}/api/users/register?invite=${inviteToken}`,
            method: RequestMethodType.post,
            body: {
                firstName: firstName,
                surname: surname,
                email: email,
                passwordHash: encryptedPass,
                phonenumber: phone
            }
        });

        ans.subscribe({
            next: () => this.isProcessing = false,
            error: () => this.isProcessing = false
        });

        return ans;
    }

    public registerViaVk(inviteToken: string): void {
        this.isProcessing = true;

        this.getVkSession((vkAns: any) => {
            var vkSession = new VkSession(
                vkAns.session.expire,
                vkAns.session.mid,
                vkAns.session.secret,
                vkAns.session.sid,
                vkAns.session.sig);

            const regAns = this._req.request<unknown, IVkRegister>({
                url: `${UrlRoutes.user}/api/users/registerViaVk?invite=${inviteToken}`,
                method: RequestMethodType.post,
                body: {
                    firstName: vkAns.session.user.first_name,
                    surname: vkAns.session.user.last_name,
                    vkDomainLink: vkAns.session.user.href,
                    vkId: vkAns.session.user.id,
                    vkSession: vkSession
                }
            });

            regAns.subscribe({
                next: () => {
                    this.isProcessing = false; this.loginViaVk(
                        vkAns.session.user.id, 
                        vkSession
                    );
                },
                error: () => this.isProcessing = false
            });
        });
    }

    public login(email: string, pass: string): Observable<HttpResponse<unknown>> {
        this.isProcessing = true;
        const encryptedPass: string = this._encr.encryptString(pass);

        const ans = this._req.request<IJWTSession, ILogin>({
            url: `${UrlRoutes.user}/api/auth/login`,
            method: RequestMethodType.post,
            body: { email: email, passwordHash: encryptedPass }
        });

        ans.subscribe({
            next: resp => {
                if (resp.ok) {
                    this._cacher.cacheJWTSession(resp.body ?? { accessToken: '' });

                    this._router
                        .navigateByUrl(
                            RolesController.getDefaultRoot(
                                this._cacher.getJWTInfo().role
                                ?? UserRole.candidate));
                }
                this.isProcessing = false;
            },
            error: () => this.isProcessing = false
        });

        return ans;
    }

    public loginViaVk(userId: string, vkSession: VkSession): void {
        this.isProcessing = true;

        console.log('AAA', vkSession);
        

        const loginAns = this._req.request<IJWTSession, VkSession>({
            url: `${UrlRoutes.user}/api/auth/loginViaVk?vkId=${userId}`,
            method: RequestMethodType.post,
            body: vkSession
        });

        loginAns.subscribe({
            next: resp => {
                if (resp.ok) {
                    this._cacher.cacheJWTSession(resp.body ?? { accessToken: '' });

                    this._router
                        .navigateByUrl(
                            RolesController.getDefaultRoot(
                                this._cacher.getJWTInfo().role
                                ?? UserRole.candidate));
                }
                this.isProcessing = false;
            },
            error: () => this.isProcessing = false
        });
    }

    public getVkSession(callback: any): void {
        VK.Auth.login(callback);
    }

    public logout(): void {
        this._cacher.removeJWTSession();
        this._router.navigateByUrl('');
    }

    public isSessionValid(session: IJWTSession): Observable<HttpResponse<unknown>> {
        return this._req.request({
            url: `${UrlRoutes.user}/api/auth/validate?token=${this._cacher.getJWTSession().accessToken}`,
            method: RequestMethodType.get
        });
    }

    public isVkSessionValid(vkSession: VkSession): Observable<HttpResponse<unknown>> {
        return this._req.request<void, VkSession>({
            url: `${UrlRoutes.user}/api/auth/validateVk`,
            body: vkSession,
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
