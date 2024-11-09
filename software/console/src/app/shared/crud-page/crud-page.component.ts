import { NgStyle } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonDirective, ButtonGroupComponent, CardBodyComponent, CardComponent,
  CardFooterComponent, CardHeaderComponent, ColComponent, DropdownComponent,
  DropdownDividerDirective, DropdownHeaderDirective, DropdownItemDirective,
  DropdownMenuDirective, DropdownToggleDirective, FormCheckLabelDirective,
  FormControlDirective,
  GutterDirective, ProgressBarDirective, ProgressComponent, RowComponent,
  TableDirective, TextColorDirective } from '@coreui/angular';
import { IconDirective } from '@coreui/icons-angular';
import { WidgetsBrandComponent } from '../../views/widgets/widgets-brand/widgets-brand.component';

import { BasicEntitiesService } from '../../services/basic-entities.service';
import { LiteralsContainerDTO, VariableDTO } from '../../api-client/data-contracts';

import { IconSubset } from '../../icons/icon-subset';
import { Subscription, timer } from 'rxjs';

@Component({
  selector: 'app-crud-page',
  templateUrl: 'crud-page.component.html',
  styleUrls: ['crud-page.component.scss'],
  standalone: true,
  imports: [CardComponent, CardBodyComponent, RowComponent, ColComponent, ButtonDirective,
    IconDirective, ButtonGroupComponent, CardFooterComponent, CardHeaderComponent,  
    FormControlDirective, FormsModule]
})
export class CrudPageComponent implements OnInit, OnDestroy
{
  // Template data
  iconSubset = IconSubset;
  literals: LiteralsContainerDTO;

  @Input({ required: true }) newItemLiteral?: string | null;
  @Input({ required: false }) searchFilter: string | null = '';
  @Output() onUpdateData = new EventEmitter<void>();
  @Output() onNewItem = new EventEmitter<void>();

  private searchSub: Subscription | null = null;

  constructor(basicEntitiesService: BasicEntitiesService)
  {
    this.literals = basicEntitiesService.literals;
  }

  ngOnInit(): void {
    this.searchFilter = '';
    this.onUpdateData.emit();
  }

  ngOnDestroy(): void {
    this.killSearchSub();
  }

  private killSearchSub() {
    if (this.searchSub) {
      this.searchSub.unsubscribe();
      this.searchSub = null;
    }
  }

  startSearchSub() {
    this.killSearchSub();
    this.searchSub = timer(500).subscribe(() => 
      {
        this.killSearchSub();
        this.onUpdateData.emit();
      } );
  }

  doNewItem()
  {
    this.onNewItem.emit();
  }

  doClearSearch()
  {
    this.searchFilter = "";
    this.onUpdateData.emit();
  }

}
