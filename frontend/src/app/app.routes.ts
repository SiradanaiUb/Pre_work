import { Routes } from '@angular/router';
import { CarrierListComponent } from './pages/carrier-list/carrier-list.component';
import { NewCarrierComponent } from './pages/new-carrier/new-carrier.component';

export const routes: Routes = [

    {
        path: '',
        redirectTo: 'list',
        pathMatch: 'full'
    },
    {
        path: 'list',
        component: CarrierListComponent
    },
    {
        path: 'createNew',
        component: NewCarrierComponent
    }
];
