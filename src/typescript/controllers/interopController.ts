import { ProcessArgumentBuilder } from './../interop/processArgumentBuilder';
import { ProcessRunner } from './../interop/processRunner';
import { OperationStatus } from './../models/operationStatus';
import * as vscode from 'vscode';
import * as path from 'path';

export class InteropController {
    private static workspaceToolPath: string;

    public static activate(context: vscode.ExtensionContext) {
        InteropController.workspaceToolPath = path.join(context.extensionPath, "extension", "bin", "DotRush.Essentials.dll");
    }

    public static async installVsdbg(): Promise<OperationStatus> {
        return await ProcessRunner.runAsync<OperationStatus>(new ProcessArgumentBuilder("dotnet")
            .appendQuoted(InteropController.workspaceToolPath)
            .append("--install-vsdbg"));
    }
}
