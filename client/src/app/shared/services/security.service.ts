import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import * as CryptoJS from 'crypto-js';

@Injectable({
  providedIn: 'root'
})
export class SecurityService {

  CRYPT_SECRET_KEY = environment.CRYPT_SECRET_KEY;
  TK_SECRET_KEY = environment.TK_SECRET_KEY;


  constructor() { }

  encryptSavedData(data: string) {
    return CryptoJS.AES.encrypt(JSON.stringify(data), this.CRYPT_SECRET_KEY.trim()).toString();
  }

  decryptSavedData(encrypted: string) {
    return CryptoJS.AES.decrypt(encrypted, this.CRYPT_SECRET_KEY.trim()).toString(CryptoJS.enc.Utf8);
  }

  encrypt_TK(data: string) {
    return CryptoJS.AES.encrypt(JSON.stringify(data), this.TK_SECRET_KEY.trim()).toString();
  }

  decrypt_TK(textToDecrypt: string) {
    return JSON.parse(CryptoJS.AES.decrypt(textToDecrypt, this.TK_SECRET_KEY.trim()).toString(CryptoJS.enc.Utf8));
  }
}
