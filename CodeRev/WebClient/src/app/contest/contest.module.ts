import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { TaskComponent } from './components/task/task.component';
import { TasksListComponent } from './components/tasks-list/tasks-list.component';
import { NotificationComponent } from './components/notification/notification.component';
import { ContestComponent } from './contest.component';
import { CodeEditorModule } from '../code-editor/code-editor.module';
import { CodeEditorComponent } from '../code-editor/code-editor.component';
import { EditorMode } from '../code-editor/models/editorMode';
import { ContestRoutingModule } from './contest-routing.module';
import { SavingService } from '../code-editor/services/saving-service/saving.service';

@NgModule({
    declarations: [
        TaskComponent,
        TasksListComponent,
        NotificationComponent,
        ContestComponent
    ],
    imports: [
        CodeEditorModule,
        CommonModule,
        ContestRoutingModule
    ],
    exports: [
        ContestComponent
    ],
    providers: [
        SavingService
    ]
})
export class ContestModule { }
