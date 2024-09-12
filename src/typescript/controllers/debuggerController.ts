import { InteropController } from './interopController';
import { ProcessItem } from '../models/processItem';
import * as res from '../resources/constants';
import * as vscode from 'vscode';
import * as path from 'path';
import * as fs from 'fs';

export class DebuggerController {
    public static async activate(context: vscode.ExtensionContext) : Promise<void> {
        context.subscriptions.push(vscode.commands.registerCommand(res.commandIdPickProcess, async () => await DebuggerController.pickProcessId()));

        if (!fs.existsSync(path.join(context.extensionPath, 'extension', 'debugger'))) {
            vscode.window.withProgress({
                location: vscode.ProgressLocation.Notification,
                title: res.messageInstallingDebuggerTitle,
                cancellable: false
            }, () => new Promise<void>(async (resolve) => {
                const operationResult = await InteropController.installVsdbg();
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

    public static async getProjectFile(): Promise<string | undefined> {
        if (vscode.window.activeTextEditor?.document === undefined) {
            const projectFiles = await vscode.workspace.findFiles('*.csproj');
            return projectFiles.length === 1 ? projectFiles[0].fsPath : undefined;
        }

        return DebuggerController.findProjectFile(path.dirname(vscode.window.activeTextEditor.document.fileName));
    }
    public static async getProgramPath(): Promise<string | undefined> {
        const projectFile = await DebuggerController.getProjectFile();
        if (projectFile === undefined)
            return await DebuggerController.pickProgramPath();

        const programFile = await InteropController.getPropertyValue('TargetPath', projectFile);
		if (!programFile)
			return await DebuggerController.pickProgramPath();
		
		return programFile;
	}
    
    public static async pickProgramPath(): Promise<string | undefined> {
        const programPath = await vscode.window.showOpenDialog({
            title: res.messageSelectProgramTitle,
            canSelectFiles: true,
            canSelectFolders: false,
            canSelectMany: false
        });
        return programPath?.[0].fsPath;
    }
    public static async pickProcessId(): Promise<string | undefined> {
        const processes = await InteropController.getProcesses();
        const selectedItem = await vscode.window.showQuickPick(processes.map(p => new ProcessItem(p)), { placeHolder: res.messageSelectProcessTitle });
        return selectedItem?.pid?.toString();
    }

    private static async findProjectFile(directory: string): Promise<string | undefined> {
        const projectFiles = await vscode.workspace.findFiles(new vscode.RelativePattern(directory, '*.csproj'));
		if (projectFiles.length > 1)
			return undefined;
        if (projectFiles.length === 1)
            return projectFiles[0].fsPath;
        
        const parentDirectory = path.dirname(directory);
        if (parentDirectory === directory)
            return undefined;

        return DebuggerController.findProjectFile(parentDirectory);
    }
}