import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RowComponent, ColComponent, TextColorDirective, CardComponent, CardHeaderComponent,
         CardBodyComponent, FormDirective, FormLabelDirective, FormControlDirective,
         ButtonDirective, FormSelectDirective, ButtonCloseDirective, ModalBodyComponent,
         ModalComponent, ModalFooterComponent, ModalHeaderComponent, ModalTitleDirective,
         ModalToggleDirective, PopoverDirective, ThemeDirective, TooltipDirective } from '@coreui/angular';
import { JsonControllerException, LiteralsContainerDTO, VariableDirection, VariableDirectionDTO, VariableDTO, VariableType,
         VariableTypeDTO } from '../../../api-client/data-contracts';
import { BackendService } from '../../../services/backend.service';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { ViewStatusService } from '../../../services/view-status.service';

@Component({
  selector: 'app-variable-edit-modal',
  standalone: true,
  imports: [
    FormsModule, RowComponent, ColComponent, TextColorDirective, CardComponent, CardHeaderComponent,
    CardBodyComponent, ModalComponent, ModalHeaderComponent, ModalTitleDirective, ThemeDirective,
    ButtonCloseDirective, ModalBodyComponent, ModalFooterComponent, ButtonDirective, ModalToggleDirective,
    PopoverDirective, TooltipDirective, FormDirective, FormLabelDirective, FormControlDirective,
    FormSelectDirective
  ],
  templateUrl: './variable-edit-modal.component.html',
  styleUrl: './variable-edit-modal.component.scss'
})
export class VariableEditModalComponent {

  modalTitle: string = '';
  visible: boolean = false;

  private emptyVariable: VariableDTO =
  {
    variableId: 0,
    description: "",
    variableCode: "",
    direction: VariableDirection.Input,
    isBuiltIn: false,
    type: VariableType.FloatingPoint,
    units: ""
  };

  private promiseResolve?: () => void;
  private promiseReject?: () => void;

  currentVariable: VariableDTO = {};

  readonly literals: LiteralsContainerDTO;
  readonly variableDirections: VariableDirectionDTO[];
  readonly variableTypes: VariableTypeDTO[];

  @HostListener('window:keydown.escape')
  keyEvent() {
    if (this.visible) this.dismiss();
  }

  constructor(private backend: BackendService, basicEntitiesService: BasicEntitiesService, private viewStatus: ViewStatusService
  ) {
    this.literals = basicEntitiesService.literals;
    this.variableDirections = basicEntitiesService.variableDirections;
    this.variableTypes = basicEntitiesService.variableTypes;
    Object.assign(this.currentVariable!, this.emptyVariable);
  }

  showModal(v: VariableDTO | null): Promise<void>
  {
    if (v) {
      this.modalTitle = this.literals.variables!.editVariable!;
      Object.assign(this.currentVariable, v);
    }
    else {
      this.modalTitle = this.literals.variables!.newVariable!;
      Object.assign(this.currentVariable, this.emptyVariable);
    }
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
