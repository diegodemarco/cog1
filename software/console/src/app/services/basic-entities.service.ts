import { Injectable } from '@angular/core';
import { BackendService } from './backend.service';
import { LiteralsContainerDTO, LocaleDTO, LogCategoryDTO, LogLevelDTO, ModbusDataType, ModbusDataTypeDTO, ModbusRegisterDTO, ModbusRegisterType, ModbusRegisterTypeDTO, VariableAccessType, VariableAccessTypeDTO, VariableSourceDTO, VariableType, VariableTypeDTO, IntegrationConnectionType, IntegrationConnectionTypeDTO } from '../api-client/data-contracts';

@Injectable({
  providedIn: 'root'
})
export class BasicEntitiesService {

  public literals!: LiteralsContainerDTO;
  public locales!: LocaleDTO[];
  public variableAccessTypes!: VariableAccessTypeDTO[];
  public variableTypes!: VariableTypeDTO[];
  public variableSources!: VariableSourceDTO[];
  public modbusRegisterTypes!: ModbusRegisterTypeDTO[];
  public modbusDataTypes!: ModbusDataTypeDTO[];
  public integrationConnectionTypes!: IntegrationConnectionTypeDTO[];
  public logCategories!: LogCategoryDTO[];
  public logLevels!: LogLevelDTO[];

  constructor(private backend: BackendService) { }

  load() :Promise<any>  
  {
    const promise = this.backend.entities.getBasicEntities()
      .then(data => {
        this.literals = data.data.literals!;
        this.locales = data.data.locales!;
        this.variableAccessTypes = data.data.variableAccessTypes!;
        this.variableTypes = data.data.variableTypes!;
        this.variableSources = data.data.variableSources!;
        this.modbusRegisterTypes = data.data.modbusRegisterTypes!;
        this.modbusDataTypes = data.data.modbusDataTypes!;
        this.integrationConnectionTypes = data.data.integrationConnectionTypes!;
        this.logCategories = data.data.logCategories!;
        this.logLevels = data.data.logLevels!;
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

  getVariableAccessTypeDescription(accessType: VariableAccessType): string
  {
    var d = this.variableAccessTypes.find(item => item.accessType == accessType);
    if (d)
      return d.description!;
    return "Unknown variable access type " + accessType.toString();
  }

  getLocaleDescription(localeCode: string): string
  {
    var l = this.locales.find(item => item.localeCode == localeCode);
    if (l)
      return l.description!;
    return "Unknown locale code " + localeCode;
  }

  getModbusRegisterTypeDescription(registerType: ModbusRegisterType): string
  {
    var d = this.modbusRegisterTypes.find(item => item.modbusRegisterType == registerType);
    if (d)
      return d.description!;
    return "Unknown modbus register type " + registerType.toString();
  }

  getModbusDataTypeDescription(dataType: ModbusDataType): string
  {
    var d = this.modbusDataTypes.find(item => item.modbusDataType == dataType);
    if (d)
      return d.description!;
    return "Unknown modbus data type " +dataType.toString();
  }

  getIntegrationConnectionTypeDescription(connectionType: IntegrationConnectionType): string
  {
    var d = this.integrationConnectionTypes.find(item => item.integrationConnectionType == connectionType);
    if (d)
      return d.description!;
    return "Unknown integration connection type " + connectionType.toString();
  }

}