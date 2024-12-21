import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { RowComponent, ColComponent, TextColorDirective, CardComponent, CardHeaderComponent, CardBodyComponent, FormDirective, FormLabelDirective, FormControlDirective, ButtonDirective, FormSelectDirective } from '@coreui/angular';
import { ButtonCloseDirective, ModalBodyComponent, ModalComponent, ModalFooterComponent,
         ModalHeaderComponent, ModalTitleDirective, ModalToggleDirective, PopoverDirective,
         ThemeDirective, TooltipDirective } from '@coreui/angular';
import { AuthService } from '../../../services/auth.service';
import { BackendService } from '../../../services/backend.service';
import { ViewStatusService } from '../../../services/view-status.service';
import { JsonControllerException, LiteralsContainerDTO } from '../../../api-client/data-contracts';

@Component({
  selector: 'app-variable-set-modal',
  standalone: true,
  imports: [
    FormsModule, ModalComponent, ModalHeaderComponent, ModalTitleDirective, ThemeDirective,
    ButtonCloseDirective, ModalBodyComponent, ModalFooterComponent, ButtonDirective,
    FormDirective, FormControlDirective
  ],
  templateUrl: './variable-set-modal.component.html',
  styleUrl: './variable-set-modal.component.scss'
})
export class VariableSetModalModalComponent {

  readonly literals: LiteralsContainerDTO;
  readonly authService: AuthService;
  private variableId!: number;
  private promiseResolve?: () => void;
  private promiseReject?: () => void;
  visible: boolean = false;
  varDescription: string = "";
  varValue: number = 0;

  @HostListener('window:keydown.escape')
  keyEvent() {
    if (this.visible) this.dismiss();
  }

  constructor(private backend: BackendService, private basicEntitiesService: BasicEntitiesService, 
    authService: AuthService, private viewStatus: ViewStatusService) {
    this.literals = basicEntitiesService.literals;
    this.authService = authService;
  }

  showModal(variableId: number, description: string, value: number): Promise<void>
  {
    this.variableId = variableId;
    this.varDescription = description;
    this.varValue = value;
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
    this.backend.variables.setVariableValue(this.variableId, this.varValue)
      .then(() =>
      {
        this.visible = false;
        if (this.promiseResolve)
        {
          this.promiseResolve();
          this.promiseResolve = undefined;
        }
      })
      .catch(error =>
      {
        let e: JsonControllerException = error.error;
        this.viewStatus.showErrorToast(e.message!);
    });
  }

}
