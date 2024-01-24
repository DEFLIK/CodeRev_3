import { NgModule } from '@angular/core';
import { LoadChildren, LoadChildrenCallback, RouterModule, Routes } from '@angular/router';
import { CandidateGradeComponent } from './components/candidate-grade/candidate-grade.component';
import { InterviewCreateComponent } from './components/interview-create/interview-create.component';
import { ReviewComponent } from './review.component';
import { TaskCreateComponent } from './components/task-create/task-create.component';
import { ControlPanelComponent } from './components/control-panel/control-panel.component';

const routes: Routes = [
    {
        path: '',
        component: ReviewComponent
    },
    {
        path: 'grade/:solutionId/:state',
        component: CandidateGradeComponent
    },
    {
        path: 'control-panel',
        component: ControlPanelComponent
    },
    {
        path: 'create/interview',
        component: InterviewCreateComponent
    },
    {
        path: 'create/task',
        component: TaskCreateComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ReviewRoutingModule { }
