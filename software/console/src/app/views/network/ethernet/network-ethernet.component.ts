import { Component, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonDirective, CardBodyComponent, CardComponent, ColComponent,
         RowComponent, TextColorDirective } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { BasicEntitiesService } from '../../../services/basic-entities.service';
import { AuthService } from '../../../services/auth.service';
import { ViewStatusService } from '../../../services/view-status.service';
import { BackendService } from '../../../services/backend.service';
import { LiteralsContainerDTO } from '../../../api-client/data-contracts';
import { IconSubset } from '../../../icons/icon-subset';
import { CrudPageComponent } from '../../../shared/crud-page/crud-page.component'

@Component({
  templateUrl: 'network-ethernet.component.html',
  styleUrls: ['network-ethernet.component.scss'],
  standalone: true,
  imports: [TextColorDirective, CardComponent, CardBodyComponent, RowComponent,
    ColComponent, ButtonDirective, IconDirective, ReactiveFormsModule,
    FormsModule, CrudPageComponent]
})
export class NetworkEthernetComponent implements OnInit
{
  // Template data
  readonly iconSubset = IconSubset;
  readonly literals: LiteralsContainerDTO;
  readonly indent: string = "\u00a0\u00a0\u00a0";
  public ethernetValues: ValuePair[] = [];
  authService: AuthService;

  constructor(private backend: BackendService, viewStatus: ViewStatusService,
    basicEntitiesService: BasicEntitiesService, authService: AuthService) 
  {
    this.literals = basicEntitiesService.literals;
    this.authService = authService;
    viewStatus.setTitle(this.literals.network!.network! + " - Ethernet");
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
        this.ethernetValues.push({ key: this.indent + ln.status, value: eth.isConnected ? ln.connected : ln.disconnected});
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
          this.ethernetValues.push({ key: this.indent + ln.ipMethod, value: eth.dhcp ? "DHCP" : ln.ipFixed });
          this.ethernetValues.push({ key: this.indent + ln.ipAddress, value: eth.ipv4! + "/" + eth.maskBits?.toString() });
          this.ethernetValues.push({ key: this.indent + ln.gateway, value: eth.gateway });
          this.ethernetValues.push({ key: this.indent + "DNS", value: eth.dns });
        }

      });
  }

  doConfigureLink() {

  }

  doConfigureIp() {
    
  }

}

class ValuePair
{
  public key?: string | null;
  public value?: string | null;
}
