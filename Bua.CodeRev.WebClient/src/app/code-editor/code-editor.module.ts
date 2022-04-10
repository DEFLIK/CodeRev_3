import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CodeEditorComponent } from './components/code-editor/code-editor.component';
import { RouterModule, Routes } from '@angular/router';
import { CodeEditorDirective } from './directives/code-editor.directive';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { CodemirrorModule } from '@ctrl/ngx-codemirror';
import { RequestService } from '../global-services/request/request.service';
import { OutputComponent } from './components/output/output.component';

const routes: Routes = [
    {
        path: '',
        component: CodeEditorComponent
    }
];

@NgModule({
    declarations: [
        CodeEditorComponent,
        CodeEditorDirective,
        OutputComponent
    ],
    imports: [
        FormsModule,
        CodemirrorModule,
        CommonModule,
        RouterModule.forChild(routes)
    ]
})

export class CodeEditorModule { }
