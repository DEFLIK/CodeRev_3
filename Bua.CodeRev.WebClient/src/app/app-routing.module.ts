import { NgModule } from '@angular/core';
import { LoadChildren, LoadChildrenCallback, RouterModule, Routes } from '@angular/router';

const routes: Routes = [
    {
        path: '',
        redirectTo: 'review',
        pathMatch: 'full'
    },
    {
        path: 'auth',
        loadChildren: () => import('./auth/auth.module').then((m: any) => m.AuthModule)
    },
    {
        path: 'editor',
        loadChildren: () => import('./code-editor/code-editor.module').then((m: any) => m.CodeEditorModule)
    },
    {
        path: 'contest',
        loadChildren: () => import('./contest/contest.module').then((m: any) => m.ContestModule)
    },
    {
        path: 'review',
        loadChildren: () => import('./review/review.module').then((m: any) => m.ReviewModule)
    },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
