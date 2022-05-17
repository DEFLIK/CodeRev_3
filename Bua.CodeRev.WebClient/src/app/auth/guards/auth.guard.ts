import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateChild, CanDeactivate, CanLoad, Route, Router, RouterStateSnapshot, UrlSegment, UrlTree } from '@angular/router';
import { catchError, map, Observable, of, tap } from 'rxjs';
import { HttpService } from 'src/app/global-services/request/http.service';
import { RequestMethodType } from 'src/app/global-services/request/models/request-method';
import { AuthService } from '../services/auth-service/auth.service';
import { UserRole } from '../models/userRole';
import { SessionStorageService } from '../services/sessionStorage-service/session-storage.service';
import { RolesController } from '../models/rolesController';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanLoad, CanActivateChild {
    constructor(
        private _auth: AuthService,
        private _cacher: SessionStorageService,
        private _router: Router
    ) {}

    public canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot): Observable<boolean> {
        return this.validate(route.routeConfig?.path ?? '');
    }

    public canLoad(
        route: Route, 
        segments: UrlSegment[]): Observable<boolean> {
        return this.validate(route.path ?? '');
    }

    public canActivateChild(
        childRoute: ActivatedRouteSnapshot, 
        state: RouterStateSnapshot): Observable<boolean> {
        return this.validate(childRoute.routeConfig?.path ?? '');
    }

    public validate(route: string): Observable<boolean> {
        return this._auth.checkCurrentSessionValid()
            .pipe(
                catchError((err) => {
                    return of();
                }),
                map(resp => {
                    const currRole = this._cacher.getJWTInfo().role;

                    if (!currRole || !resp.ok) {
                        return false;
                    }

                    switch(route) {
                        case('review'):
                            return RolesController.reviewRoles.includes(currRole ?? 0);
                        case('contest'):
                            return RolesController.contestRoles.includes(currRole ?? 0);
                        default:
                            return false;
                    }
                }),
                tap(res => {
                    if (!res) {
                        this._router
                            .navigateByUrl('');
                    }
                })
            );
    }
}
