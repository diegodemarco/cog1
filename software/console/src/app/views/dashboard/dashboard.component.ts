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

interface IUser {
  name: string;
  state: string;
  registered: string;
  country: string;
  usage: number;
  period: string;
  payment: string;
  activity: string;
  avatar: string;
  status: string;
  color: string;
}

@Component({
  templateUrl: 'dashboard.component.html',
  styleUrls: ['dashboard.component.scss'],
  standalone: true,
  imports: [WidgetsDropdownComponent, TextColorDirective, CardComponent, CardBodyComponent, RowComponent, ColComponent, ButtonDirective, IconDirective, ReactiveFormsModule, ButtonGroupComponent, FormCheckLabelDirective, ChartjsComponent, NgStyle, CardFooterComponent, GutterDirective, ProgressBarDirective, ProgressComponent, WidgetsBrandComponent, CardHeaderComponent, TableDirective, AvatarComponent]
})
export class DashboardComponent implements OnInit {

  public users: IUser[] = [
    {
      name: 'Yiorgos Avraamu',
      state: 'New',
      registered: 'Jan 1, 2021',
      country: 'Us',
      usage: 50,
      period: 'Jun 11, 2021 - Jul 10, 2021',
      payment: 'Mastercard',
      activity: '10 sec ago',
      avatar: './assets/images/avatars/1.jpg',
      status: 'success',
      color: 'success'
    },
    {
      name: 'Avram Tarasios',
      state: 'Recurring ',
      registered: 'Jan 1, 2021',
      country: 'Br',
      usage: 10,
      period: 'Jun 11, 2021 - Jul 10, 2021',
      payment: 'Visa',
      activity: '5 minutes ago',
      avatar: './assets/images/avatars/2.jpg',
      status: 'danger',
      color: 'info'
    },
    {
      name: 'Quintin Ed',
      state: 'New',
      registered: 'Jan 1, 2021',
      country: 'In',
      usage: 74,
      period: 'Jun 11, 2021 - Jul 10, 2021',
      payment: 'Stripe',
      activity: '1 hour ago',
      avatar: './assets/images/avatars/3.jpg',
      status: 'warning',
      color: 'warning'
    },
    {
      name: 'Enéas Kwadwo',
      state: 'Sleep',
      registered: 'Jan 1, 2021',
      country: 'Fr',
      usage: 98,
      period: 'Jun 11, 2021 - Jul 10, 2021',
      payment: 'Paypal',
      activity: 'Last month',
      avatar: './assets/images/avatars/4.jpg',
      status: 'secondary',
      color: 'danger'
    },
    {
      name: 'Agapetus Tadeáš',
      state: 'New',
      registered: 'Jan 1, 2021',
      country: 'Es',
      usage: 22,
      period: 'Jun 11, 2021 - Jul 10, 2021',
      payment: 'ApplePay',
      activity: 'Last week',
      avatar: './assets/images/avatars/5.jpg',
      status: 'success',
      color: 'primary'
    },
    {
      name: 'Friderik Dávid',
      state: 'New',
      registered: 'Jan 1, 2021',
      country: 'Pl',
      usage: 43,
      period: 'Jun 11, 2021 - Jul 10, 2021',
      payment: 'Amex',
      activity: 'Yesterday',
      avatar: './assets/images/avatars/6.jpg',
      status: 'info',
      color: 'dark'
    }
  ];

  readonly #destroyRef: DestroyRef = inject(DestroyRef);
  readonly #document: Document = inject(DOCUMENT);
  readonly #renderer: Renderer2 = inject(Renderer2);

  private cpuChartRef: WritableSignal<any> = signal(undefined);
  public cpuChartData!: ChartData;
  public cpuChartOptions!: ChartOptions;

  //public chart: Array<IChartProps> = [];
  public literals: LiteralsService;

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
        return this.backend.system.getSystemStats()
      })
      .then((data) =>
      {
        // CPU
        this.cpuPercentage = 100 - data.data.cpuReport?.usage?.last5Minutes?.idlePercentage!;
        this.cpuColor = this.makeColor(this.cpuPercentage, 15, 50);
        this.cpuText = this.cpuPercentage.toFixed(2) + "%";
        // RAM
        this.ramPercentage = 100 - data.data.memory?.freePercentage!;
        this.ramColor = this.makeColor(this.ramPercentage, 75, 90);
        this.ramText = this.formatStorage(data.data.memory?.usedBytes!) + " (" + this.ramPercentage.toFixed(0) + "%)";
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
}
