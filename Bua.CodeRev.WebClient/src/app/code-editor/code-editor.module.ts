import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CodeEditorComponent } from './code-editor.component';
import { RouterModule, Routes } from '@angular/router';
import { CodeEditorDirective } from './directives/code-editor.directive';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { CodemirrorModule } from '@ctrl/ngx-codemirror';
import { HttpService } from '../global-services/request/http.service';
import { OutputComponent } from './components/output/output.component';
import { RecordService } from './services/record-service/record.service';
import { ControlsComponent } from './components/controls/controls.component';

const routes: Routes = [
    {
        path: '',
        component: CodeEditorComponent
    }
];

@NgModule({
    declarations: [
        CodeEditorDirective,
        OutputComponent,
        ControlsComponent,
        CodeEditorComponent
    ],
    imports: [
        FormsModule,
        CodemirrorModule,
        CommonModule,
        RouterModule.forChild(routes)
    ],
    exports: [
        CodeEditorComponent
    ]
})

export class CodeEditorModule { }
