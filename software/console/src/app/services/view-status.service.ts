import { Injectable } from '@angular/core';
import { ToasterComponent, ToasterPlacement } from '@coreui/angular';
import { ProfileModalComponent } from '../modals/profile/profile-modal.component';
import { WarningPromptModalComponent } from '../modals/profile/warning-prompt-modal.component';
import { BaseToastComponent } from '../layout/default-layout/toast/base-toast.component';
import { BasicEntitiesService } from './basic-entities.service';
import { LiteralsContainerDTO } from '../api-client/data-contracts';


@Injectable({
  providedIn: 'root'
})
export class ViewStatusService {

  public title: string = "No title";
  public profileModal?: ProfileModalComponent = undefined;
  public warningPromptModal?: WarningPromptModalComponent = undefined;
  public toaster?: ToasterComponent = undefined;

  private literals: LiteralsContainerDTO;

  constructor (private basicEntitiesService: BasicEntitiesService)
  {
    this.literals = basicEntitiesService.literals;
  }

  public setTitle(title: string)
  {
    this.title = title;
  }

  public setProfileModal(c: ProfileModalComponent) {
    this.profileModal = c;
  }

  public setWarningPromptModal(c: WarningPromptModalComponent) {
    this.warningPromptModal = c;
  }

  public setToaster(c: ToasterComponent) {
    this.toaster = c;
  }

  public showProfileModal() {
    if (this.profileModal)
      this.profileModal.show();
  }

  public showWarningModal(title: string, message: string): Promise<void> {
    if (this.warningPromptModal){
      return this.warningPromptModal.show(title, message);
    }
    else {
      return new Promise((resolve, reject) => resolve());
    }
  }

  private showToast(message: string, title: string, color: string)
  {
    if (!this.toaster)
      return;
    const options = {
      title: title,
      message: message,
      delay: 5000,
      placement: ToasterPlacement.TopEnd,
      color: color,
      autohide: true
    };
    this.toaster.addToast(BaseToastComponent, { ...options });
  }

  public showErrorToast(message: string, title?: string | null)
  {
    this.showToast(message, title ?? this.literals.common!.error!, "danger");
  }

  public showSuccessToast(message: string, title?: string | null)
  {
    this.showToast(message, title ?? this.literals.common!.success!, "success");
  }

}
