import { Component, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LiteralsService } from '../../services/literals.service';
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

@Component({
  selector: 'app-profile-modal',
  standalone: true,
  imports: [
    FormsModule, RowComponent, ColComponent, TextColorDirective, CardComponent, CardHeaderComponent,
    CardBodyComponent, ModalComponent, ModalHeaderComponent, ModalTitleDirective, ThemeDirective,
    ButtonCloseDirective, ModalBodyComponent, ModalFooterComponent, ButtonDirective, ModalToggleDirective,
    PopoverDirective, TooltipDirective, FormDirective, FormLabelDirective, FormControlDirective, FormSelectDirective
  ],
  templateUrl: './profile-modal.component.html',
  styleUrl: './profile-modal.component.scss'
})
export class ProfileModalComponent {

  visible: boolean = false;
  localeCode: string = "";
  readonly literals: LiteralsService;
  readonly authService: AuthService;

  @HostListener('window:keydown.escape')
  keyEvent() {
    if (this.visible) this.dismiss();
  }

  constructor(private backend: BackendService, literals: LiteralsService, authService: AuthService) {
    this.literals = literals;
    this.authService = authService;
  }

  showModal()
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
    this.backend.users.updateUserProfile({ localeCode: this.localeCode })
      .then(() =>
      {
        return this.authService.reloadAccessTokenInfo();
      }).then(() =>
      {
        return this.literals.load();
      })
      .then(() =>
      {
        this.dismiss();
      });
  }

}
