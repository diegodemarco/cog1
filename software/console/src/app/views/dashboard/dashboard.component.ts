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
import { interval, Subscription, timer } from 'rxjs';
import { animation } from '@angular/animations';

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
export class DashboardComponent implements OnInit 
{
  // Enums
  readonly VariableType = VariableType;
  readonly VariableDirection = VariableDirection;

  // Subscriptions
  private cpuTimerSub: Subscription | null = null;
  private statTimerSub: Subscription | null = null;
  private varTimerSub: Subscription | null = null;

  readonly #destroyRef: DestroyRef = inject(DestroyRef);
  readonly #document: Document = inject(DOCUMENT);
  readonly #renderer: Renderer2 = inject(Renderer2);

  //
  public cpuChartData: ChartData;
  private cpuChartLabels: string[];
  private cpuChartScales: ScaleOptions<any>;
  private brandInfoBg: string | undefined;
  private brandInfo: string | undefined;

  private cpuChartRef: WritableSignal<any> = signal(undefined);
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
    this.updateChartBrandInfo();
    this.cpuChartLabels = [];
    this.cpuChartData = {
      datasets: [],
      labels: this.cpuChartLabels
    };
  }

  ngOnInit(): void {

    // Update color scheme for the chart, and respond to theme changes
    this.updateChartBrandInfo();
    this.updateChartOnColorModeChange();

    // Timers
    this.cpuTimerSub  = timer(0,  5000).subscribe(() => this.updateCpuHistory());   // Every 5 seconds
    this.statTimerSub = timer(0, 30000).subscribe(() => this.updateSystemStats());  // Every 1 minute
    this.varTimerSub  = timer(0,  2000).subscribe(() => this.updateVariables());    // Every 2 seconds
  }

  ngOnDestroy(): void
  {
    // Unsubscribe from timers
    this.cpuTimerSub!.unsubscribe();
    this.statTimerSub!.unsubscribe();
    this.varTimerSub!.unsubscribe();
  }

  private updateCpuHistory(): void
  {
    this.backend.system.getCpuHistory5Min()
      .then((data) => 
      {
        this.updateCpuChartData(data.data);
      });
  }

  private updateSystemStats()
  {
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
  }

  private updateVariables()
  {
    let tempList: VarWithValue[] = [];

    this.backend.variables.enumerateVariables()
      .then(data =>
      {
        data.data.filter((item) => item.variableId! < 1000)
          .forEach(item =>
          {
            let x: VarWithValue = {};
            Object.assign(x, item);
            tempList.push(x);
          });
        return this.backend.variables.getVariableValues();
      })
      .then(data => 
      {
        data.data.forEach(item =>
        {
          let v = tempList.find(x => x.variableId! == item.variableId!);
          v!.value = item.value
        });
        this.builtinVariables = tempList;
      });
  }

  private updateChartBrandInfo()
  {
    this.brandInfoBg = hexToRgba(getStyle('--cui-info') ?? '#20a8d8', 10);
    this.brandInfo = getStyle('--cui-info') ?? '#20a8d8';

    const colorBorderTranslucent = getStyle('--cui-border-color-translucent');
    const colorBody = getStyle('--cui-body-color');

    console.log("brandInfoBg: ", this.brandInfoBg, "brandInfo: ", this.brandInfo);
    console.log("colorBorderTranslucent: ", colorBorderTranslucent, "colorBody: ", colorBody);

    // Plugins
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

    // Scales
    this.cpuChartScales = {
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

    // Options
    this.cpuChartOptions = {
      animations: { animation: { duration: 0 } },
      maintainAspectRatio: false,
      plugins,
      scales: this.cpuChartScales,
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

  private updateCpuChartData(cpuData: number[])
  {
    // Update chart labels if necessary
    if (this.cpuChartLabels.length != cpuData.length)
    {
      this.cpuChartLabels = [];
      for (let i: number = 0; i < cpuData.length; i++)
        this.cpuChartLabels.push((cpuData.length - i).toString());
    }

    // Update chart data
    const dataset =
    {
      data: cpuData,
      label: 'CPU',
      backgroundColor: this.brandInfoBg,
      borderColor: this.brandInfo,
      pointHoverBackgroundColor: this.brandInfo,
      borderWidth: 2,
      fill: true
    };
    this.cpuChartData = {
      datasets: [dataset],
      labels: this.cpuChartLabels
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

  setChartStyles() 
  {
    if (this.cpuChartRef()) 
    {
      this.updateChartBrandInfo();
      setTimeout(() => {
        const options: ChartOptions = { ...this.cpuChartOptions };
        this.cpuChartRef().options.scales = { ...options.scales, ...this.cpuChartScales };
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

  setBinaryOutput(variableId: number, turnOn: boolean)
  {
    this.backend.variables.setVariableValue(variableId, turnOn ? 1 : 0)
      .then(() =>
      {
        this.updateVariables();
      })
  }

}
