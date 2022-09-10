import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PatchedTimelineComponent } from './patched-timeline.component';

describe('PatchedTimelineComponent', () => {
  let component: PatchedTimelineComponent;
  let fixture: ComponentFixture<PatchedTimelineComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PatchedTimelineComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PatchedTimelineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
