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
import { NzPaginationModule } from 'ng-zorro-antd/pagination';

interface CarrierDto {
  sjpNumber: string;
  carrier_Code: string;
  create_Date: Date;
  status: string;
}

@Component({
  selector: 'app-carrier-list',
  imports: [FormsModule, NzFormModule, NzInputModule, NzButtonModule, NzDatePickerModule, NzSelectModule, NzTableModule, CommonModule, NzPaginationModule],
  templateUrl: './carrier-list.component.html',
  styleUrl: './carrier-list.component.scss',
  standalone: true
})
export class CarrierListComponent implements OnInit {
  // These properties match the SearchCarriersQuery model in the backend
  sjpNumber: string | null = null;
  carrierCode: string | null = null;
  createDate: string | null = null;
  status: string | null = null;
  
  masterSrv = inject(MasterService);
  searchResults: CarrierDto[] = [];
  loading = false;
  pageIndex = 1;
  pageSize = 10;

  ngOnInit(): void {
    this.loading = true;
    this.masterSrv.getAllCarriers().subscribe({
      next: (results) => {
        this.searchResults = results;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading carriers:', error);
        this.loading = false;
      }
    });
  }
  

  onSubmit(): void {
    this.loading = true;
  
    // Check if all search fields are empty
    const isEmpty =
      !this.sjpNumber &&
      !this.carrierCode &&
      !this.createDate &&
      !this.status;
  
    if (isEmpty) {
      // Call GetAllCarriers when no filters are provided
      this.masterSrv.getAllCarriers().subscribe({
        next: (results) => {
          this.searchResults = results;
          this.loading = false;
        },
        error: (error) => {
          console.error('Error fetching all carriers:', error);
          this.loading = false;
          this.searchResults = [];
        }
      });
    } else {
      // Prepare search query object
      const query = {
        sjpNumber: this.sjpNumber,
        carrierCode: this.carrierCode,
        createDate: this.createDate,
        status: this.status
      };
  
      this.masterSrv.getSearchResults(query).subscribe({
        next: (results) => {
          this.searchResults = results;
          this.loading = false;
        },
        error: (error) => {
          console.error('Error searching carriers:', error);
          this.loading = false;
          this.searchResults = [];
        }
      });
    }
  }
  

  // Clear search form
  clearSearch(): void {
    this.sjpNumber = null;
    this.carrierCode = null;
    this.createDate = null;
    this.status = null;
    this.searchResults = [];
  }

  get pagedResults() {
    const start = (this.pageIndex - 1) * this.pageSize;
      return this.searchResults.slice(start, start + this.pageSize);
  }

  onPageChange(page: number): void {
    this.pageIndex = page;
  }
  
}