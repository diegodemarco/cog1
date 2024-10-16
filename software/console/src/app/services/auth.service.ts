import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private authenticated: boolean | null = null;
  private accessTokenKey: string = "access-key";

  constructor() { }

  public storeCredentials(token: string, persist: boolean)
  {
    this.logout();

    if (persist) {
      localStorage.setItem(this.accessTokenKey, token);
    }
    else {
      sessionStorage.setItem(this.accessTokenKey, token);
    }
    this.authenticated = true;
  }

  public logout()
  {
    localStorage.removeItem(this.accessTokenKey)
    sessionStorage.removeItem(this.accessTokenKey)
    this.authenticated = false;
  }

  public isAuthenticated(): boolean
  {
    if (this.authenticated == null)
      this.authenticated = (this.readCredentials() != null);
    return this.authenticated;
  }

  public readCredentials(): string | null
  {
    let result = localStorage.getItem(this.accessTokenKey);
    if (result != null)
      return result;
    return sessionStorage.getItem(this.accessTokenKey);
  }

}
