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
import { DraftComponent } from './components/draft/draft.component';
import { MatCheckboxModule } from '@angular/material/checkbox'; 
import { FormsModule } from '@angular/forms';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { InfoComponent } from './components/info/info.component';

@NgModule({
    declarations: [
        TaskComponent,
        TasksListComponent,
        NotificationComponent,
        ContestComponent,
        DraftComponent,
        InfoComponent
    ],
    imports: [
        CodeEditorModule,
        CommonModule,
        ContestRoutingModule,
        MatCheckboxModule,
        FormsModule,
        DragDropModule
    ],
    exports: [
        ContestComponent
    ],
    providers: [
        SavingService
    ]
})
export class ContestModule { }
