import { Component, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonDirective, CardBodyComponent, CardComponent, ColComponent,
         RowComponent, TextColorDirective } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { ViewStatusService } from '../../../services/view-status.service';
import { BackendService } from '../../../services/backend.service';
import { LiteralsContainerDTO } from '../../../api-client/data-contracts';
import { IconSubset } from '../../../icons/icon-subset';
import { CrudPageComponent } from '../../../shared/crud-page/crud-page.component'

@Component({
  templateUrl: 'network-summary.component.html',
  styleUrls: ['network-summary.component.scss'],
  standalone: true,
  imports: [TextColorDirective, CardComponent, CardBodyComponent, RowComponent,
    ColComponent, ButtonDirective, IconDirective, ReactiveFormsModule,
    FormsModule, CrudPageComponent]
})
export class NetworkSummaryComponent implements OnInit
{
  // Template data
  readonly iconSubset = IconSubset;
  readonly literals: LiteralsContainerDTO;
  readonly indent: string = "\u00a0\u00a0\u00a0";
  public ethernetValues: ValuePair[] = [];
  public wifiValues: ValuePair[] = [];

  constructor(private backend: BackendService, viewStatus: ViewStatusService, basicEntitiesService: BasicEntitiesService) 
  {
    this.literals = basicEntitiesService.literals;
    viewStatus.setTitle(this.literals.network!.networkSummary!);
  }

  ngOnInit(): void {

    this.backend.system.getSystemStats()
      .then(data =>
      {
        let ln = this.literals.network!;

        // Ethernet
        let eth = data.data.ethernet!;
        this.ethernetValues = [];
        this.ethernetValues.push({ key: ln.connection, value: "" });
        this.ethernetValues.push({ key: this.indent + ln.status, value: eth.isConnected ? ln.connected : ln.disconnected });
        this.ethernetValues.push({ key: this.indent + ln.macAddress, value: eth.macAddress });
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

        // WiFi
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
          this.wifiValues.push({ key: this.indent + ln.ipMethod, value: wifi.ipConfiguration?.dhcp ? "DHCP" : ln.ipFixed });
          this.wifiValues.push({ key: this.indent + ln.ipAddress, value: wifi.ipConfiguration?.ipv4 + "/" + wifi.ipConfiguration?.netMask?.toString() });
          this.wifiValues.push({ key: this.indent + ln.gateway, value: wifi.ipConfiguration?.gateway });
          this.wifiValues.push({ key: this.indent + "DNS", value: wifi.ipConfiguration?.dns });
        }

      });
  }

}

class ValuePair
{
  public key?: string | null;
  public value?: string | null;
}
