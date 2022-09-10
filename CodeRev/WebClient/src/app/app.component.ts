import { Component } from '@angular/core';
import { NavigationCancel, NavigationEnd, NavigationError, NavigationStart, Router, RouterEvent } from '@angular/router';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.less']
})
export class AppComponent {
    public title = 'Bua.CodeRev.WebClient';
    public loading = false;
    public isAuthPage = false;


    constructor(private _router: Router) {
        this._router
            .events
            .subscribe((e: any) => {
                this.navigationInterceptor(e);
            });
    }

    public navigationInterceptor(event: RouterEvent): void {
        if (event instanceof NavigationStart) {
            this.loading = true;
        }
        if (event instanceof NavigationEnd) {
            this.isAuthPage = event.urlAfterRedirects.includes('auth');
            this.loading = false;
        }
        if (event instanceof NavigationCancel) {
            this.loading = false;
        }
        if (event instanceof NavigationError) {
            this.loading = false;
        }
    }
}
