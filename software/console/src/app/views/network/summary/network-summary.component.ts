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
import { NetworkWiFiComponent } from '../wifi/network-wifi.component';
import { NetworkEthernetComponent } from '../ethernet/network-ethernet.component';
import { ValuePair } from '../../../shared/value-pair';

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
        this.ethernetValues = NetworkEthernetComponent.makeValuePairs(data.data.ethernet!, this.literals);

        // WiFi
        this.wifiValues = NetworkWiFiComponent.makeValuePairs(data.data.wiFi!, this.literals);

      });
  }

}
