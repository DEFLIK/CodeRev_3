import { Component, Input, OnDestroy } from '@angular/core';
import { ExecutionResult } from '../../models/executionResult';
import { TestsRunnerService } from '../../services/tests-runner-service/tests-runner.service';
import { CodemirrorComponent } from '@ctrl/ngx-codemirror';
import { ContestService } from 'src/app/contest/services/contest.service';
import { Subject, takeUntil } from 'rxjs';
import { TestsRunResponse } from '../../models/response/testsRun-response';

@Component({
    selector: 'app-toolbar',
    templateUrl: './toolbar.component.html',
    styleUrls: ['./toolbar.component.less']
})
export class ToolbarComponent implements OnDestroy {
    @Input()
    public isTestsRunnable!: boolean;
    public isCompilationSuccess: boolean | undefined = undefined;
    public isTestsSuccess: boolean | undefined = undefined;
    public selectedBar: string = 'console';
    public consoleOutput: string[] = ['Добро пожаловать в CodeRev! Чтобы запустить программу нажмите "Запуск"'];
    public testsRunResult?: TestsRunResponse;
    public testInfo?: string;
    public isTestInfoSuccess?: boolean;
    public isRunningTests: boolean = false;
    private _bindedEditor?: CodemirrorComponent;
    private _unsubscriber = new Subject<void>();

    constructor(public contest: ContestService, private _testRunner: TestsRunnerService) {
        this._testRunner.onOutputRefresh$
            .pipe(takeUntil(this._unsubscriber))
            .subscribe(result => {
                this.selectBar('tests');
                this.isRunningTests = false;
                if (!result.isCompiledSuccessfully) {
                    this.isTestsSuccess = false;
                    this.testsRunResult = new TestsRunResponse();
                    this.testsRunResult.failedTestCases = new Map<string,string>();
                    this.testsRunResult.failedTestCases.set('Ошибка компиляции', 'Не удалось скомпилировать текущий код');

                    return;
                }

                this.isTestsSuccess = this.testsRunResult?.failedTestCases?.size === 0 ?? true;
                this.testsRunResult = result;
            });
    }

    public ngOnDestroy(): void {
        this._unsubscriber.next();
    }

    public setConsoleOutput(result: ExecutionResult): void {
        this.isCompilationSuccess = result.success;
        if (this.isCompilationSuccess) {
            this.consoleOutput = result.output;
        } else {
            this.selectBar('console');
            this.consoleOutput = result.errors.map(error => `${error.errorCode}: ${error.message}`);
        }
    }

    public selectBar(barName: string): void {
        this.selectedBar = barName;
    }

    public runTests(): void {
        if (!this.contest.currentTask || this.contest.currentTask.runAttemptsLeft === 0) {
            return;
        }

        this.isRunningTests = true;
        var code = this._bindedEditor?.codeMirror?.getValue() ?? '';
        this._testRunner.run(code, this.contest.currentTask.id ?? 'none');

        this._testRunner
            .reduceTaskSolutionAttempt(this.contest.currentTask.id)
            .subscribe({
                next: (resp) => {
                    if (resp.ok && resp.body) {
                        this.contest.currentTask!.runAttemptsLeft = resp.body.runAttemptsLeft!;
                }
            }
        });
    }

    public bindToEditor(editor: CodemirrorComponent): void {
        this._bindedEditor = editor;
    }

    public setTestInfo(testName:string, message: string, isSuccess: boolean): void {
        this.testInfo = `[${testName}]: ${message}`;
        this.isTestInfoSuccess = isSuccess;
    }

}
