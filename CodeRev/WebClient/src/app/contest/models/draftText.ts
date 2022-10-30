import { IDraftObject } from './draft';

export class DraftText implements IDraftObject {
    public position!: number;
    public value!: string;
}