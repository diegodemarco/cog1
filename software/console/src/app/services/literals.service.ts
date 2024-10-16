import { inject, Injectable } from '@angular/core';
import { BackendService } from './backend.service';
import type { CommonLiteralsContainer } from '../api-client/data-contracts';

@Injectable({
  providedIn: 'root'
})
export class LiteralsService {

  public common!: CommonLiteralsContainer;

  constructor(private backend: BackendService) { }

  load() :Promise<any>  
  {
    const promise = this.backend.literals.GetLiterals()
      .then(data => {
        this.common = data.data.common!;
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
