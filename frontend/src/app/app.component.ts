import { Component,inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CarrierinfoService } from './carrierinfo.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
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
