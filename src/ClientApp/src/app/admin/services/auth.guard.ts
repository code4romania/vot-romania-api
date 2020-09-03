import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({
    providedIn: 'root',
})
export class AuthGuard implements CanActivate {
    constructor(private authService: AuthService,
                private router: Router) {

    }

    public async canActivate(): Promise<boolean> {

        if (!this.authService.getToken()) {
            this.router.navigate(['admin', 'login']);
            return false;
        }

        return true;
    }
}
