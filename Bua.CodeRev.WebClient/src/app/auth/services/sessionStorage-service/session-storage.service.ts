import { Injectable } from '@angular/core';
import { IJWTSession } from '../../models/iJwtSession';
import { UserSession } from '../../models/iUserSession';

@Injectable({
    providedIn: 'root'
})
export class SessionStorageService {
    private _sessionIdentifier: string = 'session';

    constructor() {}

    public cacheJWTSession(session: IJWTSession): void {
        localStorage.setItem(this._sessionIdentifier, JSON.stringify(session));
    }

    public removeJWTSession(): IJWTSession {
        var empty: IJWTSession = { accessToken: '' };
        this.cacheJWTSession(empty);

        return empty;
    }

    public getJWTSession(): IJWTSession {
        var session: string | null = localStorage.getItem(this._sessionIdentifier);

        if (session === null) {
            return this.removeJWTSession();
        }

        return (JSON.parse(session) as IJWTSession); 
    }

    public getJWTInfo(): UserSession {
        const jwtSession = this.getJWTSession();

        if (!jwtSession || !jwtSession.accessToken) {
            return new UserSession();
        }

        const newObj = this.parseJwt(jwtSession.accessToken);
        console.log(newObj);
        const session = new UserSession;
        Object.assign(session, newObj);

        return session;
    }

    public parseJwt(token: string): object {
        var base64Url = token.split('.')[1];
        var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        var jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
    
        return JSON.parse(jsonPayload);
    };
}
