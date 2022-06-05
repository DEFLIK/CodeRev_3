import { Injectable } from '@angular/core';
import { RecordInfo } from '../../models/codeRecord';
import { SaveChunk } from '../../models/saveChunk';
// import { CodeStorageService as StorageService } from '../storage-service/code-storage.service';

@Injectable({
    providedIn: 'root'
})
export class SavingService {
    constructor() { }

    public saveNext(taskId: string, code: string, record: RecordInfo): void {
        const time = Date.now();
        const nextChunk = new SaveChunk(taskId, time, code, record);

        localStorage.setItem('save' + taskId + time, JSON.stringify(nextChunk));

        // send saved chunk to backend
    }

    public getTaskSaves(taskId: string): SaveChunk[] {
        const res: SaveChunk[] = [];

        for (const key in localStorage) {
            if (key.includes(taskId)) {
                const save = localStorage.getItem(key);
                if (save) {
                    res.push(JSON.parse(save) as SaveChunk);
                }
            }
        }

        const sorted = res.sort((a, b) => a.saveTime - b.saveTime);
        console.log('sorted:', sorted);
        
        return sorted;
    }

    public getLastSave(taskId: string): SaveChunk { // Observable<SaveChunkResponse>
        // get from backend
        const res = this.getTaskSaves(taskId);

        return res[res.length - 1];
    }
}
