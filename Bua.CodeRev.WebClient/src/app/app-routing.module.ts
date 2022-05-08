import { NgModule } from '@angular/core';
import { LoadChildren, LoadChildrenCallback, RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth/guards/auth.guard';

const routes: Routes = [
    {
        path: '',
        redirectTo: 'auth',
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
        loadChildren: () => import('./contest/contest.module').then((m: any) => m.ContestModule),
        canLoad: [AuthGuard]
    },
    {
        path: 'review',
        loadChildren: () => import('./review/review.module').then((m: any) => m.ReviewModule),
        canLoad: [AuthGuard]
    },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
