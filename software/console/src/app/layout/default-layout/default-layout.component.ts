import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { NgScrollbar } from 'ngx-scrollbar';
import { IconDirective } from '@coreui/icons-angular';
import { ContainerComponent, INavData, ShadowOnScrollDirective, SidebarBrandComponent, SidebarComponent,
         SidebarFooterComponent, SidebarHeaderComponent, SidebarNavComponent, SidebarToggleDirective,
         SidebarTogglerDirective, ToasterComponent, ToasterPlacement} from '@coreui/angular';
import { DefaultFooterComponent, DefaultHeaderComponent } from './';
import { IconSubset } from 'src/app/icons/icon-subset';
import { ViewStatusService } from 'src/app/services/view-status.service';
import { ProfileModalComponent } from 'src/app/modals/profile/profile-modal.component';
import { WarningPromptModalComponent } from 'src/app/modals/profile/warning-prompt-modal.component';
import { BasicEntitiesService } from 'src/app/services/basic-entities.service';
import { LiteralsContainerDTO } from 'src/app/api-client/data-contracts';

function isOverflown(element: HTMLElement) {
  return (
    element.scrollHeight > element.clientHeight ||
    element.scrollWidth > element.clientWidth
  );
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './default-layout.component.html',
  styleUrls: ['./default-layout.component.scss'],
  standalone: true,
  imports: [
    SidebarComponent,
    SidebarHeaderComponent,
    SidebarBrandComponent,
    RouterLink,
    IconDirective,
    NgScrollbar,
    SidebarNavComponent,
    SidebarFooterComponent,
    SidebarToggleDirective,
    SidebarTogglerDirective,
    DefaultHeaderComponent,
    ShadowOnScrollDirective,
    ContainerComponent,
    RouterOutlet,
    DefaultFooterComponent,
    ProfileModalComponent,
    WarningPromptModalComponent,
    ToasterComponent
]
})
export class DefaultLayoutComponent implements AfterViewInit 
{
  @ViewChild(ProfileModalComponent) profileModal!: ProfileModalComponent;
  @ViewChild(WarningPromptModalComponent) warningPromptModal!: WarningPromptModalComponent;
  @ViewChild(ToasterComponent)toaster!: ToasterComponent;

  readonly literals: LiteralsContainerDTO;
  readonly navItems: INavData[];
  readonly toasterPlacement = ToasterPlacement;

  constructor(private viewStatus: ViewStatusService, basicEntitiesService: BasicEntitiesService)
  {
    this.literals = basicEntitiesService.literals;
    this.navItems = this.getNavItems();
  }

  ngAfterViewInit() {
    this.viewStatus.setProfileModal(this.profileModal);
    this.viewStatus.setWarningPromptModal(this.warningPromptModal);
    this.viewStatus.setToaster(this.toaster);
  }

  onScrollbarUpdate($event: any) {
    // if ($event.verticalUsed) {
    // console.log('verticalUsed', $event.verticalUsed);
    // }
  }

  doVisibleChanged(isVisible: boolean)
  {
    setTimeout(() => {
      window.dispatchEvent(new Event('resize'));
    }, 300);
  }

  private getNavItems(): INavData[]
  {
    return [
      {
        name: 'Dashboard',
        url: '/dashboard',
        iconComponent: { name: IconSubset.cilSpeedometer },
      },
      {
        name: this.literals.variables!.variables!,
        url: '/variables',
        iconComponent: { name: IconSubset.cilGraph },
      },
      {
        name: this.literals.security!.security!,
        url: '/security',
        iconComponent: { name: IconSubset.cilLockLocked },
        children: [
          {
            name: this.literals.security!.users!,
            url: '/security/users',
            iconComponent: { name: IconSubset.cilGroup }
          }       
        ]
      },
/*
      {
        name: 'Notifications',
        url: '/notifications',
        iconComponent: { name: 'cil-bell' },
        children: [
          {
            name: 'Modal',
            url: '/notifications/modal',
            icon: 'nav-icon-bullet'
          },
        ]
      },
      {
        title: true,
        name: 'Theme'
      },
      {
        name: 'Colors',
        url: '/theme/colors',
        iconComponent: { name: 'cil-drop' }
      },
      {
        name: 'Typography',
        url: '/theme/typography',
        linkProps: { fragment: 'headings' },
        iconComponent: { name: 'cil-pencil' }
      },
      {
        name: 'Components',
        title: true
      },
      {
        name: 'Base',
        url: '/base',
        iconComponent: { name: 'cil-puzzle' },
        children: [
          {
            name: 'Accordion',
            url: '/base/accordion',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Breadcrumbs',
            url: '/base/breadcrumbs',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Cards',
            url: '/base/cards',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Carousel',
            url: '/base/carousel',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Collapse',
            url: '/base/collapse',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'List Group',
            url: '/base/list-group',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Navs & Tabs',
            url: '/base/navs',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Pagination',
            url: '/base/pagination',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Placeholder',
            url: '/base/placeholder',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Popovers',
            url: '/base/popovers',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Progress',
            url: '/base/progress',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Spinners',
            url: '/base/spinners',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Tables',
            url: '/base/tables',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Tabs',
            url: '/base/tabs',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Tooltips',
            url: '/base/tooltips',
            icon: 'nav-icon-bullet'
          }
        ]
      },
      {
        name: 'Buttons',
        url: '/buttons',
        iconComponent: { name: 'cil-cursor' },
        children: [
          {
            name: 'Buttons',
            url: '/buttons/buttons',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Button groups',
            url: '/buttons/button-groups',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Dropdowns',
            url: '/buttons/dropdowns',
            icon: 'nav-icon-bullet'
          }
        ]
      },
      /*
      {
        name: 'Forms',
        url: '/forms',
        iconComponent: { name: 'cil-notes' },
        children: [
          {
            name: 'Form Control',
            url: '/forms/form-control',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Select',
            url: '/forms/select',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Checks & Radios',
            url: '/forms/checks-radios',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Range',
            url: '/forms/range',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Input Group',
            url: '/forms/input-group',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Floating Labels',
            url: '/forms/floating-labels',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Layout',
            url: '/forms/layout',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Validation',
            url: '/forms/validation',
            icon: 'nav-icon-bullet'
          }
        ]
      },
      /*
      {
        name: 'Charts',
        iconComponent: { name: 'cil-chart-pie' },
        url: '/charts'
      },
      {
        name: 'Icons',
        iconComponent: { name: 'cil-star' },
        url: '/icons',
        children: [
          {
            name: 'CoreUI Free',
            url: '/icons/coreui-icons',
            icon: 'nav-icon-bullet',
            badge: {
              color: 'success',
              text: 'FREE'
            }
          },
          {
            name: 'CoreUI Flags',
            url: '/icons/flags',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'CoreUI Brands',
            url: '/icons/brands',
            icon: 'nav-icon-bullet'
          }
        ]
      },
      {
        name: 'Notifications',
        url: '/notifications',
        iconComponent: { name: 'cil-bell' },
        children: [
          {
            name: 'Alerts',
            url: '/notifications/alerts',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Badges',
            url: '/notifications/badges',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Modal',
            url: '/notifications/modal',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Toast',
            url: '/notifications/toasts',
            icon: 'nav-icon-bullet'
          }
        ]
      },
      {
        name: 'Widgets',
        url: '/widgets',
        iconComponent: { name: 'cil-calculator' },
        badge: {
          color: 'info',
          text: 'NEW'
        }
      },
      {
        title: true,
        name: 'Extras'
      },
      {
        name: 'Pages',
        url: '/login',
        iconComponent: { name: 'cil-star' },
        children: [
          {
            name: 'Login',
            url: '/login',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Register',
            url: '/register',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Error 404',
            url: '/404',
            icon: 'nav-icon-bullet'
          },
          {
            name: 'Error 500',
            url: '/500',
            icon: 'nav-icon-bullet'
          }
        ]
      },
      {
        title: true,
        name: 'Links',
        class: 'mt-auto'
      },
      {
        name: 'Docs',
        url: 'https://coreui.io/angular/docs/5.x/',
        iconComponent: { name: 'cil-description' },
        attributes: { target: '_blank' }
      }
      */
    ];
  }

}