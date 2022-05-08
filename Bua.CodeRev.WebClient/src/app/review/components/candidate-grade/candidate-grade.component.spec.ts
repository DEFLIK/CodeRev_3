import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CandidateGradeComponent } from './candidate-grade.component';

describe('CandidateGradeComponent', () => {
  let component: CandidateGradeComponent;
  let fixture: ComponentFixture<CandidateGradeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CandidateGradeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CandidateGradeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
