import { Component, OnInit, ViewChild } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonDirective, CardBodyComponent, CardComponent, ColComponent,
         RowComponent, TextColorDirective } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { AuthService } from '../../../services/auth.service';
import { ViewStatusService } from '../../../services/view-status.service';
import { BackendService } from '../../../services/backend.service';
import { EthernetReportDTO, JsonControllerException, LiteralsContainerDTO, ValuePairDTO } from '../../../api-client/data-contracts';
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
  public ethernetValues: ValuePairDTO[] = [];
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

  public static makeValuePairs(report: EthernetReportDTO, literals: LiteralsContainerDTO): ValuePairDTO[]
  {
    let result: ValuePairDTO[] = [];
    let ln = literals!.network!;
    let indent: string = "\u00a0\u00a0\u00a0";

    result = [];
    result.push({ key: ln.connection, value: "" });
    result.push({ key: indent + ln.status, value: report.isConnected ? ln.connected : ln.disconnected});
    result.push({ key: indent + ln.macAddress, value: report.macAddress });
    if (report.isConnected) {
      if (report.autoNegotiate) {
        result.push({ key: indent + ln.speed + " (" + 
          (report.fullDuplex ? ln.fullDuplex?.toLowerCase() : ln.halfDuplex?.toLowerCase()) + ")", 
          value: "Auto (" + report.speed!.toString() + " Mb/s)"  });
      } else {
        result.push({ key: indent + ln.speed + " (" + 
          (report.fullDuplex ? ln.fullDuplex?.toLowerCase() : ln.halfDuplex)?.toLowerCase() + ")", 
          value: report.speed!.toString() + " Mb/s"  });
      }
      result.push({ key: "IP", value: "" });
      result.push({ key: indent + ln.ipMethod, value: report.ipConfiguration?.dhcp ? "DHCP" : ln.ipFixed });
      result.push({ key: indent + ln.ipAddress, value: report.ipConfiguration?.ipv4 + "/" + report.ipConfiguration?.netMask?.toString() });
      result.push({ key: indent + ln.gateway, value: report.ipConfiguration?.gateway });
      result.push({ key: indent + "DNS", value: report.ipConfiguration?.dns });
    }  
    return result;
  }

  private reload()
  {
    if (this.destroying) return;

    this.backend.system.getSystemStats()
      .then(data =>
      {
        let ln = this.literals.network!;

        // Ethernet
        this.isConnected = data.data.ethernet?.isConnected!;
        this.ethernetValues = NetworkEthernetComponent.makeValuePairs(data.data.ethernet!, this.literals);

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
