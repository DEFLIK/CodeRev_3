import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CodeEditorComponent } from './code-editor.component';
import { RouterModule, Routes } from '@angular/router';
import { CodeEditorDirective } from './directives/code-editor.directive';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CodemirrorModule } from '@ctrl/ngx-codemirror';
import { HttpService } from '../global-services/request/http.service';
import { ToolbarComponent } from './components/toolbar/toolbar.component';
import { RecordService } from './services/record-service/record.service';
import { ControlsComponent } from './components/controls/controls.component';
import { NgxVideoTimelineComponent, NgxVideoTimelineModule } from 'ngx-video-timeline';
import { PatchedTimelineComponent } from './components/patched-timeline/patched-timeline.component';

@NgModule({
    declarations: [
        CodeEditorDirective,
        ToolbarComponent,
        ControlsComponent,
        CodeEditorComponent,
        PatchedTimelineComponent
    ],
    imports: [
        NgxVideoTimelineModule,
        ReactiveFormsModule,
        FormsModule,
        CodemirrorModule,
        CommonModule
    ],
    exports: [
        CodeEditorComponent
    ]
})

export class CodeEditorModule { }
