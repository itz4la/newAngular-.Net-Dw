import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { SecurityService } from './security.service';



@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private _isLoggedIn: BehaviorSubject<boolean>;
  private _activeUser: BehaviorSubject<any | null>;

  authStatus: Observable<boolean>;
  activeUser: Observable<any | null>;


  constructor(private securityService: SecurityService) {
    this._isLoggedIn = new BehaviorSubject<boolean>(this.isLoggedIn());
    this._activeUser = new BehaviorSubject<any | null>(this.getUserInformation());
    this.authStatus = this._isLoggedIn.asObservable();
    this.activeUser = this._activeUser.asObservable();
  }

  getToken() {
    const jwtHelper = new JwtHelperService();
    const token = localStorage.getItem('PR_TK');
    if (!token) {
      return null;
    } else {
      if (!jwtHelper.isTokenExpired(this.securityService.decrypt_TK(token))) {
        return this.securityService.decrypt_TK(token);
      }
      else {
        this.logout();
        return null;
      }
    }
  }

  isLoggedIn() {
    const jwtHelper = new JwtHelperService();
    const token = localStorage.getItem('PR_TK');
    if (!token) {
      return false;
    } else {
      if (!jwtHelper.isTokenExpired(this.securityService.decrypt_TK(token))) {
        return true;
      }
      else {
        localStorage.removeItem('PR_TK');
        return false;
      }
    }

  }

  getUserInformation() {
    const jwtHelper = new JwtHelperService();
    const token = localStorage.getItem('PR_TK');
    if (!token) { return null; }
    if (!jwtHelper.isTokenExpired(this.securityService.decrypt_TK(token))) {
      let data = jwtHelper.decodeToken(this.securityService.decrypt_TK(token));
      const result = data;
      return result;
    } else {
      localStorage.removeItem('PR_TK');
      return null;
    }
  }

  changeAuthStatus(value: boolean) {
    this._isLoggedIn.next(value);
  }

  changeLoggedInUser() {
    const activeUser = this.getUserInformation();
    this._activeUser.next(activeUser);
  }

  signin(token: string) {
    localStorage.setItem('PR_TK', this.securityService.encrypt_TK(token));
    this.changeAuthStatus(true);
    this.changeLoggedInUser();
  }

  refreshToken(token: string) {
    localStorage.setItem('PR_TK', this.securityService.encrypt_TK(token));
    this.changeLoggedInUser();
  }

  logout() {
    localStorage.removeItem('PR_TK');
    this.changeAuthStatus(false);
  }

}
