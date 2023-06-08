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
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio'; 
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MeetCardComponent } from './components/meet-card/meet-card.component'; 
import { MeetsFilterPipe } from './pipes/meets-filter.pipe';
import { InterviewCreateComponent } from './components/interview-create/interview-create.component';
import { MatSelectModule } from '@angular/material/select'; 
import { MatInputModule } from '@angular/material/input';
import { TasksFilterPipe } from './pipes/tasks-filter.pipe'; 
import { CodeEditorModule } from '../code-editor/code-editor.module';
import { CodemirrorModule } from '@ctrl/ngx-codemirror';
import { TaskCreateComponent } from './components/task-create/task-create.component';

@NgModule({
    declarations: [
        CandidateCardComponent,
        CandidatesListComponent,
        ReviewComponent,
        CandidateFilterPipe,
        MeetsFilterPipe,
        CandidateGradeComponent,
        CandidateInviteComponent,
        MeetCardComponent,
        InterviewCreateComponent,
        TasksFilterPipe,
        TaskCreateComponent
    ],
    imports: [
        ReviewRoutingModule,
        CommonModule,
        ContestModule,
        ReactiveFormsModule,
        FormsModule,
        MatCheckboxModule,
        MatRadioModule,
        MatProgressBarModule,
        MatSelectModule,
        MatInputModule,
        CodemirrorModule,
    ]
})
export class ReviewModule { }
