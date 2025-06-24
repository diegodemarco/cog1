import { Component, ViewChild } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonDirective, CardBodyComponent, CardComponent, ColComponent,
         RowComponent, TextColorDirective } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { BasicEntitiesService } from '../../services/basic-entities.service';
import { ViewStatusService } from '../../services/view-status.service';
import { BackendService } from '../../services/backend.service';
import { AuthService } from '../../services/auth.service';
import { JsonControllerException, LiteralsContainerDTO, VariableDTO, VariableSource } from '../../api-client/data-contracts';
import { VariableEditModalComponent } from './modals/variable-edit-modal.component';
import { IconSubset } from '../../icons/icon-subset';
import { CrudPageComponent } from '../../shared/crud-page/crud-page.component'
import { Utils } from '../../utils';

@Component({
  templateUrl: 'variables.component.html',
  styleUrls: ['variables.component.scss'],
  standalone: true,
  imports: [TextColorDirective, CardComponent, CardBodyComponent, RowComponent,
    ColComponent, ButtonDirective, IconDirective, ReactiveFormsModule,
    VariableEditModalComponent, FormsModule, CrudPageComponent]
})
export class VariablesComponent
{
  // Template data
  readonly iconSubset = IconSubset;
  readonly variableSource = VariableSource;
  readonly literals: LiteralsContainerDTO;
  readonly authService: AuthService;
  readonly newItemLiteral: string = '';
  readonly basicEntitiesService: BasicEntitiesService;

  public variables: VariableDTO[] = [];
  @ViewChild(VariableEditModalComponent) editModal!: VariableEditModalComponent;
  @ViewChild(CrudPageComponent) crudPage!: CrudPageComponent;

  constructor(private backend: BackendService, private viewStatus: ViewStatusService, 
    basicEntitiesService: BasicEntitiesService, authService: AuthService) 
  {
    this.literals = basicEntitiesService.literals;
    this.authService = authService;
    this.basicEntitiesService = basicEntitiesService;
    viewStatus.setTitle(this.literals.variables!.variables!);
    if (authService.isAdmin)
      this.newItemLiteral = this.literals.variables!.newVariable!
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
