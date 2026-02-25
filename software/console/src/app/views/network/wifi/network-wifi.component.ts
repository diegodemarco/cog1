import { AfterViewInit, Component, OnDestroy, ViewChild } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonDirective, CardBodyComponent, CardComponent, ColComponent,
         DropdownComponent, DropdownDividerDirective, DropdownHeaderDirective,
         DropdownItemDirective, DropdownMenuDirective, DropdownToggleDirective,
         RowComponent, TextColorDirective } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { AuthService } from '../../../services/auth.service';
import { ViewStatusService } from '../../../services/view-status.service';
import { BackendService } from '../../../services/backend.service';
import { JsonControllerException, LiteralsContainerDTO, WiFiMode, WiFiReport } from '../../../api-client/data-contracts';
import { IconSubset } from '../../../icons/icon-subset';
import { CrudPageComponent } from '../../../shared/crud-page/crud-page.component'
import { RouterModule } from '@angular/router';
import { BaseViewComponent } from '../../base/base-view.component';
import { WiFiPasswordModalComponent } from '../modals/wifi-password-modal.component';
import { IpConfigurationModalComponent } from '../modals/ip-configuration-modal.component';
import { ValuePair } from '../../../shared/value-pair';

@Component({
  templateUrl: 'network-wifi.component.html',
  styleUrls: ['network-wifi.component.scss'],
  standalone: true,
  imports: [TextColorDirective, CardComponent, CardBodyComponent, RowComponent,
    ColComponent, ButtonDirective, IconDirective, ReactiveFormsModule, RouterModule,
    FormsModule, CrudPageComponent, DropdownComponent, DropdownComponent, DropdownToggleDirective,
    DropdownMenuDirective, DropdownHeaderDirective, DropdownItemDirective, DropdownDividerDirective,
    WiFiPasswordModalComponent, IpConfigurationModalComponent]
})
export class NetworkWiFiComponent extends BaseViewComponent implements AfterViewInit, OnDestroy
{
  // Template data
  readonly iconSubset = IconSubset;
  readonly literals: LiteralsContainerDTO;
  readonly WiFiMode = WiFiMode;
  private destroying: boolean = false;
  private isConnected: boolean = false;
  public wifiValues: ValuePair[] = [];
  public networks: NetworkEntry[] = [];
  authService: AuthService;
  wifiMode: WiFiMode = WiFiMode.Station;
  @ViewChild(WiFiPasswordModalComponent) passwordModal!: WiFiPasswordModalComponent;
  @ViewChild(IpConfigurationModalComponent) ipConfigurationModal!: IpConfigurationModalComponent;

  constructor(private backend: BackendService, viewStatus: ViewStatusService,
    basicEntitiesService: BasicEntitiesService, authService: AuthService)
  {
    super(viewStatus);
    this.literals = basicEntitiesService.literals;
    this.authService = authService;
    viewStatus.setTitle(this.literals.network!.network! + " - Wi-Fi");
  }

  ngAfterViewInit(): void {
    this.viewStatus.notifyChildViewReady(this);
  }

  public override onViewReady(): void {
    this.reload();
  }

  override ngOnDestroy(): void {
    this.destroying = true;
    super.ngOnDestroy();
  }

  public static makeValuePairs(report: WiFiReport, literals: LiteralsContainerDTO): ValuePair[]
  {
    let result: ValuePair[] = [];
    let ln = literals!.network!;
    let indent: string = "\u00a0\u00a0\u00a0";

    result.push({ key: ln.connection, value: "" });
    if (report.mode == WiFiMode.AccessPoint) {
      result.push({ key: indent + ln.status, value: "AP mode" });
    }
    else if (report.mode == WiFiMode.TemporaryAccessPoint) {
      result.push({ key: indent + ln.status, value: "Temporary AP mode" });
    }
    else {
      result.push({ key: indent + ln.status, value: report.isConnected ? ln.connected : ln.disconnected});
    }
    result.push({ key: indent + ln.macAddress, value: report.macAddress });
    if (report.isConnected || report.mode == WiFiMode.AccessPoint || report.mode == WiFiMode.TemporaryAccessPoint) 
    {
      result.push({ key: indent + "SSID", value: report.ssid });
      result.push({ key: indent + ln.frequency, value: report.frequency?.toString() + " MHz" });
      if (report.mode == WiFiMode.Station)
        result.push({ key: indent + "RSSID", value: report.rssi?.toString() + " dBm" });
      result.push({ key: "IP", value: "" });
      if (report.mode == WiFiMode.Station)
        result.push({ key: indent + ln.ipMethod, value: report.ipConfiguration?.dhcp ? "DHCP" : ln.ipFixed });
      result.push({ key: indent + ln.ipAddress, value: report.ipConfiguration?.ipv4 + "/" + report.ipConfiguration?.netMask?.toString() });
      if (report.mode == WiFiMode.Station)
        result.push({ key: indent + ln.gateway, value: report.ipConfiguration?.gateway });
      if (report.mode == WiFiMode.Station)
        result.push({ key: indent + "DNS", value: report.ipConfiguration?.dns });
    }

    return result;
  }

  reload()
  {
    if (this.destroying) return;

    let ln = this.literals.network!;

    this.viewStatus.showProgressModal(this.literals.network?.scanning!, this.literals.network?.scanningPleaseWait! + "...");
    this.backend.system.getSystemStats()
      .then(data =>
      {
        if (this.destroying) return;

        // Summary
        this.wifiMode = data.data.wiFi!.mode!;
        this.isConnected = data.data.wiFi!.isConnected!;
        this.wifiValues = NetworkWiFiComponent.makeValuePairs(data.data.wiFi!, this.literals);

        // Scan networks
        return this.backend.system.getWiFiScan();
      })
      .then(data =>
      {
        if (this.destroying) return;
        this.networks = [];
        data!.data.forEach(item =>
        {
          this.networks.push( 
          { 
            ssid: item.ssid, 
            isConnected: item.isConnected,
            isSaved: item.isSaved,
            quality: item.quality
          });
        });
      })
      .finally(() =>
      {
        if (this.destroying) return;
        this.viewStatus.hideProgressModal();
      });
  }

  doConnect(n: NetworkEntry) {
    if (this.destroying) return;
    if (n.isSaved)
    {
      this.confirmChanges(this.isConnected)
        .then(() =>
        {
          if (this.destroying) return;
          this.viewStatus.showProgressModal(n?.ssid!, this.literals.network?.connectingPleaseWait! + "...");
          this.backend.system.wiFiReconnect({ ssid: n!.ssid! })
          .catch(error => 
          {
            if (this.destroying) return;
            let e: JsonControllerException = error.error;
            this.viewStatus.showErrorToast(e.message!);
          })
          .finally(() =>
          {
            if (this.destroying) return;
            this.viewStatus.hideProgressModal();
            this.reload();
          });
        });
    }
    else 
    {
      this.confirmChanges(this.isConnected)
        .then(() =>
        {
          if (this.destroying) return;
          return this.passwordModal.showModal();
        })
        .then((pass) =>
        {
          if (this.destroying) return;
          this.viewStatus.showProgressModal(n?.ssid!, this.literals.network?.connectingPleaseWait! + "...");
          this.backend.system.wiFiConnect({ ssid: n.ssid!, password: pass! })
            .catch(error => 
            {
              if (this.destroying) return;
              let e: JsonControllerException = error.error;
              this.viewStatus.showErrorToast(e.message!);
            })
            .finally(() =>
            {
              if (this.destroying) return;
              this.viewStatus.hideProgressModal();
              this.reload();
            });
        });
    }
  }

  doDisconnect(n: NetworkEntry) {
    if (this.destroying) return;
    this.confirmChanges(true)
      .then(() =>
      {
        if (this.destroying) return;
        this.viewStatus.showProgressModal(n?.ssid!, this.literals.network?.disconnectingPleaseWait! + "...");
        this.backend.system.wiFiDisconnect({ ssid: n!.ssid! })
          .catch(error => 
          {
            if (this.destroying) return;
            let e: JsonControllerException = error.error;
            this.viewStatus.showErrorToast(e.message!);
          }).finally(() =>
          {
            if (this.destroying) return;
            this.viewStatus.hideProgressModal();
            this.reload();
          });
      });
  }

  doForget(n: NetworkEntry) {
    if (this.destroying) return;
    this.confirmChanges(n.isConnected!)
      .then(() =>
      {
        if (this.destroying) return;
        this.viewStatus.showWarningModal(this.literals?.network?.forget!, this.literals?.network?.confirmForget!)
          .then(() =>
          {
            if (this.destroying) return;
            this.viewStatus.showProgressModal(n?.ssid!, this.literals.network?.forgettingPleaseWait! + "...");
            this.backend.system.wiFiForget({ ssid: n!.ssid! })
              .catch(error => 
              {
                if (this.destroying) return;
                let e: JsonControllerException = error.error;
                this.viewStatus.showErrorToast(e.message!);
              }).finally(() =>
              {
                if (this.destroying) return;
                this.viewStatus.hideProgressModal();
                this.reload();
              });
          });
      });
  }

  doIpConfig(n: NetworkEntry) {
    if (this.destroying) return;
    this.confirmChanges(n.isConnected!)
      .then(() =>
      {
        if (this.destroying) return;
        return this.backend.system.wiFiGetIpConfiguration({ ssid: n.ssid! });
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
        this.backend.system.wiFiSetIpConfiguration({ ssid: n.ssid!, ipConfiguration: config! })
        .then(() => 
        {
          if (this.destroying) return;
          this.viewStatus.hideProgressModal();
          this.viewStatus.showSuccessToast(this.literals.network?.configurationAppliedSuccessfully!);
          if (n.isConnected!)
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

  doEnableAp()
  {
    if (this.destroying) return;
    this.confirmChanges(this.isConnected)
      .then(() =>
      {
        if (this.destroying) return;
        this.viewStatus.showProgressModal("Access point", this.literals.network?.connectingPleaseWait! + "...");
        this.backend.system.wiFiEnableAccessPoint()
          .catch(error => 
          {
            if (this.destroying) return;
            let e: JsonControllerException = error.error;
            this.viewStatus.showErrorToast(e.message!);
          }).finally(() =>
          {
            if (this.destroying) return;
            this.viewStatus.hideProgressModal();
            this.reload();
          });
      });
  }

  doDisableAp()
  {
    if (this.destroying) return;
    this.confirmChanges(true)
      .then(() =>
      {
        if (this.destroying) return;
        this.viewStatus.showProgressModal("Access point", this.literals.network?.disconnectingPleaseWait! + "...");
        this.backend.system.wiFiDisableAccessPoint()
          .catch(error => 
          {
            if (this.destroying) return;
            let e: JsonControllerException = error.error;
            this.viewStatus.showErrorToast(e.message!);
          }).finally(() =>
          {
            if (this.destroying) return;
            this.viewStatus.hideProgressModal();
            this.reload();
          });
      });
  }

  doConfigureAp()
  {
    
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

class NetworkEntry
{
  public ssid?: string | null;
  public isConnected?: boolean | null;
  public isSaved?: boolean | null;
  public quality?: number | null;
}