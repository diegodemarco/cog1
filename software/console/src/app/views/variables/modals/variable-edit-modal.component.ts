import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RowComponent, ColComponent, TextColorDirective, CardComponent, CardHeaderComponent,
         CardBodyComponent, FormDirective, FormLabelDirective, FormControlDirective,
         ButtonDirective, FormSelectDirective, ButtonCloseDirective, ModalBodyComponent,
         ModalComponent, ModalFooterComponent, ModalHeaderComponent, ModalTitleDirective,
         ModalToggleDirective, PopoverDirective, ThemeDirective, TooltipDirective,
         TabDirective, TabsComponent, TabsListComponent, TabsContentComponent,
         TabPanelComponent} from '@coreui/angular';
import { JsonControllerException, LiteralsContainerDTO, ModbusDataType, ModbusDataTypeDTO, ModbusRegisterType, ModbusRegisterTypeDTO,
         VariableAccessType, VariableAccessTypeDTO, VariableDTO, VariableSource, VariableSourceDTO, VariableType,
         VariableTypeDTO } from '../../../api-client/data-contracts';
import { BackendService } from '../../../services/backend.service';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { ViewStatusService } from '../../../services/view-status.service';

@Component({
  selector: 'app-variable-edit-modal',
  standalone: true,
  imports: [
    FormsModule, ModalComponent, ModalHeaderComponent, ModalTitleDirective, ThemeDirective,
    ButtonCloseDirective, ModalBodyComponent, ModalFooterComponent, ButtonDirective,
    FormDirective, FormControlDirective, FormSelectDirective, TabDirective, TabsComponent, 
    TabsListComponent, TabsContentComponent, TabPanelComponent
  ],
  templateUrl: './variable-edit-modal.component.html',
  styleUrl: './variable-edit-modal.component.scss'
})
export class VariableEditModalComponent {

  // Template data
  readonly variableSource = VariableSource;
  readonly literals: LiteralsContainerDTO;
  readonly variableAccessTypes: VariableAccessTypeDTO[];
  readonly variableTypes: VariableTypeDTO[];
  readonly modbusRegisterTypes: ModbusRegisterTypeDTO[];
  readonly modbusDataTypes: ModbusDataTypeDTO[];
  readonly slaveIds: number[];
  variableSources: VariableSourceDTO[];
  modalTitle: string = '';
  activeTab: string = "var";
  visible: boolean = false;
  currentVariable: VariableDTO = {};
  pollIntervalEnabled: boolean = false;
  accessTypeEnabled: boolean = false;

  private emptyVariable: VariableDTO =
  {
    variableId: 0,
    description: "",
    variableCode: "",
    accessType: VariableAccessType.Readonly,
    pollIntervalMs: 60000,
    source: VariableSource.External,
    type: VariableType.FloatingPoint,
    units: "",
    modbusRegister: {
      tcpHost: '',
      slaveId: 1,
      registerType: ModbusRegisterType.Coil,
      dataType: ModbusDataType.Boolean,
      registerAddress: 1
    }
  };

  private promiseResolve?: () => void;
  private promiseReject?: () => void;

  @HostListener('window:keydown.escape')
  keyEvent() {
    if (this.visible) this.dismiss();
  }

  constructor(private backend: BackendService, private basicEntitiesService: BasicEntitiesService, 
    private viewStatus: ViewStatusService
  ) {
    this.literals = basicEntitiesService.literals;
    this.variableAccessTypes = basicEntitiesService.variableAccessTypes;
    this.variableTypes = basicEntitiesService.variableTypes;
    this.modbusRegisterTypes = basicEntitiesService.modbusRegisterTypes;
    this.modbusDataTypes = basicEntitiesService.modbusDataTypes;
    this.variableSources = basicEntitiesService.variableSources;
    this.slaveIds = [];
    for (let i = 1; i <= 247; i++) this.slaveIds.push(i);
    Object.assign(this.currentVariable!, this.emptyVariable);
  }

  showModal(v: VariableDTO | null): Promise<void>
  {
    if (v) {
      this.modalTitle = this.literals.variables!.editVariable!;
      this.variableSources = this.basicEntitiesService.variableSources;
      Object.assign(this.currentVariable, v);
      if (!this.currentVariable.modbusRegister) {
        this.currentVariable.modbusRegister = {};
        Object.assign(this.currentVariable.modbusRegister, this.emptyVariable.modbusRegister);
      }
    }
    else {
      this.modalTitle = this.literals.variables!.newVariable!;
      this.variableSources = this.basicEntitiesService.variableSources
        .filter(item => item.variableSource! != VariableSource.BuiltIn);
      Object.assign(this.currentVariable, this.emptyVariable);
      this.currentVariable.modbusRegister = {};
      Object.assign(this.currentVariable.modbusRegister, this.emptyVariable.modbusRegister);
    }
    this.activeTab = "var";
    this.updateEnabledFields();
    this.visible = true;
    return new Promise((resolve, reject) => { 
      this.promiseResolve = resolve;
      this.promiseReject = reject; 
    })
  }

  dismiss()
  {
    if (this.visible) {
      this.visible = false;
      if (this.promiseReject)
      {
        this.promiseReject();
        this.promiseReject = undefined;
      }
    }
  }

  updateEnabledFields() {

    // Access type
    this.accessTypeEnabled = true;
    if (this.currentVariable.source == VariableSource.BuiltIn) {
      this.accessTypeEnabled = false;
    }
    else if (this.currentVariable.source == VariableSource.Calculated) {
      this.currentVariable.accessType = VariableAccessType.Readonly;
      this.accessTypeEnabled = false;
    }

    // Poll interval
    this.pollIntervalEnabled = true;
    if (this.currentVariable.source == VariableSource.External) {
      this.pollIntervalEnabled = false;
    }
    else if (this.currentVariable.source == VariableSource.BuiltIn) {
      this.pollIntervalEnabled = (this.currentVariable.type != VariableType.Binary)
    }
    if (!this.pollIntervalEnabled) this.currentVariable.pollIntervalMs = 0;
  }

  public saveChanges()
  {
    if (this.currentVariable.variableId) {
      this.backend.variables.editVariable(this.currentVariable)
        .then(() => {
          this.visible = false;
          if (this.promiseResolve)
          {
            this.promiseResolve();
            this.promiseResolve = undefined;
          }
        })
        .catch(error => {
          let e: JsonControllerException = error.error;
          this.viewStatus.showErrorToast(e.message!);
        });
    }
    else {
      this.backend.variables.createVariable(this.currentVariable)
        .then(() => {
          this.visible = false;
          if (this.promiseResolve)
          {
            this.promiseResolve();
            this.promiseResolve = undefined;
          }
        })
        .catch(error => {
          let e: JsonControllerException = error.error;
          this.viewStatus.showErrorToast(e.message!);
        });
    }
  }

}
