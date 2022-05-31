import { TestBed } from '@angular/core/testing';

import { CodeStorageService } from './code-storage.service';

describe('CodeStorageService', () => {
  let service: CodeStorageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CodeStorageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
