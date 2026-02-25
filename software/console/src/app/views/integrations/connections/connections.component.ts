import { Component, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonDirective, CardBodyComponent, CardComponent, ColComponent, RowComponent } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { BackendService } from '../../../services/backend.service';
import { ViewStatusService } from '../../../services/view-status.service';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { AuthService } from '../../../services/auth.service';
import { IntegrationConnectionDTO, IntegrationConnectionType, JsonControllerException, LiteralsContainerDTO } from '../../../api-client/data-contracts';
import { CrudPageComponent } from '../../../shared/crud-page/crud-page.component';
import { ConnectionEditModalComponent } from '../modals/connection-edit-modal.component';
import { Utils } from '../../../utils';
import { IconSubset } from '../../../icons/icon-subset';

@Component({
  templateUrl: 'connections.component.html',
  styleUrls: ['connections.component.scss'],
  standalone: true,
  imports: [CardComponent, CardBodyComponent, RowComponent, ColComponent, ButtonDirective, IconDirective, FormsModule, CrudPageComponent, ConnectionEditModalComponent]
})
export class ConnectionsComponent {
  readonly literals: LiteralsContainerDTO;
  readonly authService: AuthService;
  readonly newItemLiteral: string = '';
  readonly basicEntitiesService: BasicEntitiesService;
  readonly iconSubset = IconSubset;

  public connections: IntegrationConnectionDTO[] = [];
  @ViewChild(CrudPageComponent) crudPage!: CrudPageComponent;
  @ViewChild(ConnectionEditModalComponent) editModal!: ConnectionEditModalComponent;

  constructor(private backend: BackendService, private viewStatus: ViewStatusService, basicEntitiesService: BasicEntitiesService, authService: AuthService) {
    this.basicEntitiesService = basicEntitiesService;
    this.literals = basicEntitiesService.literals;
    this.authService = authService;
    viewStatus.setTitle(this.literals.integrations?.connections!);
    if (authService.isAdmin)
      this.newItemLiteral = this.literals.integrations?.newConnection!;
  }

  doUpdateData() {
    this.backend.integrations.enumerateConnections()
      .then(data => {
        this.connections = data.data.filter(item => Utils.matchesFilter(this.crudPage?.searchFilter || '', item.description, item.integrationConnectionId?.toString()));
      });
  }

  doNew() {
    this.editModal.showModal(null)
      .then(() => {
        this.viewStatus.showSuccessToast(this.literals.integrations!.connectionCreated!);
        this.crudPage.searchFilter = '';
        this.doUpdateData();
      })
      .catch(() => {});
  }

  doEdit(c: IntegrationConnectionDTO) {
    this.editModal.showModal(c)
      .then(() => {
        this.viewStatus.showSuccessToast(this.literals.integrations!.connectionUpdated!);
        this.doUpdateData();
      })
      .catch(() => {});
  }

  doDelete(c: IntegrationConnectionDTO) {
    this.viewStatus.showWarningModal(
      this.literals.integrations!.deleteConnection!, 
      this.literals.integrations!.deleteConnectionConfirmation?.replace('{0}', c.description!)!)
      .then(() => {
        this.backend.integrations.deleteConnection(c.integrationConnectionId!)
          .then(() =>
          {
            this.viewStatus.showSuccessToast(this.literals.integrations!.connectionDeleted!);
            this.doUpdateData();
          })
          .catch(error => {
            let e: JsonControllerException = error.error;
            this.viewStatus.showErrorToast(e.message!);
          });
      })
  }
}
