import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { TaskComponent } from './components/task/task.component';
import { TasksListComponent } from './components/tasks-list/tasks-list.component';
import { NotificationComponent } from './components/notification/notification.component';

const routes: Routes = [
    {
        // path: '',
        // component:
    }
];

@NgModule({
    declarations: [
        TaskComponent,
        TasksListComponent,
        NotificationComponent
    ],
    imports: [
        CommonModule,
        RouterModule.forChild(routes)
    ]
})
export class ContestModule { }
