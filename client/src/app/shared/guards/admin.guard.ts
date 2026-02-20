import { Injectable } from '@angular/core';
import { Router, UrlTree, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard {

  constructor(private auth: AuthService, private router: Router) { }

  canActivate():
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    return this.haveAccess();
  }

  canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    return this.haveAccess();
  }

  haveAccess() {
    let activeUser = this.auth.getUserInformation()
    if (this.auth.isLoggedIn() && activeUser && activeUser.role === 'Admin') {
      return true;
    }
    this.router.navigateByUrl('/authentication/login');
    return false;
  }

}
