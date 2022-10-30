import { DraftCheckBox } from './draftCheckBox';
import { DraftText } from './draftText';

export class Draft {
    public maxPosition!: number;
    public texts!: DraftText[];
    public checkBoxes!: DraftCheckBox[];
}

export interface IDraftObject {
    position: number;
    value: string;
}