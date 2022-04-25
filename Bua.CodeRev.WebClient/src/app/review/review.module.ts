import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CandidateCardComponent } from './components/candidate-card/candidate-card.component';
import { CandidatesListComponent } from './components/candidates-list/candidates-list.component';



@NgModule({
    declarations: [
        CandidateCardComponent,
        CandidatesListComponent
    ],
    imports: [
        CommonModule
    ]
})
export class ReviewModule { }
