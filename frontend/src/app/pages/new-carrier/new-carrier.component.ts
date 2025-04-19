import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzButtonModule } from 'ng-zorro-antd/button';  
import { MasterService } from '../../service/master.service'; // Adjust the import path as necessary

@Component({
  selector: 'app-new-carrier',
  imports: [FormsModule, NzFormModule, NzInputModule, NzButtonModule],
  templateUrl: './new-carrier.component.html',
  styleUrl: './new-carrier.component.scss'
})
export class NewCarrierComponent {

  newCarrierInfoList: any = {
    Carrier_Name: '',
  };
  carrierName: string = '';
  masterSrv= inject(MasterService);
  router = inject(Router);

  onSave() {
    const payload = {
      Carrier_Name: this.newCarrierInfoList.carrierName
    }
    console.log('Payload:', payload);
    this.masterSrv.createNewCarrier(payload).subscribe(
      (res:any) => {
        console.log('Response:', res);
        this.router.navigate(['/list']);
      },
      (error:any) => {
        console.error('Error:', error);
      }
    )
  }
}
