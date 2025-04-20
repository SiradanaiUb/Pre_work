import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MasterService {

  constructor(private http:HttpClient) { }

  createNewCarrier(obj: any) {
    return this.http.post('http://localhost:5146/api/Carrier/CreateCarrier',obj,{
      headers: {
        'Content-Type': 'application/json'
      }
    })
  }

  getSearchResults(query: any): Observable<any[]> {
    const params = new HttpParams()
      .set('sjpNumber', query.sjpNumber || '')
      .set('carrierCode', query.carrierCode || '')
      .set('createDate', query.createDate ? query.createDate.toISOString() : '')
      .set('status', query.status || '');
  
    return this.http.get<any[]>('http://localhost:5146/api/Carrier/SearchCarriers', { params });
  }

  getAllCarriers(): Observable<any[]> {
    return this.http.get<any[]>('http://localhost:5146/api/Carrier/GetAllCarriers');
  }
  

  editCarrier(obj: any) {
    return this.http.put('http://localhost:5146/api/Carrier/EditCarrier', obj, {
      headers: {
        'Content-Type': 'application/json'
      }
    });
  }

  DeleteCarrier(sjpNumber: string) {
    return this.http.delete(`http://localhost:5146/api/Carrier/DeleteCarrier/${sjpNumber}`, {
      headers: {
        'Content-Type': 'application/json'
      }
    });
  }

  ApproveCarrier(sjpNumber: string) {
    return this.http.patch(`http://localhost:5146/api/Carrier/ApproveCarrier/${sjpNumber}`, null, {
      headers: {
        'Content-Type': 'application/json'
      }
    });
  }

  RejectCarrier(sjpNumber: string) {
    return this.http.patch(`http://localhost:5146/api/Carrier/RejectCarrier/${sjpNumber}`, null, {
      headers: {
        'Content-Type': 'application/json'
      }
    });
  }
}
