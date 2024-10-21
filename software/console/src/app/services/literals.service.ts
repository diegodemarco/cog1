import { Injectable } from '@angular/core';
import { BackendService } from './backend.service';
import type { CommonLiteralsContainer, DashboardLiteralsContainer, SecurityLiteralsContainer, VariablesLiteralsContainer } from '../api-client/data-contracts';

@Injectable({
  providedIn: 'root'
})
export class LiteralsService {

  public common!: CommonLiteralsContainer;
  public dashboard!: DashboardLiteralsContainer;
  public security!: SecurityLiteralsContainer;
  public variables!: VariablesLiteralsContainer;

  constructor(private backend: BackendService) { }

  load() :Promise<any>  
  {
    const promise = this.backend.literals.getLiterals()
      .then(data => {
        this.common = data.data.common!;
        this.dashboard = data.data.dashboard!;
        this.security = data.data.security!;
        this.variables = data.data.variables!;
        console.log("Loaded literals: ", this.common);
      })
      .catch(error =>
      {
        // Nothing to do
        console.log("Error loading literals!");
      });
    return promise;
  }

}
