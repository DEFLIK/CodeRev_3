import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateChild, CanDeactivate, CanLoad, Route, Router, RouterStateSnapshot, UrlSegment, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth-service/auth.service';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanLoad, CanActivateChild {

    constructor(
        private _auth: AuthService,
        private _router: Router
    ) {}

    public async canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot): Promise<boolean> {
        return true; // todo
    }

    public async canLoad(
        route: Route, 
        segments: UrlSegment[]): Promise<boolean> {
        return true; // todo
    }

    public async canActivateChild(
        childRoute: ActivatedRouteSnapshot, 
        state: RouterStateSnapshot): Promise<boolean> {
        return true; // todo
    }
}
