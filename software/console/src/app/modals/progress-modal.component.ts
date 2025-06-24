import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BasicEntitiesService } from '../services/basic-entities.service';
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
import { BackendService } from '../services/backend.service';
import { IconSubset } from '../icons/icon-subset';
import { LiteralsContainerDTO } from '../api-client/data-contracts';

@Component({
  selector: 'app-progress-modal',
  standalone: true,
  imports: [
    FormsModule, RowComponent, ColComponent, TextColorDirective, CardComponent, CardHeaderComponent,
    CardBodyComponent, ModalComponent, ModalHeaderComponent, ModalTitleDirective, ThemeDirective,
    ButtonCloseDirective, ModalBodyComponent, ModalFooterComponent, ButtonDirective, ModalToggleDirective,
    PopoverDirective, TooltipDirective, FormDirective, FormLabelDirective, FormControlDirective, 
    FormSelectDirective, IconDirective
  ],
  templateUrl: './progress-modal.component.html',
  styleUrl: './progress-modal.component.scss'
})
export class ProgressModalComponent {

  visible: boolean = false;
  localeCode: string = "";
  readonly literals: LiteralsContainerDTO;
  readonly iconSubset = IconSubset;
  modalTitle?: string = undefined;
  message?: string = undefined;
    
  constructor(private backend: BackendService, basicEntitiesService: BasicEntitiesService) {
    this.literals = basicEntitiesService.literals;
  }

  show(title: string, message: string): void
  {
    this.modalTitle = title;
    this.message = message;
    this.visible = true;
  }

  dismiss()
  {
    this.visible = false;
  }

}
