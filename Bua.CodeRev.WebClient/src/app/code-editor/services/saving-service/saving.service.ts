import { Injectable } from '@angular/core';
import { HttpService } from 'src/app/global-services/request/http.service';
import { RequestMethodType } from 'src/app/global-services/request/models/request-method';
import { UrlRoutes } from 'src/app/global-services/request/models/url-routes';
import { RecordInfo } from '../../models/codeRecord';
import { SaveChunkRequest } from '../../models/request/saveChunk-request';
import { SaveChunk } from '../../models/saveChunk';
// import { CodeStorageService as StorageService } from '../storage-service/code-storage.service';

@Injectable()
export class SavingService {
    constructor(private _http: HttpService) {}

    public saveNext(taskId: string, code: string, recordInfo: RecordInfo): void {
        const time = Date.now();
        const nextChunk = new SaveChunk(taskId, time, code, recordInfo);

        localStorage.setItem('save' + taskId + time, JSON.stringify(nextChunk));

        // this._http
        //     .request<void, SaveChunkRequest>({
        //         url: `${UrlRoutes.tracker}/api/v1/tracker/save?taskSolutionId=${taskId}`,
        //         method: RequestMethodType.put,
        //         auth: true,
        //         body: new SaveChunkRequest(nextChunk)
        //     })
        //     .subscribe(resp => {
        //         if (!resp.ok) {
        //             console.error('Record backend saving error!');
        //         }
        //     });
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
        
        return sorted;
    }

    public getLastSave(taskId: string): SaveChunk { // Observable<SaveChunkResponse>
        // get from backend
        const res = this.getTaskSaves(taskId);

        return res[res.length - 1];
    }

    public applySaves(saves: SaveChunk[]): void {
        this.clearSaves();

        for (const save of saves) {
            localStorage.setItem('save' + save.taskId + save.saveTime, JSON.stringify(save));
        }
    }

    public clearSaves(): void {
        for (const key in localStorage) {
            if (key.includes('save')) {
                localStorage.removeItem(key);
            }
        }
    }
}
