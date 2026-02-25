import { Component, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonDirective, CardBodyComponent, CardComponent, ColComponent, RowComponent } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { BackendService } from '../../../services/backend.service';
import { ViewStatusService } from '../../../services/view-status.service';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { AuthService } from '../../../services/auth.service';
import { OutboundIntegrationDTO, IntegrationConnectionDTO, JsonControllerException, LiteralsContainerDTO } from '../../../api-client/data-contracts';
import { CrudPageComponent } from '../../../shared/crud-page/crud-page.component';
import { OutboundEditModalComponent } from '../modals/outbound-edit-modal.component';
import { Utils } from '../../../utils';
import { IconSubset } from '../../../icons/icon-subset';

@Component({
  templateUrl: 'outbound.component.html',
  styleUrls: ['outbound.component.scss'],
  standalone: true,
  imports: [CardComponent, CardBodyComponent, RowComponent, ColComponent, ButtonDirective,
    IconDirective, FormsModule, CrudPageComponent, OutboundEditModalComponent]
})
export class OutboundComponent {
  readonly literals: LiteralsContainerDTO;
  readonly authService: AuthService;
  readonly newItemLiteral: string = '';
  readonly basicEntitiesService: BasicEntitiesService;
  readonly iconSubset = IconSubset;

  public items: OutboundIntegrationDTO[] = [];
  private connectionsMap: Map<number, IntegrationConnectionDTO> = new Map();
  @ViewChild(CrudPageComponent) crudPage!: CrudPageComponent;
  @ViewChild(OutboundEditModalComponent) editModal!: OutboundEditModalComponent;

  constructor(private backend: BackendService, private viewStatus: ViewStatusService, basicEntitiesService: BasicEntitiesService, authService: AuthService) {
    this.basicEntitiesService = basicEntitiesService;
    this.literals = basicEntitiesService.literals;
    this.authService = authService;
    viewStatus.setTitle(this.literals.integrations?.outboundIntegrations!);
    if (authService.isAdmin)
      this.newItemLiteral = this.literals.integrations?.newOutboundIntegration!;
  }

  doUpdateData() {
    Promise.all([
      this.backend.integrations.enumerateOutboundIntegrations(),
      this.backend.integrations.enumerateConnections()
    ]).then(([outboundData, connectionsData]) => {
      this.connectionsMap = new Map(connectionsData.data.map(c => [c.integrationConnectionId!, c]));
      this.items = outboundData.data.filter(item => Utils.matchesFilter(this.crudPage?.searchFilter || '', item.description, item.integrationId?.toString()));
    });
  }

  getConnectionDescription(connectionId: number | undefined): string {
    if (!connectionId) return '';
    return this.connectionsMap.get(connectionId)?.description || connectionId.toString();
  }

  doNew() {
    this.editModal.showModal(null)
      .then(() => {
        this.viewStatus.showSuccessToast(this.literals.integrations!.outboundIntegrationCreated!);
        this.crudPage.searchFilter = '';
        this.doUpdateData();
      })
      .catch(() => {});
  }

  doEdit(i: OutboundIntegrationDTO) {
    this.editModal.showModal(i)
      .then(() => {
        this.viewStatus.showSuccessToast(this.literals.integrations!.outboundIntegrationUpdated!);
        this.doUpdateData();
      })
      .catch(() => {});
  }

  doDelete(i: OutboundIntegrationDTO) {
    this.viewStatus.showWarningModal(
      this.literals.integrations!.deleteOutboundIntegration!, 
      this.literals.integrations!.deleteOutboundIntegrationConfirmation?.replace('{0}', i.description!)!)
      .then(() => {
        this.backend.integrations.deleteOutboundIntegration(i.integrationId!)
          .then(() =>
          {
            this.viewStatus.showSuccessToast(this.literals.integrations!.outboundIntegrationDeleted!);
            this.doUpdateData();
          })
          .catch(error => {
            let e: JsonControllerException = error.error;
            this.viewStatus.showErrorToast(e.message!);
          });
      })
  }
}
