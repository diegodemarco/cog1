import { inject, Injectable } from '@angular/core';
import { Security } from '../api-client/Security'
import { Variables } from '../api-client/Variables'
import { Literals } from '../api-client/Literals'
import { SystemStats } from '../api-client/SystemStats'
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})
export class BackendService {

  security: Security = new Security();;
  variables: Variables = new Variables();
  systemStats: SystemStats = new  SystemStats();
  literals: Literals = new Literals();

  constructor (private config: ConfigService) {}

  configure()
  {
      this.security.baseUrl = this.config.apiBasePath;
      this.variables.baseUrl = this.config.apiBasePath;
      this.systemStats.baseUrl = this.config.apiBasePath;
      this.literals.baseUrl = this.config.apiBasePath;
  }

}
