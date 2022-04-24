import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { ExecutionResult } from '../../models/executionResult';

@Injectable({
    providedIn: 'root'
})
export class CodeStorageService {
    public onOutputRefresh$: Subject<ExecutionResult> = new Subject<ExecutionResult>();
    public get defaultCode(): string {
        return this._defaultCode;
    }
    public get entryNamespace(): string {
        return this._entryNamespace;
    }
    public get entryClass(): string {
        return this._entryClass;
    }
    public get entryMethod(): string {
        return this._entryMethod;
    }
    private _entryNamespace: string = 'CodeRev';
    private _entryClass: string = 'Program';
    private _entryMethod: string ='Main';
    private _outputLines: ExecutionResult = new ExecutionResult();
    private _defaultCode: string = `using System;
using System.Collections.Generic;

namespace CodeRev
{
    public class Program
    {
        public static void Main() {
            Console.WriteLine("Hello bobiks!");
        }
    }
}`;
    private _codeKey = 'code';
    private _recordKey = 'record';
    constructor() { }

    public storeOutput(outputLines: ExecutionResult): void {
        this._outputLines = outputLines;
        this.onOutputRefresh$.next(outputLines);
    }

    public saveCode(code: string): void {
        localStorage.setItem(this._codeKey, code);
    }
    
    public saveRecord(record: string): void {
        localStorage.setItem(this._recordKey, record);
    }

    public getSavedCode(): string {
        return localStorage.getItem(this._codeKey) ?? '';
    }

    public getSavedRecord(): string | null {
        return localStorage.getItem(this._recordKey);
    }

    public clearSavedCode(): void {
        localStorage.removeItem(this._codeKey);
    }

    public clearSavedRecord(): void {
        localStorage.removeItem(this._recordKey);
    }
}
