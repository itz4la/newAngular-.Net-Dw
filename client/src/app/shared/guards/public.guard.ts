import { Injectable } from '@angular/core';
import { Router, UrlTree, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class PublicGuard {

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
    if (!this.auth.isLoggedIn()) {
      return true;
    }
    let activeUser = this.auth.getUserInformation()

    if (activeUser) {
      this.router.navigateByUrl('/' + activeUser.role.toLowerCase());
    }

    return false;
  }

}
