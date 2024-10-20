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
  public isAdmin: boolean = false;
  public userName: string = '';

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
    this.isAdmin = false;
    this.userName = '';

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
    console.log("isAuthenticated(): ", result);
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
        this.isAdmin = data.data.user!.isAdmin!;
        this.userName = data.data.user!.userName!;
        console.log("Loaded access token info: userName: ", this.userName, " isAdmin: ", this.isAdmin);
      })
      .catch(error =>
      {
        console.log("Error loading access token information: status: ", error.status, "details: ", error);
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
