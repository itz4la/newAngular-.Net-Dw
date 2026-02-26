import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full',
  },
  {
    path: '',
    loadComponent: () => import('./layout-container/layout-container.component').then(m => m.LayoutContainerComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent),
      },
      {
        path: 'products',
        loadComponent: () => import('./products/products.component').then(m => m.ProductsComponent),
      },
      {
        path: 'products/create',
        loadComponent: () => import('./products/product-create/product-create.component').then(m => m.ProductCreateComponent),
      },
      {
        path: 'products/edit/:id',
        loadComponent: () => import('./products/product-edit/product-edit.component').then(m => m.ProductEditComponent),
      },
      {
        path: 'categories',
        loadComponent: () => import('./categories/categories.component').then(m => m.CategoriesComponent),
      },
      {
        path: 'categories/create',
        loadComponent: () => import('./categories/category-create/category-create.component').then(m => m.CategoryCreateComponent),
      },
      {
        path: 'categories/edit/:id',
        loadComponent: () => import('./categories/category-edit/category-edit.component').then(m => m.CategoryEditComponent),
      },
      {
        path: 'orders',
        loadComponent: () => import('./orders/orders.component').then(m => m.OrdersComponent),
      },
      {
        path: 'orders/create',
        loadComponent: () => import('./orders/order-create/order-create.component').then(m => m.OrderCreateComponent),
      },
      {
        path: 'users',
        loadComponent: () => import('./users/users.component').then(m => m.UsersComponent),
      },
      {
        path: 'users/create',
        loadComponent: () => import('./users/user-create/user-create.component').then(m => m.UserCreateComponent),
      },
      {
        path: 'users/edit/:id',
        loadComponent: () => import('./users/user-edit/user-edit.component').then(m => m.UserEditComponent),
      },


    ]

  }

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
