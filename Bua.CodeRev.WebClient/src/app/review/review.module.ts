import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CandidateCardComponent } from './components/candidate-card/candidate-card.component';
import { CandidatesListComponent } from './components/candidates-list/candidates-list.component';
import { RouterModule, Routes } from '@angular/router';
import { ReviewComponent } from './review.component';

const routes: Routes = [
    {
        path: '',
        component: ReviewComponent
    }
];

@NgModule({
    declarations: [
        CandidateCardComponent,
        CandidatesListComponent,
        ReviewComponent
    ],
    imports: [
        CommonModule,
        RouterModule.forChild(routes)
    ]
})
export class ReviewModule { }
