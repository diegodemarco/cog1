import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BasicEntitiesService } from '../../services/basic-entities.service';
import { RowComponent, ColComponent, TextColorDirective, CardComponent, CardHeaderComponent, CardBodyComponent, FormDirective, FormLabelDirective, FormControlDirective, ButtonDirective, FormSelectDirective } from '@coreui/angular';
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
import { AuthService } from 'src/app/services/auth.service';
import { BackendService } from 'src/app/services/backend.service';
import { LiteralsContainerDTO, LocaleDTO } from 'src/app/api-client/data-contracts';

@Component({
  selector: 'app-profile-modal',
  standalone: true,
  imports: [
    FormsModule, ModalComponent, ModalHeaderComponent, ModalTitleDirective, ThemeDirective,
    ButtonCloseDirective, ModalBodyComponent, ModalFooterComponent, ButtonDirective,
    FormDirective, FormControlDirective, FormSelectDirective
],
  templateUrl: './profile-modal.component.html',
  styleUrl: './profile-modal.component.scss'
})
export class ProfileModalComponent {

  visible: boolean = false;
  localeCode: string = "";
  readonly literals: LiteralsContainerDTO;
  readonly locales: LocaleDTO[];
  readonly authService: AuthService;

  @HostListener('window:keydown.escape')
  keyEvent() {
    if (this.visible) this.dismiss();
  }

  constructor(private backend: BackendService, private basicEntitiesService: BasicEntitiesService, authService: AuthService) {
    this.literals = basicEntitiesService.literals;
    this.locales = basicEntitiesService.locales;
    this.authService = authService;
  }

  show()
  {
    this.localeCode = this.authService.localeCode;
    this.visible = true;
  }

  dismiss()
  {
    this.visible = false;
  }

  public saveChanges()
  {
    this.backend.security.updateUserProfile({ localeCode: this.localeCode })
      .then(() =>
      {
        return this.authService.reloadAccessTokenInfo();
      }).then(() =>
      {
        return this.basicEntitiesService.load();
      })
      .then(() =>
      {
        this.dismiss();
        location.reload();
      });
  }

}
