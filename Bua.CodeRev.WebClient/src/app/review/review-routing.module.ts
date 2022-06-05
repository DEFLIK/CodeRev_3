import { NgModule } from '@angular/core';
import { LoadChildren, LoadChildrenCallback, RouterModule, Routes } from '@angular/router';
import { CandidateGradeComponent } from './components/candidate-grade/candidate-grade.component';
import { ReviewComponent } from './review.component';

const routes: Routes = [
    {
        path: '',
        component: ReviewComponent
    },
    {
        path: 'grade/:solutionId',
        component: CandidateGradeComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ReviewRoutingModule { }
