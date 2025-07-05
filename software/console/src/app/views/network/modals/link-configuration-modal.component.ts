import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FormDirective, FormControlDirective,
         ButtonDirective, FormSelectDirective, ButtonCloseDirective, ModalBodyComponent,
         ModalComponent, ModalFooterComponent, ModalHeaderComponent, ModalTitleDirective,
         ThemeDirective, TabDirective, TabsComponent, TabsListComponent, TabsContentComponent,
         TabPanelComponent,
         AlertComponent,
         RowComponent,
         ContainerComponent} from '@coreui/angular';
import { EthernetLinkConfigurationDTO, LiteralsContainerDTO } from '../../../api-client/data-contracts';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { Utils } from '../../../utils';

@Component({
  selector: 'app-link-configuration-modal',
  standalone: true,
  imports: [
    FormsModule, ModalComponent, ModalHeaderComponent, ModalTitleDirective, ThemeDirective,
    ButtonCloseDirective, ModalBodyComponent, ModalFooterComponent, ButtonDirective,
    FormDirective, FormControlDirective, FormSelectDirective, TabDirective, TabsComponent, 
    TabsListComponent, TabsContentComponent, TabPanelComponent, ContainerComponent,
    AlertComponent, RowComponent
  ],
  templateUrl: './link-configuration-modal.component.html',
  styleUrl: './link-configuration-modal.component.scss'
})
export class LinkConfigurationModalComponent {

  // Template data
  readonly literals: LiteralsContainerDTO;
  visible: boolean = false;
  currentConfig: EthernetLinkConfigurationDTO = { speed: 0 };
  alertVisible: boolean = false;
  alertMessage: string = '';

  private promiseResolve?: (c: EthernetLinkConfigurationDTO) => void;
  private promiseReject?: () => void;

  @HostListener('window:keydown.escape')
  keyEvent() {
    if (this.visible) this.dismiss();
  }

  constructor(basicEntitiesService: BasicEntitiesService) {
    this.literals = basicEntitiesService.literals;
  }

  public showModal(config: EthernetLinkConfigurationDTO): Promise<EthernetLinkConfigurationDTO>
  {
    this.alertVisible = false;
    this.alertMessage = '';

    Object.assign(this.currentConfig, config);
    if (!this.currentConfig.speed) this.currentConfig.speed = 0;

    this.visible = true;

    return new Promise((resolve, reject) => { 
      this.promiseResolve = resolve;
      this.promiseReject = reject; 
    })
  }

  public dismiss()
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
    this.promiseResolve!(this.currentConfig);
    this.promiseResolve = undefined;
  }

  private showAlert(msg: string) {

    this.alertMessage = msg;
    this.alertVisible = true;

  }

}

class LinkConfiguration {

}