import { NgModule } from '@angular/core';
import { LoadChildren, LoadChildrenCallback, RouterModule, Routes } from '@angular/router';

const routes: Routes = [
    {
        path: '',
        redirectTo: 'editor',
        pathMatch: 'full'
    },
    {
        path: 'editor',
        loadChildren: () => import('./code-editor/code-editor.module').then((m: any) => m.CodeEditorModule)
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
