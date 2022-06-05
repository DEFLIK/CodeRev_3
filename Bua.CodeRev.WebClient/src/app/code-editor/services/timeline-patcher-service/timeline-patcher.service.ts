import { Injectable } from '@angular/core';
import { DateUtil, NgxVideoTimelineComponent } from 'ngx-video-timeline';
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
        timelineComp.mousewheelFunc = (event: any): boolean => {
            const e = window.event || event;
            const delta = Math.max(-1, Math.min(1, e.wheelDelta || -e.detail));

            const middleTime =
                timelineComp.startTimestamp + (timelineComp.hoursPerRuler * timelineComp.playBarDistanceLeft * 3600 * 1000);
            if (delta < 0) {
                timelineComp.zoom += timelineComp.zoom * 2;
                if (timelineComp.zoom >= 16) {
                    timelineComp.zoom = 16;
                }
                timelineComp.hoursPerRuler = timelineComp.zoom;
            } else if (delta > 0) {
                const newZoom = timelineComp.zoom - timelineComp.zoom / 2;
                if (timelineComp.zoom > 16 / 1024) {
                    timelineComp.zoom = newZoom;
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

        // Фикс прорисовки при смене разрешения
        const oldResize = timelineComp.onResize.bind(timelineComp);
        timelineComp.onResize = (): void => {
            oldResize();
            timelineComp.drawPalyBar();
        };


        // Фиск неправильного перемещения указателя при смене разрешения
        const oldDrawPlayBar = timelineComp.drawPalyBar.bind(timelineComp);
        timelineComp.drawPalyBar = (): void => {
            timelineComp.playBarOffsetX =  timelineComp.canvasW / 2;
            oldDrawPlayBar();
        };

        // Отключаю встроеную функцию воспроизведения, т.к. в ней нет смысла
        timelineComp.onPlayClick = (): void => {};

        // Обновление отрисовки для применения новой логики
        timelineComp.onResize();

        // timelineComp.onPlayClick();
    }
}
