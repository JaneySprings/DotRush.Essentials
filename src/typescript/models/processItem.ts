import * as vscode from 'vscode';

export class ProcessItem implements vscode.QuickPickItem {
    label: string;
    description?: string;
    pid: number;

    constructor(process: Process) {
        this.label = process.name ?? 'unknown';
        this.description = `pid: ${process.id}`;
        this.pid = process.id;
    }
}

export interface Process  {
    id: number;
    name?: string;
    fullPath?: string;
}