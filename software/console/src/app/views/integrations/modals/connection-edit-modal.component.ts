import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalComponent, ModalHeaderComponent, ModalTitleDirective, ButtonCloseDirective,
  ModalBodyComponent, ModalFooterComponent, ButtonDirective, FormDirective, FormControlDirective,
  FormLabelDirective, FormSelectDirective, TabDirective, TabsComponent, TabsListComponent, TabsContentComponent, TabPanelComponent } from '@coreui/angular';
import { LiteralsContainerDTO, IntegrationConnectionDTO, IntegrationConnectionType, JsonControllerException, ValuePairDTO, IntegrationConnectionTypeDTO } from '../../../api-client/data-contracts';
import { BackendService } from '../../../services/backend.service';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { ViewStatusService } from '../../../services/view-status.service';
import { IconDirective } from '@coreui/icons-angular';
import { IconSubset } from '../../../icons/icon-subset';

@Component({
  selector: 'app-connection-edit-modal',
  standalone: true,
  imports: [FormsModule, ModalComponent, ModalHeaderComponent, ModalTitleDirective, ButtonCloseDirective,
    ModalBodyComponent, ModalFooterComponent, ButtonDirective, FormDirective, FormControlDirective,
    FormLabelDirective, FormSelectDirective, TabDirective, TabsComponent, TabsListComponent, TabsContentComponent, TabPanelComponent, IconDirective],
  templateUrl: './connection-edit-modal.component.html',
  styleUrl: './connection-edit-modal.component.scss'
})
export class ConnectionEditModalComponent {

  readonly connectionTypeEnum = IntegrationConnectionType;
  readonly iconSubset = IconSubset;
  readonly literals: LiteralsContainerDTO;
  readonly connectionTypes: IntegrationConnectionTypeDTO[];
  activeTab: string = 'general';
  visible: boolean = false;
  modalTitle: string = '';
  current: IntegrationConnectionDTO = {} as IntegrationConnectionDTO;

  private empty: IntegrationConnectionDTO = {
    integrationConnectionId: 0,
    connectionType: IntegrationConnectionType.MQTT,
    description: '',
    httpBaseUrl: '',
    httpHeaders: [],
    mqttHost: '',
    mqttBaseTopic: 'data',
    mqttServerCertificate: '',
    mqttClientCertificate: '',
    userName: '',
    password: ''
  };

  private resolve?: () => void;
  private reject?: () => void;

  @HostListener('window:keydown.escape')
  keyEvent() { if (this.visible) this.dismiss(); }

  constructor(private backend: BackendService, private basicEntitiesService: BasicEntitiesService, private viewStatus: ViewStatusService) {
    this.literals = basicEntitiesService.literals;
    this.connectionTypes = basicEntitiesService.integrationConnectionTypes;
    Object.assign(this.current, this.empty);
  }

  showModal(c: IntegrationConnectionDTO | null): Promise<void> {
    if (c) {
      this.modalTitle = this.literals.integrations!.editConnection!;
      Object.assign(this.current, c);
    } else {
      this.modalTitle = this.literals.integrations!.newConnection!;
      Object.assign(this.current, this.empty);
      this.current.description = this.literals.integrations!.newConnection!;
    }
    this.activeTab = 'general';
    this.visible = true;
    return new Promise((res, rej) => { this.resolve = res; this.reject = rej; });
  }

  dismiss() {
    if (this.visible) {
      this.visible = false;
      if (this.reject) { this.reject(); this.reject = undefined; }
    }
  }

  saveChanges() {
    if (this.current.integrationConnectionId) {
      this.backend.integrations.editConnection(this.current)
        .then(() => { this.visible = false; if (this.resolve) { this.resolve(); this.resolve = undefined; } })
        .catch(err => { const e: JsonControllerException = err.error; this.viewStatus.showErrorToast(e.message || 'Error'); });
    } else {
      this.backend.integrations.createConnection(this.current)
        .then(() => { this.visible = false; if (this.resolve) { this.resolve(); this.resolve = undefined; } })
        .catch(err => { const e: JsonControllerException = err.error; this.viewStatus.showErrorToast(e.message || 'Error'); });
    }
  }

}
