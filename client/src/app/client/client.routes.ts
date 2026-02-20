import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: '',
    loadComponent: () => import('./layout-container/layout-container.component').then(m => m.LayoutContainerComponent),
    children: [
      {
        path: 'home',
        loadComponent: () => import('./home/home.component').then(m => m.HomeComponent),
      },
      {
        path: 'browse',
        loadComponent: () => import('./browse/browse.component').then(m => m.BrowseComponent),
      },
      {
        path: 'product-details/:id',
        loadComponent: () => import('./product-details/product-details.component').then(m => m.ProductDetailsComponent),
      },
      {
        path: 'profile',
        loadComponent: () => import('./profile/profile.component').then(m => m.ProfileComponent),
      },
      {
        path: 'my-purchase',
        loadComponent: () => import('./my-purchase/my-purchase.component').then(m => m.MyPurchaseComponent),
      },
    ]

  }

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ClientRoutingModule { }
