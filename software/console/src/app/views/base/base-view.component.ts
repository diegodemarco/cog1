import { Component, OnDestroy } from '@angular/core';
import { ViewStatusService } from '../../services/view-status.service';

@Component({
  template: '',
  // templateUrl: 'network-wifi.component.html',
  // styleUrls: ['network-wifi.component.scss'],
  // standalone: true,
  // imports: [TextColorDirective, CardComponent, CardBodyComponent, RowComponent,
  //   ColComponent, ButtonDirective, IconDirective, ReactiveFormsModule, RouterModule,
  //   FormsModule, CrudPageComponent, DropdownComponent, DropdownComponent, DropdownToggleDirective,
  //   DropdownMenuDirective, DropdownHeaderDirective, DropdownItemDirective, DropdownDividerDirective]
})
export class BaseViewComponent implements OnDestroy
{

  protected viewStatus: ViewStatusService;

  constructor(viewStatus: ViewStatusService)
  {
    this.viewStatus = viewStatus;
  }

  ngOnDestroy(): void {
    this.viewStatus.hideModals();
  }

}