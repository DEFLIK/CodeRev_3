import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateChild, CanDeactivate, CanLoad, Route, Router, RouterStateSnapshot, UrlSegment, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { HttpService } from 'src/app/global-services/request/http.service';
import { RequestMethodType } from 'src/app/global-services/request/models/request-method';
import { AuthService } from '../services/auth-service/auth.service';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanLoad, CanActivateChild {

    constructor(
        private _req: HttpService,
        private _router: Router
    ) {}

    public async canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot): Promise<boolean> {
        return this.validate();
    }

    public async canLoad(
        route: Route, 
        segments: UrlSegment[]): Promise<boolean> {
        return this.validate();
    }

    public async canActivateChild(
        childRoute: ActivatedRouteSnapshot, 
        state: RouterStateSnapshot): Promise<boolean> {
        return this.validate();
    }

    private validate(): boolean {
        this._req.request({
            url: `http://localhost:62167/api/auth/validate-role?role=candidate`,
            method: RequestMethodType.get
        }).subscribe();

        return true;
    }
}
