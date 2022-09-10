/* eslint-disable max-classes-per-file */
export interface ICodeOperation<T = void> {
    o: OperationType, 
    i: number[] | number[][], 
    a: string | string[] | string[][],
    activity?: ExtraActivity<T>
}
export type OperationType = 'c' | 'd' | 'i' | 'k' | 'l' | 'm' | 'n' | 'o' | 'p' | 'r' | 's' | 'x' | 'e';
export interface ICodeRecord {
    o: ICodeOperation[],
    t: number | number[],
    l: number
}
export enum OperationColors {
    'p' = '#FDD116',
    'e' = '#DD4B39',
    'exec' = '#009460'
}
export enum ExtraActions {
    pageOpened = 'open',
    pageHidden = 'hidden',
    execute = 'execute'
}
export interface IOperationMark {
    startTime: number,
    endTime: number,
    color: OperationColors
}
export class ExtraActivity<T = void> {
    constructor(
        public action: ExtraActions,
        public data?: T 
    ) {}
}

export class RecordInfo {
    public readonly points: IOperationMark[] = [];
    public readonly record: ICodeRecord[];
    public readonly recordStartTime: number;
    public readonly duration: number;
    private _importantOperations: Set<OperationType> = new Set(['p']);

    constructor(records: ICodeRecord[], saveTime: number) {
        this.recordStartTime = saveTime;
        this.record = records;

        if (records.length === 0) {
            this.duration = 0;
            
            return;
        }

        switch(typeof(records[records.length - 1].t)) {
            case('number'):
                this.duration = records[records.length - 1].t as number;
                break;
            default:
                this.duration = (records[records.length - 1].t as number[])[1];
        }

        this.recordStartTime -= this.duration;
        let lastHideTime = 0;
        for (const record of records) {
            for (const operation of record.o) {
                if (operation.o === 'e') {
                    switch (operation.activity?.action) {
                        case (ExtraActions.pageHidden):
                            lastHideTime = record.t as number;
                            break;
                        case (ExtraActions.pageOpened):
                            this.points.push({
                                startTime: lastHideTime,
                                endTime: record.t as number,
                                color: OperationColors.e
                            });
                            break;
                        case (ExtraActions.execute):
                            this.points.push({
                                startTime: record.t as number,
                                endTime: record.t as number,
                                color: OperationColors.exec
                            });
                            break;
                    }   
                }

                if (this._importantOperations.has(operation.o)) { // o - operation -> o - opeartion type
                    let start = 0;
                    let end = 0;
                    switch(typeof record.t) {
                        case('number'):
                            start = record.t as number;
                            end = record.t as number;
                            break;
                        default:
                            start = (record.t as number[])[0];
                            end = (record.t as number[])[1];
                    }
                    this.points.push({ startTime: start, endTime: end, color: OperationColors[operation.o as keyof typeof OperationColors] });
                }
            }
        }    
    }
}