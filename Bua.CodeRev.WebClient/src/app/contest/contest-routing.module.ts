import { NgModule } from '@angular/core';
import { LoadChildren, LoadChildrenCallback, RouterModule, Routes } from '@angular/router';
import { ContestComponent } from './contest.component';

const routes: Routes = [
    {
        path: '',
        component: ContestComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class ContestRoutingModule { }
