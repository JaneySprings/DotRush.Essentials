import { DotNetConfigurationProvider } from './providers/dotnetConfigurationProvider';
import { DotNetTaskProvider } from './providers/dotnetTaskProvider';
import { CommandController } from './commandController';
import { DebuggerController } from './debuggerController';
import * as res from './resources/constants';
import * as vscode from 'vscode';

export function activate(context: vscode.ExtensionContext) {
    CommandController.activate(context);
    DebuggerController.activate(context);

    context.subscriptions.push(vscode.tasks.registerTaskProvider(res.taskDefinitionId, new DotNetTaskProvider()));
    context.subscriptions.push(vscode.debug.registerDebugConfigurationProvider(res.debuggerVsdbgId, new DotNetConfigurationProvider()));
}

export function deactivate() {
}