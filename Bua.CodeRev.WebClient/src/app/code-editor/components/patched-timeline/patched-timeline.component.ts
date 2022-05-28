import { Component, ComponentFactoryResolver, OnDestroy, OnInit, Output, ViewChild, ViewContainerRef } from '@angular/core';
import { NgxVideoTimelineComponent, VideoCellType } from 'ngx-video-timeline';
import { interval, Subject, takeUntil } from 'rxjs';
import { EventEmitter } from '@angular/core';
import { RecordInfo } from '../../models/codeRecord';
import { TimelinePatcherService } from '../../services/timeline-patcher-service/timeline-patcher.service';

@Component({
    selector: 'app-patched-timeline',
    templateUrl: './patched-timeline.component.html',
    styleUrls: ['./patched-timeline.component.less']
})
export class PatchedTimelineComponent implements OnDestroy {
    @Output()
    public valueChanges: EventEmitter<number> = new EventEmitter<number>();
    @ViewChild('container', { read: ViewContainerRef })
    public container!: ViewContainerRef;
    public timeLineComp?: NgxVideoTimelineComponent;
    private _unsubscriber: Subject<void> = new Subject<void>();
    private _videoCells?: VideoCellType[];

    constructor(
        private _patcher: TimelinePatcherService,
        private _componentFactory: ComponentFactoryResolver,

    ) { }
    public ngOnDestroy(): void {
        this._unsubscriber.next();
    }

    public buildComponent(): void {
        this.container.clear();
        const comp = this._componentFactory
            .resolveComponentFactory(
                NgxVideoTimelineComponent
            );
        const compRef = this.container.createComponent(comp);
        this._videoCells = [];
        this.timeLineComp = compRef.instance;
        this.timeLineComp.videoCells = this._videoCells;
        this.timeLineComp.playBarColor = '#DA2323';

        this.subscribeToValueChange(this.timeLineComp.keyUp);
        this.subscribeToValueChange(this.timeLineComp.mouseDown);
        
        setTimeout(() => {
            if (!this.timeLineComp) {
                throw new Error('Timeline component got removed before patching ended');
            }

            this._patcher.patchTimelineComponent(this.timeLineComp);

            this.timeLineComp.zoom = 0.5;
            this.refreshZoom(this.timeLineComp);
        });
    }

    public setCurrentTime(time: number): void {
        if (!this.timeLineComp) {
            console.log('no comp');
            
            return;
        }

        console.log('set time');
        this.timeLineComp.isPlayClick = true;
        this.timeLineComp.playTime = Number(this.timeLineComp.startTimeThreshold) + time;
        this.timeLineComp.set_time_to_middle(this.timeLineComp.playTime);
    }

    public setProperties(startTime: number, duration: number, records: RecordInfo): void {
        setTimeout(() => {
            if (!this.timeLineComp || !this._videoCells) {
                throw new Error('Component not builded yet');
            }

            this.timeLineComp.startTimeThreshold = startTime;
            this.timeLineComp.endTimeThreshold = startTime + duration;
            this.timeLineComp.playTime = startTime;
            
            this.updateMarks(this._videoCells, records, startTime, duration);

            this.timeLineComp.onResize();
        });
    }

    public mfWheel(e: Event): void {
        this.timeLineComp?.mousewheelFunc(e);
    }

    private subscribeToValueChange(event: EventEmitter<number>): void {
        event
            .pipe(
                takeUntil(this._unsubscriber)
            )
            .subscribe((value: any) => {
                this.valueChanges.emit(value);
            });
    }

    private updateMarks(sourceContainer: VideoCellType[], newRecord: RecordInfo, startTime: number, duration: number): void {
        sourceContainer.splice(0, sourceContainer.length);
        for (const point of newRecord.points) {
            sourceContainer.push({
                beginTime: startTime + point.startTime,
                endTime: startTime + point.endTime + 5000,
                style: {
                    background: point.color
                }
            });
        }
        
        sourceContainer.push({
            beginTime: startTime,
            endTime: startTime + duration,
            style: {
                background: '#90cbff59'
            }
        });
    }

    private refreshZoom(timeLineComp: NgxVideoTimelineComponent): void {
        timeLineComp.mousewheelFunc(new WheelEvent('in', {
            deltaX: 1
        }));
    }
}
