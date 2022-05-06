import { Injectable } from '@angular/core';
import { ISession } from '../../models/iSession';

@Injectable({
    providedIn: 'root'
})
export class SessionStorageService {
    private _sessionIdentifier: string = 'session';

    constructor() {}

    public cacheSession(session: ISession): void {
        localStorage.setItem(this._sessionIdentifier, JSON.stringify(session));
    }

    public removeSession(): ISession {
        var empty: ISession = { token: '' };
        this.cacheSession(empty);

        return empty;
    }

    public getSession(): ISession {
        var session: string | null = localStorage.getItem(this._sessionIdentifier);

        if (session === null) {
            return this.removeSession();
        }

        return (JSON.parse(session) as ISession); 
    }
}
