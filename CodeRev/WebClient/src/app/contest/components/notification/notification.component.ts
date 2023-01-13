import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
    selector: 'app-notification',
    templateUrl: './notification.component.html',
    styleUrls: ['./notification.component.less']
})
export class NotificationComponent<T> {
    @Output()
    public confirmed = new EventEmitter();
    @Output()
    public declined = new EventEmitter();
    @Input()
    public topText?: string;
    @Input()
    public text?: string;
    @Input()
    public acceptText?: string;
    @Input()
    public declineText?: string;

    constructor() { }

    public confirm(): void {
        this.confirmed.emit();
    }

    public decline(): void {
        this.declined.emit();
    }

}
