import { Injectable } from '@angular/core';
import { HttpService } from 'src/app/global-services/request/http.service';
import { RequestMethodType } from 'src/app/global-services/request/models/request-method';
import { UrlRoutes } from 'src/app/global-services/request/models/url-routes';
import { RecordInfo } from '../../models/codeRecord';
import { SaveChunkRequest } from '../../models/request/saveChunk-request';
import { SaveChunkResponse } from '../../models/response/saveChunk-response';
import { SaveChunk } from '../../models/saveChunk';
// import { CodeStorageService as StorageService } from '../storage-service/code-storage.service';

@Injectable()
export class SavingService {
    constructor(private _http: HttpService) {}

    public saveNext(taskId: string, code: string, recordInfo: RecordInfo): void {
        const time = Date.now();
        const nextChunk = new SaveChunk(taskId, time, code, recordInfo);

        localStorage.setItem('code' + taskId, code);

        this._http
            .request<void, SaveChunkRequest>({
                url: `${UrlRoutes.tracker}/api/v1.0/tracker/save?taskSolutionId=${taskId}`,
                method: RequestMethodType.put,
                auth: true,
                body: new SaveChunkRequest(nextChunk)
            })
            .subscribe(resp => {
                if (!resp.ok) {
                    console.error('Record backend saving error!');
                }
            });
    }

    public getTaskSaves(taskId: string): SaveChunk[] {
        const res: SaveChunk[] = [];

        for (const key in localStorage) {
            if (key.includes(taskId) && key.includes('save')) {
                const save = localStorage.getItem(key);
                if (save) {
                    res.push(JSON.parse(save) as SaveChunk);
                }
            }
        }

        const sorted = res.sort((a, b) => a.saveTime - b.saveTime);
        
        return sorted;
    }

    public getLastSavedCode(taskId: string): string | null {
        return localStorage.getItem('code' + taskId);
    }

    public setSavedCode(taskId: string, code: string): void {
        localStorage.setItem('code' + taskId, code);
    }

    public applySaves(taskId: string, saves: SaveChunkResponse[]): void {
        for (const save of saves) {
            const newSaveModel = new SaveChunk(
                taskId ?? '',
                save.saveTime ?? 0, 
                save.code ?? '', 
                new RecordInfo(save.records ?? [], save.saveTime ?? 0));
            localStorage.setItem('save' + taskId + save.saveTime, JSON.stringify(newSaveModel));
        }
    }

    public clearSaves(): void {
        for (const key in localStorage) {
            if (key.includes('save')) {
                localStorage.removeItem(key);
            }
        }
    }

    public clearCodes(): void {
        for (const key in localStorage) {
            if (key.includes('code')) {
                localStorage.removeItem(key);
            }
        }
    }
}
