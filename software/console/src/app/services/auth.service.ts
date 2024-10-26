import { Injectable } from '@angular/core';
import { BackendService } from './backend.service';
import { HttpResponse } from '@angular/common/http';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService 
{
  private accessTokenKey: string = "access-key";

  public accessToken : string | null = null;
  public isOperator: boolean = false;
  public isAdmin: boolean = false;
  public userName: string = '';
  public userId: number = 0;
  public localeCode: string = '';

  constructor(private backend: BackendService, private router: Router) { }

  public storeCredentials(token: string, persist: boolean)
  {
    this.logout();

    if (persist) {
      localStorage.setItem(this.accessTokenKey, token);
    }
    else {
      sessionStorage.setItem(this.accessTokenKey, token);
    }
    this.accessToken = token;

    // Update the access token in the backend
    this.backend.updateCredentials(this.accessToken);
  }

  public logout()
  {
    localStorage.removeItem(this.accessTokenKey)
    sessionStorage.removeItem(this.accessTokenKey)
    this.accessToken = '';
    this.isOperator = false;
    this.isAdmin = false;
    this.userName = '';
    this.userId = 0;
    this.localeCode = '';

    // Update the access token in the backend
    this.backend.updateCredentials('');
  }

  public isAuthenticated(): boolean
  {
    if (this.accessToken == null) {
      this.accessToken = this.readAccessToken();
      if (this.accessToken == null)
        this.accessToken = '';
    }
    const result = (this.accessToken.length > 0);
    return result;
  }

  private readAccessToken(): string | null
  {
    let result = localStorage.getItem(this.accessTokenKey);
    if (result != null)
      return result;
    return sessionStorage.getItem(this.accessTokenKey);
  }

  public reloadAccessTokenInfo(): Promise<any>  
  {
    if (this.isAuthenticated())
    {
      let token = this.readAccessToken();
      if (token == null) this.backend.updateCredentials('');
        else this.backend.updateCredentials(token!);
      const promise = this.backend.security.getAccessTokenInfo()
      .then(data => {
        this.isOperator = data.data.user!.isOperator!;
        this.isAdmin = data.data.user!.isAdmin!;
        this.userName = data.data.user!.userName!;
        this.userId = data.data.user!.userId!;
        this.localeCode = data.data.user!.localeCode!;
      })
      .catch(error =>
      {
        // If this a 401 error, we should not move forward
        if (error.status == 401)
        {
          this.logout();
          this.router.navigate(['/login'])
        }
      });
      return promise;
    }
    else 
    {
      return new Promise((resolve, reject) => resolve(null));
    }
  }

}
