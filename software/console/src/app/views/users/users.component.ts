import { Component, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonDirective, ButtonGroupComponent, CardBodyComponent, CardComponent, CardFooterComponent,
         CardHeaderComponent, ColComponent, RowComponent } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { BasicEntitiesService } from '../../services/basic-entities.service';
import { ViewStatusService } from '../../services/view-status.service';
import { BackendService } from '../../services/backend.service';
import { JsonControllerException, LiteralsContainerDTO, UserDTO } from '../../api-client/data-contracts';
import { CrudPageComponent } from '../../shared/crud-page/crud-page.component';
import { UserEditModalComponent } from './modals/user-edit-modal.component';
import { Utils } from 'src/app/utils';
import { IconSubset } from 'src/app/icons/icon-subset';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  templateUrl: 'users.component.html',
  styleUrls: ['users.component.scss'],
  standalone: true,
  imports: [CardComponent, CardBodyComponent, RowComponent, 
    ColComponent, ButtonDirective, IconDirective, ButtonGroupComponent,
    CardFooterComponent, CardHeaderComponent, CrudPageComponent, FormsModule,
    UserEditModalComponent
    ]
})
export class DashboardComponent
{
  readonly literals: LiteralsContainerDTO;
  readonly iconSubset = IconSubset;
  readonly basicEntitiesService: BasicEntitiesService;
  readonly authService: AuthService;
  readonly newItemLiteral: string = '';
  @ViewChild(CrudPageComponent) crudPage!: CrudPageComponent;
  @ViewChild(UserEditModalComponent) editModal!: UserEditModalComponent;

  // User data
  public users: UserDTO[] = [];

  constructor(private backend: BackendService, private viewStatus: ViewStatusService,
    authService: AuthService,  basicEntitiesService: BasicEntitiesService) 
  {
    this.basicEntitiesService = basicEntitiesService;
    this.literals = basicEntitiesService.literals;
    this.authService = authService;
    viewStatus.setTitle(this.literals.security!.users!);
    if (authService.isAdmin)
      this.newItemLiteral = this.literals.security!.newUser!
  }

  doNewUser()
  {
    this.editModal.showModal(null)
      .then(() => {
        this.viewStatus.showSuccessToast(this.literals.security!.userCreated!);
        this.crudPage.searchFilter = '';
        this.doUpdateData();
      })
      .catch(() => {});
  }

  doEditUser(u: UserDTO)
  {
    this.editModal.showModal(u)
      .then(() => {
        this.viewStatus.showSuccessToast(this.literals.security!.userUpdated!);
        this.doUpdateData();
      })
      .catch(() => {});
  }

  doDeleteUser(u: UserDTO)
  {
    this.viewStatus.showWarningModal(
      this.literals.security!.deleteUser!, 
      this.literals.security!.deleteUserConfirmation?.replace('{0}', u.userName!)!)
      .then(() => {
        this.backend.security.deleteUser(u.userId!)
          .then(() =>
          {
            this.viewStatus.showSuccessToast(this.literals.security!.userDeleted!);
            this.doUpdateData();
          })
      })
      .catch(error => {
        let e: JsonControllerException = error.error;
        this.viewStatus.showErrorToast(e.message!);
      });
  }

  doUpdateData()
  {
    this.backend.security.enumerateUsers()
      .then(data =>
      {
        this.users = data.data.filter((user) => 
          Utils.matchesFilter(this.crudPage.searchFilter, user.userName));
      });
  }

}
