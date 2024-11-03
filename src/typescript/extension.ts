import { DotNetDebugConfigurationProvider } from './providers/dotnetDebugConfigurationProvider';
import { ContextMenuController } from './controllers/contextMenuController';
import { DebuggerController } from './controllers/debuggerController';
import { TestController } from './controllers/testController';
import { DotNetTaskProvider } from './providers/dotnetTaskProvider';
import { ModulesView } from './features/modulesView';
import { Interop } from './interop/interop';
import * as res from './resources/constants';
import * as vscode from 'vscode';

export function activate(context: vscode.ExtensionContext) {
    Interop.init(context.extensionPath);

    ContextMenuController.activate(context);
    DebuggerController.activate(context);
    TestController.activate(context);

    ModulesView.feature.activate(context);

    context.subscriptions.push(vscode.tasks.registerTaskProvider(res.taskDefinitionId, new DotNetTaskProvider()));
    context.subscriptions.push(vscode.debug.registerDebugConfigurationProvider(res.debuggerVsdbgId, new DotNetDebugConfigurationProvider()));
}

export function deactivate() {
}