import { Component, OnInit, ViewChild } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonDirective, CardBodyComponent, CardComponent, ColComponent,
         RowComponent, TextColorDirective } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { AuthService } from '../../../services/auth.service';
import { ViewStatusService } from '../../../services/view-status.service';
import { BackendService } from '../../../services/backend.service';
import { JsonControllerException, LiteralsContainerDTO } from '../../../api-client/data-contracts';
import { IconSubset } from '../../../icons/icon-subset';
import { CrudPageComponent } from '../../../shared/crud-page/crud-page.component'
import { BaseViewComponent } from '../../base/base-view.component';
import { IpConfigurationModalComponent } from '../modals/ip-configuration-modal.component';
import { LinkConfigurationModalComponent } from '../modals/link-configuration-modal.component';

@Component({
  templateUrl: 'network-ethernet.component.html',
  styleUrls: ['network-ethernet.component.scss'],
  standalone: true,
  imports: [TextColorDirective, CardComponent, CardBodyComponent, RowComponent,
    ColComponent, ButtonDirective, IconDirective, ReactiveFormsModule,
    FormsModule, CrudPageComponent, IpConfigurationModalComponent,
    LinkConfigurationModalComponent]
})
export class NetworkEthernetComponent extends BaseViewComponent implements OnInit
{
  // Template data
  readonly iconSubset = IconSubset;
  readonly literals: LiteralsContainerDTO;
  readonly indent: string = "\u00a0\u00a0\u00a0";
  private destroying: boolean = false;
  private isConnected: boolean = false;
  public ethernetValues: ValuePair[] = [];
  authService: AuthService;
  @ViewChild(IpConfigurationModalComponent) ipConfigurationModal!: IpConfigurationModalComponent;
  @ViewChild(LinkConfigurationModalComponent) linkConfigurationModal!: LinkConfigurationModalComponent;

  constructor(private backend: BackendService, viewStatus: ViewStatusService,
    basicEntitiesService: BasicEntitiesService, authService: AuthService) 
  {
    super(viewStatus);
    this.literals = basicEntitiesService.literals;
    this.authService = authService;
    viewStatus.setTitle(this.literals.network!.network! + " - Ethernet");
  }

  ngOnInit(): void {

    this.reload();
  }

  private reload()
  {
    if (this.destroying) return;

    this.backend.system.getSystemStats()
      .then(data =>
      {
        let ln = this.literals.network!;

        // Ethernet
        let eth = data.data.ethernet!;
        this.ethernetValues = [];
        this.ethernetValues.push({ key: ln.connection, value: "" });
        this.ethernetValues.push({ key: this.indent + ln.status, value: eth.isConnected ? ln.connected : ln.disconnected});
        this.ethernetValues.push({ key: this.indent + ln.macAddress, value: eth.macAddress });
        this.isConnected = eth.isConnected!;
        if (eth.isConnected) {
          if (eth.autoNegotiate) {
            this.ethernetValues.push({ key: this.indent + ln.speed + " (" + 
              (eth.fullDuplex ? ln.fullDuplex?.toLowerCase() : ln.halfDuplex?.toLowerCase()) + ")", 
              value: "Auto (" + eth.speed!.toString() + " Mb/s)"  });
          } else {
            this.ethernetValues.push({ key: this.indent + ln.speed + " (" + 
              (eth.fullDuplex ? ln.fullDuplex?.toLowerCase() : ln.halfDuplex)?.toLowerCase() + ")", 
              value: eth.speed!.toString() + " Mb/s"  });
          }
          this.ethernetValues.push({ key: "IP", value: "" });
          this.ethernetValues.push({ key: this.indent + ln.ipMethod, value: eth.ipConfiguration?.dhcp ? "DHCP" : ln.ipFixed });
          this.ethernetValues.push({ key: this.indent + ln.ipAddress, value: eth.ipConfiguration?.ipv4 + "/" + eth.ipConfiguration?.netMask?.toString() });
          this.ethernetValues.push({ key: this.indent + ln.gateway, value: eth.ipConfiguration?.gateway });
          this.ethernetValues.push({ key: this.indent + "DNS", value: eth.ipConfiguration?.dns });
        }

      });
  }

  override ngOnDestroy(): void {
    this.destroying = true;
    super.ngOnDestroy();
  }

  doConfigureLink() {
    if (this.destroying) return;
    this.confirmChanges(this.isConnected)
      .then(() =>
      {
        if (this.destroying) return;
        return this.backend.system.ethernetGetLinkConfiguration();
      })
      .then((response) =>
      {
        if (this.destroying) return;
        return this.linkConfigurationModal.showModal(response?.data!);
      })
      .then((config) =>
      {
        if (this.destroying) return;
        this.viewStatus.showProgressModal(this.literals.network?.linkConfiguration!, 
          this.literals.network?.configuringPleaseWait! + "...");
        this.backend.system.ethernetSetLinkConfiguration(config!)
          .then(() => 
          {
            if (this.destroying) return;
            this.viewStatus.hideProgressModal();
            this.viewStatus.showSuccessToast(this.literals.network?.configurationAppliedSuccessfully!);
            if (this.isConnected!)
              this.reload();
          })
          .catch((error) =>
          {
            if (this.destroying) return;
            this.viewStatus.hideProgressModal();
            let e: JsonControllerException = error.error;
            this.viewStatus.showErrorToast(e.message!);
          });
      });
  }

  doIpConfig() {
    if (this.destroying) return;
    this.confirmChanges(this.isConnected)
      .then(() =>
      {
        if (this.destroying) return;
        return this.backend.system.ethernetGetIpConfiguration();
      })
      .then((response) =>
      {
        if (this.destroying) return;
        return this.ipConfigurationModal.showModal(response?.data!);
      })
      .then((config) =>
      {
        if (this.destroying) return;
        this.viewStatus.showProgressModal(this.literals.network?.ipConfiguration!, 
          this.literals.network?.configuringPleaseWait! + "...");
        this.backend.system.ethernetSetIpConfiguration(config!)
          .then(() => 
          {
            if (this.destroying) return;
            this.viewStatus.hideProgressModal();
            this.viewStatus.showSuccessToast(this.literals.network?.configurationAppliedSuccessfully!);
            if (this.isConnected!)
              this.reload();
          })
          .catch((error) =>
          {
            if (this.destroying) return;
            this.viewStatus.hideProgressModal();
            let e: JsonControllerException = error.error;
            this.viewStatus.showErrorToast(e.message!);
          });
      });
  }

  private confirmChanges(condition: boolean) : Promise<void>
  {
    if (condition)
    {
      return this.viewStatus.showWarningModal(this.literals.common?.warning!, 
        this.literals.network?.confirmChanges!);
    }
    else {
      return new Promise((resolve, reject) => resolve());
    }
  }

}

class ValuePair
{
  public key?: string | null;
  public value?: string | null;
}
