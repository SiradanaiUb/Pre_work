import { Component,inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CarrierinfoService } from './services/CarrierInfo/carrierinfo.service';
import {NzTableModule} from 'ng-zorro-antd/table';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,NzTableModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'frontend';
  carrierInfos: any[] = [];

  carrierInfoService = inject(CarrierinfoService);

  constructor() {
    this.carrierInfoService.get().subscribe(carrierInfos => {
      this.carrierInfos = carrierInfos;
    });
  }
}
