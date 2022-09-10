import { RecordInfo } from './codeRecord';

export class SaveChunk {
    constructor(
        public taskId: string,
        public saveTime: number,
        public code: string,
        public recordInfo: RecordInfo
    ) {}
}