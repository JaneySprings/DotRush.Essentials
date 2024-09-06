import { DotNetDebugConfigurationProvider } from './providers/dotnetDebugConfigurationProvider';
import { ContextMenuController } from './controllers/contextMenuController';
import { DebuggerController } from './controllers/debuggerController';
import { InteropController } from './controllers/interopController';
import * as res from './resources/constants';
import * as vscode from 'vscode';

export function activate(context: vscode.ExtensionContext) {
    InteropController.activate(context);
    ContextMenuController.activate(context);
    DebuggerController.activate(context);

    context.subscriptions.push(vscode.debug.registerDebugConfigurationProvider(res.debuggerVsdbgId, new DotNetDebugConfigurationProvider()));
}

export function deactivate() {
}