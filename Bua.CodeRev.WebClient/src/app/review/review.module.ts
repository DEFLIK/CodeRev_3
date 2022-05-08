import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CandidateCardComponent } from './components/candidate-card/candidate-card.component';
import { CandidatesListComponent } from './components/candidates-list/candidates-list.component';
import { RouterModule, Routes } from '@angular/router';
import { ReviewComponent } from './review.component';
import { CandidateFilterPipe } from './pipes/candidate-filter.pipe';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CandidateGradeComponent } from './components/candidate-grade/candidate-grade.component';

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
    declarations: [
        CandidateCardComponent,
        CandidatesListComponent,
        ReviewComponent,
        CandidateFilterPipe,
        CandidateGradeComponent
    ],
    imports: [
        CommonModule,
        RouterModule.forChild(routes),
        ReactiveFormsModule,
        FormsModule
    ]
})
export class ReviewModule { }
