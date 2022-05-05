import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { TaskComponent } from './components/task/task.component';
import { TasksListComponent } from './components/tasks-list/tasks-list.component';
import { NotificationComponent } from './components/notification/notification.component';
import { ContestComponent } from './contest.component';
import { CodeEditorModule } from '../code-editor/code-editor.module';
import { CodeEditorComponent } from '../code-editor/code-editor.component';

const routes: Routes = [
    {
        path: '',
        component: ContestComponent
    },
];

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
        RouterModule.forChild(routes)
    ]
})
export class ContestModule { }
