import { Injectable } from '@angular/core';
import { ToasterComponent, ToasterPlacement } from '@coreui/angular';
import { ProfileModalComponent } from '../modals/profile/profile-modal.component';
import { WarningPromptModalComponent } from '../modals/warning-prompt-modal.component';
import { BaseToastComponent } from '../layout/default-layout/toast/base-toast.component';
import { BasicEntitiesService } from './basic-entities.service';
import { LiteralsContainerDTO } from '../api-client/data-contracts';
import { ProgressModalComponent } from '../modals/progress-modal.component';
import { BaseViewComponent } from '../views/base/base-view.component';


@Injectable({
  providedIn: 'root'
})
export class ViewStatusService {

  public title: string = "No title";
  public profileModal?: ProfileModalComponent = undefined;
  public warningPromptModal?: WarningPromptModalComponent = undefined;
  public progressModal?: ProgressModalComponent = undefined;
  public toaster?: ToasterComponent = undefined;
  private baseView?: BaseViewComponent = undefined;
  private baseLayoutReady: boolean = false;

  private literals: LiteralsContainerDTO;

  constructor (private basicEntitiesService: BasicEntitiesService)
  {
    this.literals = basicEntitiesService.literals;
  }

  public setTitle(title: string)
  {
    this.title = title;
  }

  public setDefaultLayoutReady() {
    this.baseLayoutReady = true;
    if (this.baseView)
      this.baseView!.onViewReady();
  }

  public notifyChildViewReady(view: BaseViewComponent) {
    this.baseView = view;
    if (this.baseLayoutReady)
      view.onViewReady();
  }

  public setProfileModal(c: ProfileModalComponent) {
    this.profileModal = c;
  }

  public setWarningPromptModal(c: WarningPromptModalComponent) {
    this.warningPromptModal = c;
  }

  public setProgressModal(c: ProgressModalComponent) {
    this.progressModal = c;
  }

  public setToaster(c: ToasterComponent) {
    this.toaster = c;
  }

  public showProfileModal() {
    if (this.profileModal)
      this.profileModal.show();
  }

  public showWarningModal(title: string, message: string): Promise<void> {
    if (this.warningPromptModal) {
      return this.warningPromptModal.show(title, message);
    }
    else {
      return new Promise((resolve, reject) => resolve());
    }
  }

  public showProgressModal(title: string, message: string): void {
    if (this.progressModal)
      this.progressModal.show(title, message);
  }

  public hideProgressModal() {
    if (this.progressModal)
      this.progressModal.dismiss();
  }

  public hideModals() {
    if (this.warningPromptModal)
      this.warningPromptModal.dismiss();
    this.hideProgressModal();
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
