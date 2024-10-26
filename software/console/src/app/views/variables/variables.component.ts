import { NgStyle } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonDirective, ButtonGroupComponent, CardBodyComponent, CardComponent,
  CardFooterComponent, CardHeaderComponent, ColComponent, DropdownComponent,
  DropdownDividerDirective, DropdownHeaderDirective, DropdownItemDirective,
  DropdownMenuDirective, DropdownToggleDirective, FormCheckLabelDirective,
  FormControlDirective,
  GutterDirective, ProgressBarDirective, ProgressComponent, RowComponent,
  TableDirective, TextColorDirective } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { WidgetsBrandComponent } from '../widgets/widgets-brand/widgets-brand.component';
import { WidgetsDropdownComponent } from '../widgets/widgets-dropdown/widgets-dropdown.component';

import { BasicEntitiesService } from '../../services/basic-entities.service';
import { ViewStatusService } from '../../services/view-status.service';
import { BackendService } from '../../services/backend.service';
import { JsonControllerException, LiteralsContainerDTO, VariableDTO } from '../../api-client/data-contracts';

import { VariableEditModalComponent } from './modals/variable-edit-modal.component';
import { IconSubset } from 'src/app/icons/icon-subset';
import { CrudPageComponent } from '../../shared/crud-page/crud-page.component'
import { Utils } from '../../utils';
import { AuthService } from 'src/app/services/auth.service';


@Component({
  templateUrl: 'variables.component.html',
  styleUrls: ['variables.component.scss'],
  standalone: true,
  imports: [WidgetsDropdownComponent, TextColorDirective, CardComponent, CardBodyComponent, RowComponent,
    ColComponent, ButtonDirective, IconDirective, ReactiveFormsModule, ButtonGroupComponent,
    FormCheckLabelDirective, NgStyle, CardFooterComponent, GutterDirective,
    ProgressBarDirective, ProgressComponent, WidgetsBrandComponent, CardHeaderComponent, TableDirective,
    DropdownComponent, ButtonDirective, DropdownToggleDirective, DropdownMenuDirective,
    DropdownHeaderDirective, DropdownItemDirective, RouterLink, DropdownDividerDirective,
    FormControlDirective, VariableEditModalComponent, FormsModule, CrudPageComponent]
})
export class VariablesComponent
{
  // Template data
  readonly iconSubset = IconSubset;
  readonly literals: LiteralsContainerDTO;
  readonly authService: AuthService;

  public variables: VariableDTO[] = [];
  @ViewChild(VariableEditModalComponent) editModal!: VariableEditModalComponent;
  @ViewChild(CrudPageComponent) crudPage!: CrudPageComponent;

  constructor(private backend: BackendService, private viewStatus: ViewStatusService, 
    basicEntitiesService: BasicEntitiesService, authService: AuthService) 
  {
    this.literals = basicEntitiesService.literals;
    this.authService = authService;
    viewStatus.setTitle(this.literals.variables!.variables!);
  }

  doNewVariable()
  {
    this.editModal.showModal(null)
      .then(() => {
        this.viewStatus.showSuccessToast(this.literals.variables!.variableCreated!);
        this.crudPage.searchFilter = '';
        this.doUpdateData();
      })
      .catch(() => {});
  }

  doEditVariable(v: VariableDTO)
  {
    this.editModal.showModal(v)
      .then(() => {
        this.viewStatus.showSuccessToast(this.literals.variables!.variableUpdated!);
        this.doUpdateData();
      })
      .catch(() => {});
  }

  doDeleteVariable(v: VariableDTO)
  {
    this.viewStatus.showWarningModal(
      this.literals.variables!.deleteVariable!, 
      this.literals.variables!.deleteVariableConfirmation?.replace('{0}', v.description!)!)
      .then(() => {
        this.backend.variables.deleteVariable(v.variableId!)
          .then(() =>
          {
            this.viewStatus.showSuccessToast(this.literals.variables!.variableDeleted!);
            this.doUpdateData();
          })
          .catch(error => {
            let e: JsonControllerException = error.error;
            this.viewStatus.showErrorToast(e.message!);
          });
        })
      .catch(() => { });
  }

  doUpdateData()
  {
    this.backend.variables.enumerateVariables()
      .then(data =>
      {
        this.variables = data.data.filter(item =>
          Utils.matchesFilter(this.crudPage.searchFilter, item.description, item.variableCode, item.variableId!.toString())
        );
      });
  }

}
