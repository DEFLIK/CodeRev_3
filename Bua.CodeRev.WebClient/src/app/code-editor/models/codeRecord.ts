export interface ICodeOperation {
    o: OperationType, 
    i: number[] | number[][], 
    a: string | string[] | string[][]
}
export type OperationType = 'c' | 'd' | 'i' | 'k' | 'l' | 'm' | 'n' | 'o' | 'p' | 'r' | 's' | 'x' | 'e';
export interface ICodeRecord {
    o: ICodeOperation[],
    t: number | number[],
    l: number
}
export enum OperationColors {
    'p' = 'red',
    'x' = 'yellow',
}
export interface IOperationMark {
    startTime: number,
    endTime: number,
    color: OperationColors
}


export class RecordInfo {
    public readonly points: IOperationMark[] = [];
    private _importantOperations: Set<OperationType> = new Set(['p', 'x']);

    constructor(records: ICodeRecord[]) {
        for (const record of records) {
            for (const operation of record.o) {
                if (this._importantOperations.has(operation.o)) { // o - operation -> o - opeartion type
                    let start = 0;
                    let end = 10;
                    switch(typeof record.t) {
                        case('number'):
                            start = record.t as number;
                            end = record.t as number + 10;
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