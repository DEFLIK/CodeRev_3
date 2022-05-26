import { TestBed } from '@angular/core/testing';

import { TimelinePatcherService } from './timeline-patcher.service';

describe('TimelinePatcherService', () => {
  let service: TimelinePatcherService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TimelinePatcherService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
