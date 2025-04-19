import { Component, inject, OnInit } from '@angular/core';
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
export class CarrierListComponent implements OnInit {
  // These properties match the SearchCarriersQuery model in the backend
  sjpNumber: string | null = null;
  carrierCode: string | null = null;
  createDate: Date | null = null;
  status: string | null = null;
  
  masterSrv = inject(MasterService);
  searchResults: any[] = [];
  loading = false;

  ngOnInit(): void {
    // Optional: Load initial data or status options
  }

  onSubmit(): void {
    this.loading = true;
    
    // Create query object matching the SearchCarriersQuery model
    const query = {
      sjpNumber: this.sjpNumber,
      carrierCode: this.carrierCode,
      createDate: this.createDate, // Angular's Date will serialize to DateTime in backend
      status: this.status
    };

    // Call the service method to get search results
    this.masterSrv.getSearchResults(query).subscribe({
      next: (results) => {
        this.searchResults = results;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error searching carriers:', error);
        this.loading = false;
      }
    });
  }

  // Clear search form
  clearSearch(): void {
    this.sjpNumber = null;
    this.carrierCode = null;
    this.createDate = null;
    this.status = null;
  }
}