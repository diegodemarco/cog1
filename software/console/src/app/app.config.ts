import { APP_INITIALIZER, ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideAnimations } from '@angular/platform-browser/animations';
import { ConfigService } from './services/config.service';
import { LiteralsService } from './services/literals.service';
import {
  provideRouter,
  withEnabledBlockingInitialNavigation,
  //withHashLocation,
  withInMemoryScrolling,
  withRouterConfig,
  withViewTransitions
} from '@angular/router';

import { DropdownModule, SidebarModule } from '@coreui/angular';
import { IconSetService } from '@coreui/icons-angular';
import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { BackendService } from './services/backend.service';
import { AuthService } from './services/auth.service';

export function configInit(configService: ConfigService, backendService: BackendService, 
  listeralsService: LiteralsService, authService: AuthService) {
  return () => {
    return configService.load()
      .then(() => 
      {
        backendService.configure();
        return authService.reloadAccessTokenInfo();
      })
      .then(() => 
      {
        return listeralsService.load();
      });
    };
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes,
      withRouterConfig({
        onSameUrlNavigation: 'reload'
      }),
      withInMemoryScrolling({
        scrollPositionRestoration: 'top',
        anchorScrolling: 'enabled'
      }),
      withEnabledBlockingInitialNavigation(),
      withViewTransitions(),
      //withHashLocation()
    ),
    provideHttpClient(),
    {
      provide: APP_INITIALIZER,
      useFactory: configInit,
      multi: true,
      deps: [ConfigService, BackendService, LiteralsService, AuthService]
    },
    importProvidersFrom(SidebarModule, DropdownModule),
    IconSetService,
    provideAnimations()
  ]
};
