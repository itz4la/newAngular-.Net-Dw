import { Routes } from '@angular/router';
import { PublicGuard } from './shared/guards/public.guard';
import { AdminGuard } from './shared/guards/admin.guard';
import { UserGuard } from './shared/guards/user.guards';

export const routes: Routes = [

  {
    path: '',
    redirectTo: 'authentication',
    pathMatch: 'full',
  },
  {
    path: 'authentication',
    loadChildren: () => import('./authentication/authentication.routes').then(m => m.AuthenticationRoutingModule),
    canActivate: [PublicGuard]
  },
  {
    path: 'client',
    loadChildren: () => import('./client/client.routes').then(m => m.ClientRoutingModule),
    canActivate: [UserGuard]
  },
  {
    path: 'admin',
    loadChildren: () => import('./admin/admin.routes').then(m => m.AdminRoutingModule),
    canActivate: [AdminGuard]
  },
];
