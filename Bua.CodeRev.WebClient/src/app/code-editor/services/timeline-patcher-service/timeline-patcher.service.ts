import { Injectable } from '@angular/core';
import { NgxVideoTimelineComponent } from 'ngx-video-timeline';
import { interval } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class TimelinePatcherService {

    constructor() { }

    // Monkey патч прикольной, но плохо сделанной, опенсурсной библиотеки ngx-video-timeline
    public patchTimelineComponent(timelineComp: NgxVideoTimelineComponent): void {
    // Фикс неправильного отслеживания позици мыши при выходе за рамки таймлайна
        const oldOnOut = timelineComp.mouseoutFunc.bind(timelineComp);
        timelineComp.mouseoutFunc = (): void => {
            oldOnOut();
            if (timelineComp.gIsMousedown) {
                const msPerPx = (timelineComp.zoom * 3600 * 1000) / timelineComp.canvasW;
                const event = new MouseEvent('up', {
                    clientX: (timelineComp.playTime as number) / msPerPx - (timelineComp.playTime as number)
                });
                timelineComp.mouseupFunc(event);
            }
                      
            return;
        };


        // Фикс странного перемещения при нажатии на ползунок
        timelineComp.playBarOffsetX1 = 0;
        timelineComp.playBarOffsetX1 = 0;
        timelineComp.playBarOffsetY1 = 0;
        timelineComp.playBarOffsetY2 = 0;
        
        // Фикс хардкод лимита на приближение
        const oldOnWheel = timelineComp.mousewheelFunc.bind(timelineComp);
        timelineComp.mousewheelFunc = (event: any): boolean => {
            const e = window.event || event;
            const delta = Math.max(-1, Math.min(1, e.wheelDelta || -e.detail));

            const middleTime =
                timelineComp.startTimestamp + (timelineComp.hoursPerRuler * timelineComp.playBarDistanceLeft * 3600 * 1000);
            if (delta < 0) {
                timelineComp.zoom += timelineComp.zoom > 4 ? 4 : 0.2;
                if (timelineComp.zoom >= 24) {
                    timelineComp.zoom = 24;
                }
                timelineComp.hoursPerRuler = timelineComp.zoom;
            } else if (delta > 0) {
                timelineComp.zoom -= timelineComp.zoom > 4 ? 4 : 0.2;
                if (timelineComp.zoom <= 0.1) {
                    timelineComp.zoom = 0.1;
                }
                timelineComp.hoursPerRuler = timelineComp.zoom;
            }
            

            timelineComp.clearCanvas();
            timelineComp.startTimestamp =
                middleTime - (timelineComp.hoursPerRuler * 3600 * 1000) / 2;

            timelineComp.init(timelineComp.startTimestamp, timelineComp.timecell, true);
            timelineComp.drawPalyBar();

            return false;
        };
    }
}
