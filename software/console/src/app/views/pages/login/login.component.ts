import { Component, inject } from '@angular/core';
import { NgStyle } from '@angular/common';
import { IconDirective } from '@coreui/icons-angular';
import { ContainerComponent, RowComponent, ColComponent, CardGroupComponent,
  TextColorDirective, CardComponent, CardBodyComponent, FormDirective, 
  InputGroupComponent, InputGroupTextDirective, FormControlDirective, ButtonDirective,
  FormCheckComponent, AlertComponent } from '@coreui/angular';
import { BackendService } from '../../../services/backend.service';
import { AuthService } from '../../../services/auth.service';
import { FormsModule } from '@angular/forms';
import { BasicEntitiesService } from 'src/app/services/basic-entities.service';
import { Router } from '@angular/router';
import { JsonControllerException, LiteralsContainerDTO } from 'src/app/api-client/data-contracts';
import { ViewStatusService } from 'src/app/services/view-status.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
    standalone: true,
    imports: [ContainerComponent, RowComponent, ColComponent, CardGroupComponent, TextColorDirective, CardComponent,
       CardBodyComponent, FormDirective, InputGroupComponent, InputGroupTextDirective, IconDirective, FormControlDirective,
        ButtonDirective, NgStyle, FormCheckComponent, FormsModule, AlertComponent]
})
export class LoginComponent {

  username: string = "";
  password: string = "";
  persist: boolean = false;
  alertVisible: boolean = false;
  alertMessage: string = "";
  readonly literals: LiteralsContainerDTO;

  constructor(private backend: BackendService, private authService: AuthService, 
    private router: Router, private basicEntitiesService: BasicEntitiesService) 
  { 
    this.literals = basicEntitiesService.literals;
  }

  doLogin()
  {
    this.backend.security.login({ userName: this.username, password: this.password })
    .then(result => 
    {
      // Store the received credentials
      this.authService.storeCredentials(result.data.token!, this.persist);

      // Reload the information related to the received access token
      this.authService.reloadAccessTokenInfo().then(() =>
      {
        // Reload literals (language may have changed)
        this.basicEntitiesService.load().then(() =>
        {
          // Navigate to the home page
          this.router.navigate(["/"]); 
        });
      });      
    })
    .catch(error =>
    {
      let e: JsonControllerException = error.error;
      this.alertMessage = e.message!;
      this.alertVisible = true;
    });
  }

  doDataChanged()
  {
    this.alertVisible = false;
  }

}
