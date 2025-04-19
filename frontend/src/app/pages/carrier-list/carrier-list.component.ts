import { Component, inject, OnInit, Output, EventEmitter } from '@angular/core';
import { MasterService } from '../../service/master.service'; 
import { FormsModule } from '@angular/forms';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzTableModule } from 'ng-zorro-antd/table';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-carrier-list',
  imports: [FormsModule, NzFormModule, NzInputModule, NzButtonModule, NzDatePickerModule, NzSelectModule, NzTableModule, CommonModule],
  templateUrl: './carrier-list.component.html',
  styleUrl: './carrier-list.component.scss'
})
export class CarrierListComponent {
  sjpNumber: string | null = null;
  carrierCode: string | null = null;
  createDate: Date | null = null;
  status: string | null = null;
  masterSrv = inject(MasterService);

  searchResults: any[] = [];

  onSubmit(): void {
  }

  enteredSearchValue: string = '';

  @Output()
  searchTextChanged: EventEmitter<string> = new EventEmitter<string>();

  onsearchTextChanged(): void {
    this.searchTextChanged.emit(this.enteredSearchValue);
    console.log('Search text changed:', this.enteredSearchValue);
  }
}
