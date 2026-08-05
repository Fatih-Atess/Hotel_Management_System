import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { Auth } from '../services/auth';

export const authGuard: CanActivateFn = () => {
    const authService = inject(Auth);
    const router = inject(Router);

    if (authService.isLoggedIn()) {
        return true;
    }

    router.navigate(['/login']);
    return false;
};

export const adminGuard: CanActivateFn = () => {
    const authService = inject(Auth);
    const router = inject(Router);

    if (authService.isLoggedIn() && authService.isAdmin()) {
        return true;
    }
    if (authService.isLoggedIn()) {
        router.navigate(['/customer']);
    } else {
        router.navigate(['/login']);
    }

    return false;
};

export const loginGuard: CanActivateFn = () => {
    const authService = inject(Auth);
    const router = inject(Router);

    if (authService.isLoggedIn()) {
        if (authService.isAdmin()) {
            router.navigate(['/admin']);
        } else {
            router.navigate(['/customer']);
        }
        return false;
    }
    return true;
};