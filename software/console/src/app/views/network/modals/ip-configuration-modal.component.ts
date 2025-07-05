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
import { IpConfigurationDTO, LiteralsContainerDTO } from '../../../api-client/data-contracts';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { Utils } from '../../../utils';

@Component({
  selector: 'app-ip-configuration-modal',
  standalone: true,
  imports: [
    FormsModule, ModalComponent, ModalHeaderComponent, ModalTitleDirective, ThemeDirective,
    ButtonCloseDirective, ModalBodyComponent, ModalFooterComponent, ButtonDirective,
    FormDirective, FormControlDirective, FormSelectDirective, TabDirective, TabsComponent, 
    TabsListComponent, TabsContentComponent, TabPanelComponent, ContainerComponent,
    AlertComponent, RowComponent
  ],
  templateUrl: './ip-configuration-modal.component.html',
  styleUrl: './ip-configuration-modal.component.scss'
})
export class IpConfigurationModalComponent {

  // Template data
  readonly literals: LiteralsContainerDTO;
  visible: boolean = false;
  currentConfig: IpConfigurationDTO = { dhcp: true, ipv4: '', netMask: 24, gateway: '', dns: '' };
  alertVisible: boolean = false;
  alertMessage: string = '';

  private promiseResolve?: (c: IpConfigurationDTO) => void;
  private promiseReject?: () => void;

  @HostListener('window:keydown.escape')
  keyEvent() {
    if (this.visible) this.dismiss();
  }

  constructor(basicEntitiesService: BasicEntitiesService) {
    this.literals = basicEntitiesService.literals;
  }

  public showModal(config: IpConfigurationDTO): Promise<IpConfigurationDTO>
  {
    this.alertVisible = false;
    this.alertMessage = '';

    Object.assign(this.currentConfig, config);
    if (!this.currentConfig.ipv4) this.currentConfig.ipv4 = '';
    if (!this.currentConfig.netMask) this.currentConfig.netMask = 24;
    if (!this.currentConfig.gateway) this.currentConfig.gateway = '';
    if (!this.currentConfig.dns) this.currentConfig.dns = '';

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
    if (!this.currentConfig.dhcp) {
      // Validate fixed IP data
      if (!Utils.validateIPaddress(this.currentConfig.ipv4!)) {
        this.showAlert("Invalid IP address");
        return;
      }
      if (this.currentConfig.netMask! < 1 || this.currentConfig.netMask! > 31) {
        this.showAlert("Invalid network mask");
        return;
      }
      if (!Utils.validateIPaddress(this.currentConfig.gateway!)) {
        this.showAlert("Invalid gateway");
        return;
      }
      if (this.currentConfig.dns! != '' && !Utils.validateIPaddress(this.currentConfig.dns!)) {
        this.showAlert("Invalid DNS");
        return;
      }
    }
    this.visible = false;
    this.promiseResolve!(this.currentConfig);
    this.promiseResolve = undefined;
  }

  private showAlert(msg: string) {

    this.alertMessage = msg;
    this.alertVisible = true;

  }

}

class ipConfiguration {

}