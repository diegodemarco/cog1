import { Injectable } from '@angular/core';
import { BackendService } from './backend.service';
import type { CommonLiteralsContainer, DashboardLiteralsContainer, LiteralsContainerDTO, LocaleDTO, SecurityLiteralsContainer, VariableDirectionDTO, VariablesLiteralsContainer, VariableType, VariableTypeDTO } from '../api-client/data-contracts';

@Injectable({
  providedIn: 'root'
})
export class BasicEntitiesService {

  public literals!: LiteralsContainerDTO;
  public locales!: LocaleDTO[];
  public variableDirections!: VariableDirectionDTO[];
  public variableTypes!: VariableTypeDTO[];

  constructor(private backend: BackendService) { }

  load() :Promise<any>  
  {
    const promise = this.backend.entities.getBasicEntities()
      .then(data => {
        this.literals = data.data.literals!;
        this.locales = data.data.locales!;
        this.variableDirections = data.data.variableDirections!;
        this.variableTypes = data.data.variableTypes!;
      })
      .catch(error =>
      {
        console.log("Error loading basic entities: ", error);
      });
    return promise;
  }

  getVariableTypeDescription(type: VariableType): string
  {
    var d = this.variableTypes.find(item => item.variableType == type);
    if (d)
      return d.description!;
    return "Unknown variable type " + type.toString();
  }

  getLocaleDescription(localeCode: string): string
  {
    var l = this.locales.find(item => item.localeCode == localeCode);
    if (l)
      return l.description!;
    return "Unknown locale code " + localeCode;
  }

}
