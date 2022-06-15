import { ICodeRecord, RecordInfo } from '../codeRecord';
import { SaveChunk } from '../saveChunk';

export class SaveChunkResponse {
    public saveTime?: number;
    public code?: string;
    public records?: ICodeRecord[];
}