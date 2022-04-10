import { TestBed } from '@angular/core/testing';

import { CompileService } from './compile-service.service';

describe('CompileServiceService', () => {
    let service: CompileService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(CompileService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
