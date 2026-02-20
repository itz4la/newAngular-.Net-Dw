import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '',
    pathMatch: 'full',
  },
  {
    path: '',
    loadComponent: () => import('./layout-container/layout-container.component').then(m => m.LayoutContainerComponent),
    children: [

    ]

  }

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
