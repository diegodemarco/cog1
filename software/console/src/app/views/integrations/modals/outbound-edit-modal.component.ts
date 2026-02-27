import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalComponent, ModalHeaderComponent, ModalTitleDirective, ButtonCloseDirective,
  ModalBodyComponent, ModalFooterComponent, ButtonDirective, FormDirective, FormControlDirective,
  FormLabelDirective, FormSelectDirective,
  TabDirective, TabsComponent, TabsListComponent, TabsContentComponent, TabPanelComponent } from '@coreui/angular';
import { LiteralsContainerDTO, OutboundIntegrationDTO, IntegrationConnectionDTO, IntegrationConnectionType, VariableDTO, JsonControllerException } from '../../../api-client/data-contracts';
import { BackendService } from '../../../services/backend.service';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { ViewStatusService } from '../../../services/view-status.service';
import { IconDirective } from '@coreui/icons-angular';
import { IconSubset } from '../../../icons/icon-subset';

@Component({
  selector: 'app-outbound-edit-modal',
  standalone: true,
  imports: [FormsModule, ModalComponent, ModalHeaderComponent, ModalTitleDirective, ButtonCloseDirective,
    ModalBodyComponent, ModalFooterComponent, ButtonDirective, FormDirective, FormControlDirective,
    FormLabelDirective, FormSelectDirective, IconDirective,
    TabDirective, TabsComponent, TabsListComponent, TabsContentComponent, TabPanelComponent],
  templateUrl: './outbound-edit-modal.component.html',
  styleUrl: './outbound-edit-modal.component.scss'
})
export class OutboundEditModalComponent {

  readonly connectionTypeEnum = IntegrationConnectionType;
  readonly iconSubset = IconSubset;
  readonly literals: LiteralsContainerDTO;
  activeTab: string = 'general';
  visible: boolean = false;
  modalTitle: string = '';
  current: OutboundIntegrationDTO = {} as OutboundIntegrationDTO;
  connections: IntegrationConnectionDTO[] = [];
  variables: VariableDTO[] = [];
  selectedVariableId: number = 0;

  get availableVariables(): VariableDTO[] {
    const selected = this.current.variableChangeList || [];
    return this.variables.filter(v => !selected.includes(v.variableId!));
  }

  get selectedVariables(): VariableDTO[] {
    const selected = this.current.variableChangeList || [];
    return selected.map(id => this.variables.find(v => v.variableId === id)).filter(v => !!v) as VariableDTO[];
  }

  private empty: OutboundIntegrationDTO = {
    integrationId: 0,
    description: '',
    integrationConnectionId: 0,
    httpUrl: '',
    mqttTopic: '',
    sendIntervalSeconds: 60,
    variableChangeList: [],
    reportBufferingMinutes: 1440,
    reportTemplate: ''
  };

  private resolve?: () => void;
  private reject?: () => void;

  @HostListener('window:keydown.escape')
  keyEvent() { if (this.visible) this.dismiss(); }

  constructor(private backend: BackendService, private basicEntitiesService: BasicEntitiesService, private viewStatus: ViewStatusService) {
    this.literals = basicEntitiesService.literals;
    Object.assign(this.current, this.empty);
  }

  get selectedConnection(): IntegrationConnectionDTO | undefined {
    return this.connections.find(c => c.integrationConnectionId === this.current.integrationConnectionId);
  }

  get selectedConnectionType(): IntegrationConnectionType | undefined {
    return this.selectedConnection?.connectionType;
  }

  get fullHttpUrl(): string {
    const base = (this.selectedConnection?.httpBaseUrl || '').replace(/\/+$/, '');
    const path = (this.current.httpUrl || '').replace(/^\/+/, '');
    if (!base && !path) return '';
    if (!base) return path;
    if (!path) return base;
    return base + '/' + path;
  }

  get fullMqttTopic(): string {
    const base = (this.selectedConnection?.mqttBaseTopic || '').replace(/\/+$/, '');
    const sub = (this.current.mqttTopic || '').replace(/^\/+/, '');
    if (!base && !sub) return '';
    if (!base) return sub;
    if (!sub) return base;
    return base + '/' + sub;
  }

  addVariable() {
    if (!this.selectedVariableId) return;
    if (!this.current.variableChangeList) this.current.variableChangeList = [];
    if (!this.current.variableChangeList.includes(this.selectedVariableId)) {
      this.current.variableChangeList.push(this.selectedVariableId);
    }
    this.selectedVariableId = 0;
  }

  removeVariable(variableId: number) {
    if (!this.current.variableChangeList) return;
    const idx = this.current.variableChangeList.indexOf(variableId);
    if (idx >= 0) this.current.variableChangeList.splice(idx, 1);
  }

  showModal(item: OutboundIntegrationDTO | null): Promise<void> {
    this.backend.integrations.enumerateConnections()
      .then(data => { this.connections = data.data; });
    this.backend.variables.enumerateVariables()
      .then(data => { this.variables = data.data; });

    if (item) {
      this.modalTitle = this.literals.integrations!.editOutboundIntegration!;
      Object.assign(this.current, item);
    } else {
      this.modalTitle = this.literals.integrations!.newOutboundIntegration!;
      Object.assign(this.current, this.empty);
      this.current.description = this.literals.integrations!.newOutboundIntegration!;
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
    if (this.current.integrationId) {
      this.backend.integrations.editOutboundIntegration(this.current)
        .then(() => { this.visible = false; if (this.resolve) { this.resolve(); this.resolve = undefined; } })
        .catch(err => { const e: JsonControllerException = err.error; this.viewStatus.showErrorToast(e.message || 'Error'); });
    } else {
      this.backend.integrations.createOutboundIntegration(this.current)
        .then(() => { this.visible = false; if (this.resolve) { this.resolve(); this.resolve = undefined; } })
        .catch(err => { const e: JsonControllerException = err.error; this.viewStatus.showErrorToast(e.message || 'Error'); });
    }
  }

}
