import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
  {
    path: '',
    children: [
      {
        path: 'login',
        loadComponent: () => import('./login/login.component').then( m => m.LoginComponent),
      },
      {
        path: 'register',
        loadComponent: () => import('./register/register.component').then( m => m.RegisterComponent),
      },
    ]

  }

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthenticationRoutingModule { }
