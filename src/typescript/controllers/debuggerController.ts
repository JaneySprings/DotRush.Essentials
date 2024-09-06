import { InteropController } from './interopController';
import * as res from '../resources/constants';
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
        let workspaceFolder : vscode.WorkspaceFolder | undefined;
        
        if (vscode.window.activeTextEditor?.document.fileName.endsWith(".cs"))
            workspaceFolder = vscode.workspace.getWorkspaceFolder(vscode.window.activeTextEditor.document.uri);
        if (vscode.workspace.workspaceFolders !== undefined)
            workspaceFolder ??= vscode.workspace.workspaceFolders[0];
        if (workspaceFolder === undefined)
            return undefined;

		const projectFiles = await vscode.workspace.findFiles(new vscode.RelativePattern(workspaceFolder, '*.csproj'));
		if (projectFiles.length === 0 || projectFiles.length > 1)
			return undefined;

        return projectFiles[0].fsPath;
    }
    public static async getProgramPath(): Promise<string | undefined> {
        const projectFile = await DebuggerController.getProjectFile();
        if (projectFile === undefined)
            return undefined;

		const programName = path.basename(projectFile, '.csproj') + '.dll';
		const programBasePath = path.join(path.dirname(projectFile), 'bin', 'Debug');
		const assemblyFiles = await vscode.workspace.findFiles(new vscode.RelativePattern(programBasePath, "**/*.dll"));
        const programFiles = assemblyFiles.filter(asm => path.basename(asm.fsPath).toLowerCase() === programName.toLowerCase())
		if (programFiles.length === 0 || programFiles.length > 1)
			return undefined;
		
		return programFiles[0].fsPath;
	}
}