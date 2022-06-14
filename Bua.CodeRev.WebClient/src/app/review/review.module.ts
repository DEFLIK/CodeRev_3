import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CandidateCardComponent } from './components/candidate-card/candidate-card.component';
import { CandidatesListComponent } from './components/candidates-list/candidates-list.component';
import { RouterModule, Routes } from '@angular/router';
import { ReviewComponent } from './review.component';
import { CandidateFilterPipe } from './pipes/candidate-filter.pipe';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CandidateGradeComponent } from './components/candidate-grade/candidate-grade.component';
import { ContestModule } from '../contest/contest.module';
import { ContestComponent } from '../contest/contest.component';
import { ReviewRoutingModule } from './review-routing.module';
import { CandidateInviteComponent } from './components/candidate-invite/candidate-invite.component';

@NgModule({
    declarations: [
        CandidateCardComponent,
        CandidatesListComponent,
        ReviewComponent,
        CandidateFilterPipe,
        CandidateGradeComponent,
        CandidateInviteComponent
    ],
    imports: [
        ReviewRoutingModule,
        CommonModule,
        ContestModule,
        ReactiveFormsModule,
        FormsModule
    ]
})
export class ReviewModule { }
