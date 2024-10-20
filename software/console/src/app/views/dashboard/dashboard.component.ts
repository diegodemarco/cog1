import { DOCUMENT, NgStyle } from '@angular/common';
import { Component, DestroyRef, effect, inject, OnInit, Renderer2, signal, WritableSignal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ChartData, ChartDataset, ChartOptions, PluginOptionsByType, ScaleOptions, TooltipLabelStyle } from 'chart.js';
import {
  AvatarComponent,
  ButtonDirective,
  ButtonGroupComponent,
  CardBodyComponent,
  CardComponent,
  CardFooterComponent,
  CardHeaderComponent,
  ColComponent,
  FormCheckLabelDirective,
  GutterDirective,
  ProgressBarDirective,
  ProgressComponent,
  RowComponent,
  TableDirective,
  TextColorDirective
} from '@coreui/angular';
import { ChartjsComponent } from '@coreui/angular-chartjs';
import { IconDirective } from '@coreui/icons-angular';
import { getStyle, hexToRgba } from '@coreui/utils';

import { WidgetsBrandComponent } from '../widgets/widgets-brand/widgets-brand.component';
import { WidgetsDropdownComponent } from '../widgets/widgets-dropdown/widgets-dropdown.component';
//import { DashboardChartsData, IChartProps } from './dashboard-charts-data';
import { LiteralsService } from 'src/app/services/literals.service';
import { ViewStatusService } from 'src/app/services/view-status.service';
import { BackendService } from 'src/app/services/backend.service';
import { DeepPartial } from 'chart.js/dist/types/utils';
import { VariableType, VariableDTO, VariableDirection } from 'src/app/api-client/data-contracts';
import { IconSubset } from 'src/app/icons/icon-subset';

interface VarWithValue extends VariableDTO 
{
  value?: number | null | undefined;
}

@Component({
  templateUrl: 'dashboard.component.html',
  styleUrls: ['dashboard.component.scss'],
  standalone: true,
  imports: [WidgetsDropdownComponent, TextColorDirective, CardComponent, CardBodyComponent, RowComponent, ColComponent, ButtonDirective, IconDirective, ReactiveFormsModule, ButtonGroupComponent, FormCheckLabelDirective, ChartjsComponent, NgStyle, CardFooterComponent, GutterDirective, ProgressBarDirective, ProgressComponent, WidgetsBrandComponent, CardHeaderComponent, TableDirective, AvatarComponent]
})
export class DashboardComponent implements OnInit {

  readonly VariableType = VariableType;
  readonly VariableDirection = VariableDirection;

  readonly #destroyRef: DestroyRef = inject(DestroyRef);
  readonly #document: Document = inject(DOCUMENT);
  readonly #renderer: Renderer2 = inject(Renderer2);

  private cpuChartRef: WritableSignal<any> = signal(undefined);
  public cpuChartData!: ChartData;
  public cpuChartOptions!: ChartOptions;

  //public chart: Array<IChartProps> = [];
  public literals: LiteralsService;

  // Stats data
  public cpuPercentage: number | null = null;
  public cpuText: string | null = "...";
  public cpuColor: string = "succes";
  public ramPercentage: number | null = null;
  public ramText: string | null = "...";
  public ramColor: string = "succes";
  public storagePercentage: number | null = null;
  public storageText: string | null = "...";
  public storageColor: string = "succes";
  public temperaturePercentage: number | null = null;
  public temperatureText: string | null = "...";
  public temperatureColor: string = "succes";

  // Variable data
  public builtinVariables: VarWithValue[] = [];

  constructor(private backend: BackendService, literals: LiteralsService, viewStatus: ViewStatusService) 
  {
    this.literals = literals;
    viewStatus.setTitle(literals.dashboard.dashboard!);
  }

  ngOnInit(): void {

    this.initCpuChart();

    this.updateChartOnColorModeChange();

    this.backend.system.getCpuHistory5Min()
      .then((data) => 
      {
        this.updateCpuChart(data.data);
      });

    this.backend.system.getSystemStats()
      .then(data =>
      {
        // CPU
        this.cpuPercentage = 100 - data.data.cpuReport?.usage?.last5Minutes?.idlePercentage!;
        this.cpuColor = this.makeColor(this.cpuPercentage, 15, 50);
        this.cpuText = this.cpuPercentage.toFixed(2) + "%";
        // RAM
        this.ramPercentage = 100 - data.data.memory?.freePercentage!;
        this.ramColor = this.makeColor(this.ramPercentage, 75, 90);
        this.ramText = this.formatStorage(data.data.memory?.totalBytes! - data.data.memory?.freeBytes!) 
          + " (" + this.ramPercentage.toFixed(0) + "%)";
        // Storage
        this.storagePercentage = 100 - data.data.disk?.freePercentage!;
        this.storageColor = this.makeColor(this.storagePercentage, 70, 90);
        this.storageText = this.formatStorage(data.data.disk?.bytesUsed!) + " (" + this.storagePercentage.toFixed(0) + "%)";
        // Temperature
        this.temperaturePercentage = (100 * data.data.temperature?.maxTemperatureC!) / 100;
        this.temperatureColor = this.makeColor(this.temperaturePercentage, 60, 80);
        this.temperatureText = (data.data.temperature?.maxTemperatureC!).toFixed(0) + " °C";
      });

    this.loadVariables();
  }

  private initCpuChart()
  {
    const plugins: DeepPartial<PluginOptionsByType<any>> = {
      legend: {
        display: false
      },
      tooltip: {
        callbacks: {
          labelColor: (context) => ({ backgroundColor: context.dataset.borderColor } as TooltipLabelStyle)
        }
      }
    };

    const scales = this.getCpuChartScales();
  
    this.cpuChartOptions = {
      maintainAspectRatio: false,
      plugins,
      scales,
      elements: {
        line: {
          tension: 0.4
        },
        point: {
          radius: 0,
          hitRadius: 10,
          hoverRadius: 4,
          hoverBorderWidth: 3
        }
      }
    };

  }

  private getCpuChartScales() 
  {
    const colorBorderTranslucent = getStyle('--cui-border-color-translucent');
    const colorBody = getStyle('--cui-body-color');

    const scales: ScaleOptions<any> = {
      x: {
        grid: {
          color: colorBorderTranslucent,
          drawOnChartArea: false
        },
        ticks: {
          color: colorBody
        }
      },
      y: {
        border: {
          color: colorBorderTranslucent
        },
        grid: {
          color: colorBorderTranslucent
        },
        max: 100,
        beginAtZero: true,
        // ticks: {
        //   color: colorBody,
        //   maxTicksLimit: 5,
        //   stepSize: Math.ceil(100 / 5)
        // }
      }
    };
    return scales;
  }

  private updateCpuChart(cpuData: number[])
  {
    const brandInfoBg = hexToRgba(getStyle('--cui-info') ?? '#20a8d8', 10);
    const brandInfo = getStyle('--cui-info') ?? '#20a8d8';

    const datasets: ChartDataset[] = [
      {
        data: cpuData,
        label: 'CPU',
        backgroundColor: brandInfoBg,
        borderColor: brandInfo,
        pointHoverBackgroundColor: brandInfo,
        borderWidth: 2,
        fill: true
      }
    ];

    const labels: string[] = [];
    for (let i: number = 0; i < cpuData.length; i++) {
      labels.push((cpuData.length - i).toString());
    }

    this.cpuChartData = {
      datasets,
      labels
    };
  }

  private makeColor(value: number, limit1: number, limit2: number): string
  {
    if (value >= limit2)
      return "danger";
    if (value >= limit1)
      return "warning";
    return "success";
  }

  private formatStorage(bytes: number): string
  {
    if (bytes > 1024 * 1024 * 1024)
      return (bytes / 1024 / 1024 / 1024).toFixed(2) + " GB";
    if (bytes > 1024 * 1024)
      return (bytes / 1024 / 1024).toFixed(0) + " MB";
    if (bytes > 1024)
      return (bytes / 1024).toFixed(0) + " KB";
    return bytes.toFixed(0) + " B";
  }

  handleChartRef($chartRef: any) {
    if ($chartRef) {
      this.cpuChartRef.set($chartRef);
    }
  }

  updateChartOnColorModeChange() {
    const unListen = this.#renderer.listen(this.#document.documentElement, 'ColorSchemeChange', () => {
      this.setChartStyles();
    });

    this.#destroyRef.onDestroy(() => {
      unListen();
    });
  }

  setChartStyles() {
    if (this.cpuChartRef()) {
      setTimeout(() => {
        const options: ChartOptions = { ...this.cpuChartOptions };
        const scales = this.getCpuChartScales();
        this.cpuChartRef().options.scales = { ...options.scales, ...scales };
        this.cpuChartRef().update();
      });
    }
  }

  getIconSubset(type: VariableType): string
  {
    switch (type)
    {
      case VariableType.Binary:
        return IconSubset.cilToggleOn;
      case VariableType.Integer:
      case VariableType.FloatingPoint:
        return IconSubset.cibCreativeCommonsZero;
    }
    return "";
  }

  getVariableType(type: VariableType): string
  {
    switch (type)
    {
      case VariableType.Binary:
        return this.literals.common.binary!;
      case VariableType.Integer:
        return this.literals.common.integer!;
      case VariableType.FloatingPoint:
        return this.literals.common.fLoatingPoint!;
    }
    return "";
  }

  private loadVariables()
  {
    this.backend.variables.enumerateVariables()
      .then(data =>
      {
        let list: VarWithValue[] = [];
        data.data.filter((item) => item.variableId! < 1000)
          .forEach(item =>
          {
            let x: VarWithValue = {};
            Object.assign(x, item);
            list.push(x);
          });
        this.builtinVariables = list;
        return this.backend.variables.getVariableValues();
      })
      .then(data => 
      {
        data.data.forEach(item =>
        {
          let v = this.builtinVariables.find(x => x.variableId! == item.variableId!);
          v!.value = item.value
        });
      });
  }

  setBinaryOutput(variableId: number, turnOn: boolean)
  {
    this.backend.variables.setVariableValue(variableId, turnOn ? 1 : 0)
      .then(() =>
      {
        this.loadVariables();
      })
  }

}
