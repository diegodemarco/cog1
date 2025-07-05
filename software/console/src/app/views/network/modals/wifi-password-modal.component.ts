import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FormDirective, FormControlDirective,
         ButtonDirective, FormSelectDirective, ButtonCloseDirective, ModalBodyComponent,
         ModalComponent, ModalFooterComponent, ModalHeaderComponent, ModalTitleDirective,
         ThemeDirective, TabDirective, TabsComponent, TabsListComponent, TabsContentComponent,
         TabPanelComponent} from '@coreui/angular';
import { LiteralsContainerDTO } from '../../../api-client/data-contracts';
import { BasicEntitiesService } from '../../../services/basic-entities.service';

@Component({
  selector: 'app-wifi-password-modal',
  standalone: true,
  imports: [
    FormsModule, ModalComponent, ModalHeaderComponent, ModalTitleDirective, ThemeDirective,
    ButtonCloseDirective, ModalBodyComponent, ModalFooterComponent, ButtonDirective,
    FormDirective, FormControlDirective, FormSelectDirective, TabDirective, TabsComponent, 
    TabsListComponent, TabsContentComponent, TabPanelComponent
  ],
  templateUrl: './wifi-password-modal.component.html',
  styleUrl: './wifi-password-modal.component.scss'
})
export class WiFiPasswordModalComponent {

  // Template data
  readonly literals: LiteralsContainerDTO;
  visible: boolean = false;
  password: string = '';

  private promiseResolve?: (p: string) => void;
  private promiseReject?: () => void;

  @HostListener('window:keydown.escape')
  keyEvent() {
    if (this.visible) this.dismiss();
  }

  constructor(basicEntitiesService: BasicEntitiesService) {
    this.literals = basicEntitiesService.literals;
  }

  showModal(): Promise<string>
  {
    this.password = '';
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
    this.visible = false;
    this.promiseResolve!(this.password);
    this.promiseResolve = undefined;
  }

}
