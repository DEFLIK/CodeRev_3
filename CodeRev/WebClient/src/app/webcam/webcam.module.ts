/* eslint-disable object-curly-spacing */
/* eslint-disable quotes */
import { NgModule } from '@angular/core';
import {SignalrComponent} from "./components/signalr.component";
import {FormsModule} from "@angular/forms";
import {WebcamRoutingModule} from "./webcam-routing.module";
import { CommonModule } from '@angular/common';

@NgModule({
    declarations: [
        SignalrComponent
    ],
    imports: [
        WebcamRoutingModule,
        FormsModule,
        CommonModule
    ],
    exports: [
        SignalrComponent
    ]
})
export class WebcamModule { }
