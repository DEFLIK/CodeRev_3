import { Component } from '@angular/core';
import { NavigationCancel, NavigationEnd, NavigationError, NavigationStart, Router, RouterEvent } from '@angular/router';
import { NotificationInfo } from './review/models/notification';
import { ReviewService } from './review/services/review.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.less']
})
export class AppComponent {
    public title = 'Bua.CodeRev.WebClient';
    public loading = false;
    public isAuthPage = false;
    public isShowingNotify = false;
    public notifications: NotificationInfo[] = [];


    constructor(private _router: Router, private _review: ReviewService) {
        this._router
            .events
            .subscribe((e: any) => {
                this.navigationInterceptor(e);
            });
        if (!this.isAuthPage) {
            _review.getNotifications().subscribe(resp => {
                if (resp.ok && resp.body) {
                    this.notifications = resp.body.map(n => new NotificationInfo(n));
                }
            });
        }
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

    public showNotify(show: boolean): void {
        this.isShowingNotify = !this.isShowingNotify;
    }
}
