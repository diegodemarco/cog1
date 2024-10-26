import { ChangeDetectorRef, Component, ElementRef, forwardRef, Input, Renderer2 } from '@angular/core';

import {
  ProgressBarComponent,
  ProgressBarDirective,
  ProgressComponent,
  ToastBodyComponent,
  ToastCloseDirective,
  ToastComponent,
  ToasterService,
  ToastHeaderComponent
} from '@coreui/angular';

@Component({
  selector: 'app-base-toast',
  templateUrl: './base-toast.component.html',
  styleUrls: ['./base-toast.component.scss'],
  providers: [{ provide: ToastComponent, useExisting: forwardRef(() => BaseToastComponent) }],
  standalone: true,
  imports: [ToastHeaderComponent, ToastBodyComponent, ToastCloseDirective, ProgressBarDirective, ProgressComponent, ProgressBarComponent]
})
export class BaseToastComponent extends ToastComponent {

  @Input() title = '';
  @Input() message = '';

  constructor(
    public override hostElement: ElementRef,
    public override renderer: Renderer2,
    public override toasterService: ToasterService,
    public override changeDetectorRef: ChangeDetectorRef
  ) {
    super(hostElement, renderer, toasterService, changeDetectorRef);
  }
}