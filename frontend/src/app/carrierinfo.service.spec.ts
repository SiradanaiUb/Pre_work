import { TestBed } from '@angular/core/testing';

import { CarrierinfoService } from './carrierinfo.service';

describe('CarrierinfoService', () => {
  let service: CarrierinfoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CarrierinfoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
