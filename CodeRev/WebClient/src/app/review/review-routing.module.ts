import { NgModule } from '@angular/core';
import { LoadChildren, LoadChildrenCallback, RouterModule, Routes } from '@angular/router';
import { CandidateGradeComponent } from './components/candidate-grade/candidate-grade.component';
import { InterviewCreateComponent } from './components/interview-create/interview-create.component';
import { ReviewComponent } from './review.component';

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
        path: 'create/interview',
        component: InterviewCreateComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ReviewRoutingModule { }
