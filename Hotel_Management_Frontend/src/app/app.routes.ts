import { Routes } from '@angular/router';
import { Customer } from './pages/customer/customer';
import { Admin } from './pages/admin/admin';
import { Login } from './components/login/login';
import { authGuard, adminGuard, loginGuard } from './guards/auth.guard';

export const routes: Routes = [
    { path: 'login', component: Login, canActivate: [loginGuard] },
    { path: 'customer', component: Customer, canActivate: [authGuard] },
    { path: 'admin', component: Admin, canActivate: [adminGuard] },
    { path: '', redirectTo: '/login', pathMatch: 'full' },
    { path: '**', redirectTo: '/login' }
];
