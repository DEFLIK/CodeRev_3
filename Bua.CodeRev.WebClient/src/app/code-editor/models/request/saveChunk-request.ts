import { ICodeRecord, RecordInfo } from '../codeRecord';
import { SaveChunk } from '../saveChunk';

export class SaveChunkRequest {
    public taskSolutionId: string;
    public saveTime: number;
    public code: string;
    public record: ICodeRecord[];

    constructor(model: SaveChunk) {
        this.taskSolutionId = model.taskId;
        this.saveTime = model.saveTime;
        this.code = model.code;
        this.record = model.recordInfo.record;
    }
}