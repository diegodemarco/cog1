import { Injectable } from '@angular/core';
import { ProfileModalComponent } from '../modals/profile/profile-modal.component';

@Injectable({
  providedIn: 'root'
})
export class ViewStatusService {

  constructor() { }

  public title: string = "No title";
  public profileModal?: ProfileModalComponent = undefined;

  public setTitle(title: string)
  {
    this.title = title;
  }

  public setProfileModal(p: ProfileModalComponent) {
    this.profileModal = p;
  }

  public showProfileModal() {
    if (this.profileModal)
      this.profileModal.showModal();
  }

}
