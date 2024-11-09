import { Injectable } from '@angular/core';
import { Security } from '../api-client/Security'
import { Variables } from '../api-client/Variables'
import { Entities } from '../api-client/Entities'
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

  public security: Security = new Security( this.apiConfig );
  public variables: Variables = new Variables( this.apiConfig );
  public system: System = new  System( this.apiConfig );
  public entities: Entities = new Entities( this.apiConfig );

  constructor (private config: ConfigService) {}

  configure()
  {
      this.security.baseUrl = this.config.apiBasePath;
      this.variables.baseUrl = this.config.apiBasePath;
      this.system.baseUrl = this.config.apiBasePath;
      this.entities.baseUrl = this.config.apiBasePath;
  }

  public updateCredentials(accessToken: string)
  {
    if (accessToken.length > 0) {
      this.apiConfig.baseApiParams!.headers = { Authorization: "bearer " + accessToken };
    }
    else {
      this.apiConfig.baseApiParams!.headers = { };
    }
  }

}
