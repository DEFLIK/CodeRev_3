import { Component, OnInit } from '@angular/core';
import { ExecutionResult } from '../../models/executionResult';

@Component({
    selector: 'app-output',
    templateUrl: './output.component.html',
    styleUrls: ['./output.component.less']
})
export class OutputComponent {
    public success: boolean | undefined = undefined;
    public text: string[] = ['Добро пожаловать в CodeRev! Чтобы запустить программу нажмите "Запуск"'];

    constructor() { }

    public setOutput(result: ExecutionResult): void {
        this.success = result.success;
        if (this.success) {
            this.text = result.output;
        } else {
            this.text = result.errors.map(error => `${error.errorCode}: ${error.message}`);
        }
    }

}
