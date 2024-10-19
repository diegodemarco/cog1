import { Injectable } from '@angular/core';
import { Security } from '../api-client/Security'
import { Variables } from '../api-client/Variables'
import { Literals } from '../api-client/Literals'
import { System } from '../api-client/System'
import { ConfigService } from './config.service';
import { ApiConfig } from '../api-client/http-client';

@Injectable({
  providedIn: 'root'
})
export class BackendService 
{
  private apiConfig: ApiConfig = 
  {
    baseApiParams: 
    { 
      credentials: "same-origin",
      headers: { },
      redirect: "follow",
      referrerPolicy: "no-referrer",
     }
  };

  security: Security = new Security( this.apiConfig );
  variables: Variables = new Variables( this.apiConfig );
  system: System = new  System( this.apiConfig );
  literals: Literals = new Literals( this.apiConfig );

  constructor (private config: ConfigService) {}

  configure()
  {
      this.security.baseUrl = this.config.apiBasePath;
      this.variables.baseUrl = this.config.apiBasePath;
      this.system.baseUrl = this.config.apiBasePath;
      this.literals.baseUrl = this.config.apiBasePath;
  }

  public updateCredentials(accessToken: string)
  {
    console.log("Updated access token to ", accessToken);
    if (accessToken.length > 0) {
      this.apiConfig.baseApiParams!.headers = { Authorization: "bearer " + accessToken };
    }
    else {
      this.apiConfig.baseApiParams!.headers = { };
    }
  }

}
