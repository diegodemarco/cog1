import { Injectable } from '@angular/core';
import { Router } from "@angular/router";
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard {

  constructor(private authService: AuthService, private router: Router) { }

  public canActivate(): boolean
  {
    if (this.authService.isAuthenticated()) {
        return true;
    } else {
        this.router.navigate(['/login']);
        return false;
    }
  }
}

@Injectable({
    providedIn: 'root'
  })
  export class SkipLoginGuard {
  
    constructor(private authService: AuthService, private router: Router) { }
  
    public canActivate(): boolean
    {
      if (this.authService.isAuthenticated()) {
        this.router.navigate(['/']);
        return false;
      } else {
        return true;
      }
    }
  }
  