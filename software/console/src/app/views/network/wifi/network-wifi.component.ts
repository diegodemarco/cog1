import { AfterViewInit, Component, OnDestroy } from '@angular/core';
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
import { JsonControllerException, LiteralsContainerDTO } from '../../../api-client/data-contracts';
import { IconSubset } from '../../../icons/icon-subset';
import { CrudPageComponent } from '../../../shared/crud-page/crud-page.component'
import { RouterModule } from '@angular/router';
import { BaseViewComponent } from '../../base/base-view.component';

@Component({
  templateUrl: 'network-wifi.component.html',
  styleUrls: ['network-wifi.component.scss'],
  standalone: true,
  imports: [TextColorDirective, CardComponent, CardBodyComponent, RowComponent,
    ColComponent, ButtonDirective, IconDirective, ReactiveFormsModule, RouterModule,
    FormsModule, CrudPageComponent, DropdownComponent, DropdownComponent, DropdownToggleDirective,
    DropdownMenuDirective, DropdownHeaderDirective, DropdownItemDirective, DropdownDividerDirective]
})
export class NetworkWiFiComponent extends BaseViewComponent implements AfterViewInit, OnDestroy
{
  // Template data
  readonly iconSubset = IconSubset;
  readonly literals: LiteralsContainerDTO;
  readonly indent: string = "\u00a0\u00a0\u00a0";
  private destroying: boolean = false;
  public wifiValues: ValuePair[] = [];
  public networks: NetworkEntry[] = [];
  authService: AuthService;

  constructor(private backend: BackendService, viewStatus: ViewStatusService,
    basicEntitiesService: BasicEntitiesService, authService: AuthService)
  {
    super(viewStatus);
    this.literals = basicEntitiesService.literals;
    this.authService = authService;
    viewStatus.setTitle(this.literals.network!.network! + " - Wi-Fi");
  }

  ngAfterViewInit(): void {
    setTimeout(() => this.reload(), 100);
  }

  override ngOnDestroy(): void {
    this.destroying = true;
    super.ngOnDestroy();
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
        let wifi = data.data.wiFi!;
        this.wifiValues = [];
        this.wifiValues.push({ key: ln.connection, value: "" });
        this.wifiValues.push({ key: this.indent + ln.status, value: wifi.isConnected ? ln.connected : ln.disconnected});
        this.wifiValues.push({ key: this.indent + ln.macAddress, value: wifi.macAddress });
        if (wifi.isConnected) {
          this.wifiValues.push({ key: this.indent + "SSID", value: wifi.ssid });
          this.wifiValues.push({ key: this.indent + ln.frequency, value: wifi.frequency?.toString() + " MHz" });
          this.wifiValues.push({ key: this.indent + "RSSID", value: wifi.rssi?.toString() + " dBm" });
          this.wifiValues.push({ key: "IP", value: "" });
          this.wifiValues.push({ key: this.indent + ln.ipMethod, value: wifi.dhcp ? "DHCP" : ln.ipFixed });
          this.wifiValues.push({ key: this.indent + ln.ipAddress, value: wifi.ipv4! + "/" + wifi.maskBits?.toString() });
          this.wifiValues.push({ key: this.indent + ln.gateway, value: wifi.gateway });
          this.wifiValues.push({ key: this.indent + "DNS", value: wifi.dns });
        }

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
      this.confirmChanges(true)
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
      this.confirmChanges(true)
        .then(() =>
        {
          if (this.destroying) return;
          alert("not implemented");
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
          .then(() =>
          {
            if (this.destroying) return;
            this.reload();
          })
          .catch(error => 
          {
            if (this.destroying) return;
            let e: JsonControllerException = error.error;
            this.viewStatus.showErrorToast(e.message!);
          }).finally(() =>
          {
            if (this.destroying) return;
            this.viewStatus.hideProgressModal();
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
              .then(() =>
              {
                if (this.destroying) return;
                this.reload();
              })
              .catch(error => 
              {
                if (this.destroying) return;
                let e: JsonControllerException = error.error;
                this.viewStatus.showErrorToast(e.message!);
              }).finally(() =>
              {
                if (this.destroying) return;
                this.viewStatus.hideProgressModal();
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
        alert("ip config " + n.ssid);
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

class NetworkEntry
{
  public ssid?: string | null;
  public isConnected?: boolean | null;
  public isSaved?: boolean | null;
  public quality?: number | null;
}