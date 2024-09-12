import { DotNetDebugConfigurationProvider } from './providers/dotnetDebugConfigurationProvider';
import { ContextMenuController } from './controllers/contextMenuController';
import { DebuggerController } from './controllers/debuggerController';
import { InteropController } from './controllers/interopController';
import { DotNetTaskProvider } from './providers/dotnetTaskProvider';
import { ModulesView } from './features/modulesView';
import * as res from './resources/constants';
import * as vscode from 'vscode';

export function activate(context: vscode.ExtensionContext) {
    InteropController.activate(context);
    ContextMenuController.activate(context);
    DebuggerController.activate(context);

    ModulesView.feature.activate(context);

    context.subscriptions.push(vscode.tasks.registerTaskProvider(res.taskDefinitionId, new DotNetTaskProvider()));
    context.subscriptions.push(vscode.debug.registerDebugConfigurationProvider(res.debuggerVsdbgId, new DotNetDebugConfigurationProvider()));
}

export function deactivate() {
}