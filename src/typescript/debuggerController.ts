import { CommandController } from './commandController';
import * as res from './resources/constants';
import * as vscode from 'vscode';
import * as path from 'path';
import * as fs from 'fs';

export class DebuggerController {
    public static async activate(context: vscode.ExtensionContext) : Promise<void> {
        if (!fs.existsSync(path.join(context.extensionPath, 'extension', 'debugger'))) {
            vscode.window.withProgress({
                location: vscode.ProgressLocation.Notification,
                title: res.messageInstallingDebuggerTitle,
                cancellable: false
            }, () => new Promise<void>(async (resolve) => {
                const operationResult = await CommandController.installVsdbg();
                if (!operationResult.isSucceded) {
                    vscode.window.showErrorMessage(res.messageInstallingDebuggerFailed, { 
                        detail: operationResult.message,
                        modal: true,
                    });
                }
                resolve();
            }));
        }
    }
}