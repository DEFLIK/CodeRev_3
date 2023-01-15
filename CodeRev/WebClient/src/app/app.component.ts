import { AfterViewInit, Component } from '@angular/core';
import { NavigationCancel, NavigationEnd, NavigationError, NavigationStart, Router, RouterEvent } from '@angular/router';
import { Subscription } from 'rxjs';
import { NotificationInfo } from './review/models/notification';
import { ReviewService } from './review/services/review.service';
import { NotificationSignalrService } from './webcam/services/notificationsignalr.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.less']
})
export class AppComponent implements AfterViewInit{
    public title = 'Bua.CodeRev.WebClient';
    public loading = false;
    public isReview = false;
    public isShowingNotify = false;
    public notifications: NotificationInfo[] = [];
    private _notifySub? : Subscription;


    constructor(private _router: Router, private _review: ReviewService, private _notify: NotificationSignalrService) {
        this._router
            .events
            .subscribe((e: any) => {
                this.navigationInterceptor(e);
            });
    }
    public async ngAfterViewInit(): Promise<void> {
        await this._notify.startConnection('default');
    }

    public navigationInterceptor(event: RouterEvent): void {
        if (event instanceof NavigationStart) {
            this.loading = true;
        }
        if (event instanceof NavigationEnd) {
            this.isReview = event.urlAfterRedirects.includes('review');
            if (this._notifySub) {
                this._notifySub.unsubscribe();
            }

            if (this.isReview) {
                this._notifySub = this._notify.notification.subscribe(() => {
                    this.updateNotifications();
                });
                this.updateNotifications();
            }
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

    private updateNotifications(): void {
        this._review.getNotifications().subscribe(resp => {
            if (resp.ok && resp.body) {
                this.notifications = resp.body.map(n => new NotificationInfo(n)).reverse();
            }
        });
    }
}
