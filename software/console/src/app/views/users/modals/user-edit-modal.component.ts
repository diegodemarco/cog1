import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BackendService } from '../../../services/backend.service';
import { RowComponent, ColComponent, TextColorDirective, CardComponent, CardHeaderComponent, 
         CardBodyComponent, FormDirective, FormLabelDirective, FormControlDirective, 
         ButtonDirective, FormSelectDirective, ButtonCloseDirective, ModalBodyComponent,
         ModalComponent, ModalFooterComponent, ModalHeaderComponent, ModalTitleDirective,
         ModalToggleDirective, PopoverDirective, ThemeDirective, TooltipDirective} from '@coreui/angular';
import { LiteralsContainerDTO, LocaleDTO, UserDTO, UserWithPasswordDTO } from '../../../api-client/data-contracts';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { AuthService } from 'src/app/services/auth.service';
import { ViewStatusService } from 'src/app/services/view-status.service';

@Component({
  selector: 'app-user-edit-modal',
  standalone: true,
  imports: [
    FormsModule, RowComponent, ColComponent, TextColorDirective, CardComponent, CardHeaderComponent,
    CardBodyComponent, ModalComponent, ModalHeaderComponent, ModalTitleDirective, ThemeDirective,
    ButtonCloseDirective, ModalBodyComponent, ModalFooterComponent, ButtonDirective, ModalToggleDirective,
    PopoverDirective, TooltipDirective, FormDirective, FormLabelDirective, FormControlDirective,
    FormSelectDirective
  ],
  templateUrl: './user-edit-modal.component.html',
  styleUrl: './user-edit-modal.component.scss'
})
export class UserEditModalComponent {

  modalTitle: string = '';
  visible: boolean = false;
  currentUser: UserDTO = {};
  userRole: number = 1;
  userPassword: string = '';
  changePassword: boolean = false;

  readonly literals: LiteralsContainerDTO;
  readonly locales: LocaleDTO[];
  readonly authService: AuthService;

  private emptyUser: UserDTO =
  {
    userId: 0,
    isAdmin: false,
    isOperator: false,
    localeCode: 'en',
    userName: ''
  };

  private promiseResolve?: () => void;
  private promiseReject?: () => void;
  private originalLocaleCode: string = '';

  @HostListener('window:keydown.escape') keyEvent() {
    if (this.visible) this.dismiss();
  }

  constructor(private backend: BackendService, private viewState: ViewStatusService,
    authService: AuthService, basicEntitiesService: BasicEntitiesService) {
    this.literals = basicEntitiesService.literals;
    this.locales = basicEntitiesService.locales;
    this.authService = authService;
    Object.assign(this.currentUser!, this.emptyUser);
  }

  showModal(u: UserDTO | null): Promise<void>
  {
    this.userRole = 1;
    this.userPassword = '';
    this.changePassword = false;
    if (u) {
      this.modalTitle = this.literals.security!.editUser!;
      this.originalLocaleCode = u.localeCode!;
      Object.assign(this.currentUser, u);
      if (this.currentUser.isAdmin) this.userRole = 3;
        else if (this.currentUser.isOperator) this.userRole = 2;
    }
    else {
      this.modalTitle = this.literals.security!.newUser!;
      Object.assign(this.currentUser, this.emptyUser);
    }
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
    // Update role
    this.currentUser.isAdmin = (this.userRole == 3)
    this.currentUser.isOperator = (this.userRole >= 2)
    let updatedUser: UserWithPasswordDTO = {
      user: this.currentUser,
      password: this.userPassword
    };

    if (this.currentUser.userId) {
      if (!this.changePassword) updatedUser.password = null;
      this.backend.security.editUser(updatedUser)
        .then(() => {
          this.visible = false;
          if (this.currentUser.userId == this.authService.userId 
            && this.currentUser.localeCode != this.originalLocaleCode) {
              location.reload();
          }
          else {
            if (this.promiseResolve)
              {
                this.promiseResolve();
                this.promiseResolve = undefined;
              }
            }
        })
        .catch(error => {
          this.viewState.showErrorToast(error.error.Message);
        });
    }
    else {
      this.backend.security.createUser(updatedUser)
        .then(() => {
          this.visible = false;
          if (this.promiseResolve)
          {
            this.promiseResolve();
            this.promiseResolve = undefined;
          }
        })
        .catch(error => {
          this.viewState.showErrorToast(error.error.Message);
        });
    }
  }

}
