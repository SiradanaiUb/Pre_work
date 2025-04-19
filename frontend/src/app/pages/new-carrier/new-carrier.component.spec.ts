import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewCarrierComponent } from './new-carrier.component';

describe('NewCarrierComponent', () => {
  let component: NewCarrierComponent;
  let fixture: ComponentFixture<NewCarrierComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewCarrierComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NewCarrierComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
