import { Routes } from '@angular/router';
import { Customer } from './pages/customer/customer';
import { Admin } from './pages/admin/admin';

export const routes: Routes = [
    { path: 'customer', component: Customer},
    { path: 'admin', component: Admin},
    { path: '', redirectTo: '/customer', pathMatch: 'full'}
];
