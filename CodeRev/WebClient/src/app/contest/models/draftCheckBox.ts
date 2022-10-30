import { IDraftObject } from './draft';

export class DraftCheckBox implements IDraftObject {
    public position!: number;
    public value!: string;
    public isChecked!: boolean;
}