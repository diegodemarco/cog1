import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BasicEntitiesService } from '../../services/basic-entities.service';
import { RowComponent, ColComponent, TextColorDirective, CardComponent, CardHeaderComponent, CardBodyComponent, FormDirective, FormLabelDirective, FormControlDirective, ButtonDirective, FormSelectDirective } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { 
  ButtonCloseDirective,
  ModalBodyComponent,
  ModalComponent,
  ModalFooterComponent,
  ModalHeaderComponent,
  ModalTitleDirective,
  ModalToggleDirective,
  PopoverDirective,
  ThemeDirective,
  TooltipDirective } from '@coreui/angular';
import { BackendService } from '../../services/backend.service';
import { IconSubset } from 'src/app/icons/icon-subset';
import { LiteralsContainerDTO } from 'src/app/api-client/data-contracts';

@Component({
  selector: 'app-warning-prompt-modal',
  standalone: true,
  imports: [
    FormsModule, RowComponent, ColComponent, TextColorDirective, CardComponent, CardHeaderComponent,
    CardBodyComponent, ModalComponent, ModalHeaderComponent, ModalTitleDirective, ThemeDirective,
    ButtonCloseDirective, ModalBodyComponent, ModalFooterComponent, ButtonDirective, ModalToggleDirective,
    PopoverDirective, TooltipDirective, FormDirective, FormLabelDirective, FormControlDirective, 
    FormSelectDirective, IconDirective
  ],
  templateUrl: './warning-prompt-modal.component.html',
  styleUrl: './warning-prompt-modal.component.scss'
})
export class WarningPromptModalComponent {

  visible: boolean = false;
  localeCode: string = "";
  readonly literals: LiteralsContainerDTO;
  readonly iconSubset = IconSubset;
  private promiseResolve?: () => void;
  private promiseReject?: () => void;
  modalTitle?: string = undefined;
  message?: string = undefined;
    
  @HostListener('window:keydown.escape')
  keyEvent() {
    if (this.visible) this.dismiss();
  }

  constructor(private backend: BackendService, basicEntitiesService: BasicEntitiesService) {
    this.literals = basicEntitiesService.literals;
  }

  show(title: string, message: string): Promise<void>
  {
    this.modalTitle = title;
    this.message = message;
    this.visible = true;
    return new Promise((resolve, reject) => {
      this.promiseResolve = resolve;
      this.promiseReject = reject;
    });
  }

  dismiss()
  {
    this.visible = false;
    if (this.promiseReject)
      this.promiseReject();
  }

  public accept()
  {
    if (this.promiseResolve)
      this.promiseResolve();
    this.dismiss();
  }

}
