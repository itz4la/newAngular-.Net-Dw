import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { v4 as uuidv4 } from 'uuid';
import { MD5 } from 'crypto-js';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { SecurityService } from './security.service';
import { environment } from '../../../environments/environment';
@Injectable({
  providedIn: 'root',
})
export class RequestsService {


  API_SERVER_URI = environment.API_SERVER_URI;


  constructor(private http: HttpClient, private authService: AuthService, private securityService: SecurityService) { }


  createHeader() {
    const token = this.authService.getToken();
    let uuid = uuidv4();
    if (token) {
      const headers = new HttpHeaders()
        .set('Authorization', 'Bearer ' + token)
        .set('XSRF-TOKEN', uuid)
        .set('X-XSRF-TOKEN', MD5(uuid + environment.CSRF_SECRET).toString());
      return headers;
    }
    else {
      const headers = new HttpHeaders()
        .set('XSRF-TOKEN', uuid)
        .set('X-XSRF-TOKEN', MD5(uuid + environment.CSRF_SECRET).toString());
      return headers;
    }
  }

  createSimpleHeader() {
    const headers = new HttpHeaders();
    return headers;
  }

  get(uri: string): Observable<any> {
    return this.http.get<any>(this.API_SERVER_URI + uri, { headers: this.createHeader() });
  }

  post(uri: string, data: any): Observable<any> {

    return this.http.post<any>(this.API_SERVER_URI + uri, data, { headers: this.createHeader() });
  }

  put(uri: string, data: any): Observable<any> {

    return this.http.put<any>(this.API_SERVER_URI + uri, data, { headers: this.createHeader() });
  }

  patch(uri: string, data: any): Observable<any> {

    return this.http.patch<any>(this.API_SERVER_URI + uri, data, { headers: this.createHeader() });
  }

  delete(uri: string): Observable<any> {
    return this.http.delete<any>(this.API_SERVER_URI + uri, { headers: this.createHeader() });
  }

  getFile(uri: string): Observable<Blob> {
    return this.http.get(this.API_SERVER_URI + uri, { responseType: 'blob', headers: this.createHeader() });
  }


}
