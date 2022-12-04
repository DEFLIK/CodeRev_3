import { NgModule } from '@angular/core';
import { LoadChildren, LoadChildrenCallback, RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth/guards/auth.guard';
import {WebcamModule} from "./webcam/webcam.module";

const routes: Routes = [
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
        canActivate: [AuthGuard]
    },
    {
        path: 'review',
        loadChildren: () => import('./review/review.module').then((m: any) => m.ReviewModule),
        canActivate: [AuthGuard]
    },
    {
        path: 'web',
        loadChildren: () => import('./webcam/webcam.module').then((m: any)=> m.WebcamModule)
    },
    {
        path: '**',
        redirectTo: 'auth',
        pathMatch: 'full'
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
