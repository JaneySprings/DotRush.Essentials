import { DebuggerController } from '../controllers/debuggerController';
import { ProcessArgumentBuilder } from '../interop/processArgumentBuilder';
import * as res from '../resources/constants';
import * as vscode from 'vscode';

export class DotNetTaskProvider implements vscode.TaskProvider {
    resolveTask(task: vscode.Task, token: vscode.CancellationToken): vscode.ProviderResult<vscode.Task> { 
        return DebuggerController.getProjectFile().then((path: string | undefined) => {
            return path !== undefined 
                ? DotNetTaskProvider.getTask(task.definition, path)
                : undefined;
        })
    }
    provideTasks(token: vscode.CancellationToken): vscode.ProviderResult<vscode.Task[]> {
        return DebuggerController.getProjectFile().then((path: string | undefined) => {
            return path !== undefined
                ? [DotNetTaskProvider.getTask({ type: res.taskDefinitionId }, path)]
                : undefined;
        });
    }

    public static async getTestTaskAsync(projectFile: vscode.Uri, ...args: string[]): Promise<vscode.Task> {
        return await DotNetTaskProvider.getTaskAsync(projectFile, 'test', ...args);
    }
    public static async getBuildTaskAsync(projectFile: vscode.Uri, ...args: string[]): Promise<vscode.Task> {
        return await DotNetTaskProvider.getTaskAsync(projectFile, 'build', ...args);
    }
    public static async getRestoreTaskAsync(projectFile: vscode.Uri, ...args: string[]): Promise<vscode.Task> {
        return await DotNetTaskProvider.getTaskAsync(projectFile, 'restore', ...args);
    }
    public static async getCleanTaskAsync(projectFile: vscode.Uri, ...args: string[]): Promise<vscode.Task> {
        return await DotNetTaskProvider.getTaskAsync(projectFile, 'clean', ...args);
    }

    private static getTask(definition: vscode.TaskDefinition, projectPath: string): vscode.Task {
        const builder = new ProcessArgumentBuilder('dotnet')
            .append('build')
            .append(projectPath)

        definition.args?.forEach((arg: string) => builder.override(arg));
        
        return new vscode.Task(
            definition, 
            vscode.TaskScope.Workspace, 
            'Build',
            res.extensionId,
            new vscode.ShellExecution(builder.getCommand(), builder.getArguments()),
            `$${res.microsoftProblemMatcherId}`
        );
    }
    private static async getTaskAsync(projectFile: vscode.Uri, target: string, ...args: string[]): Promise<vscode.Task> {
        const builder = new ProcessArgumentBuilder('dotnet')
            .append(target)
            .append(projectFile.fsPath)
            .append(...args);

        return new vscode.Task({ type: `${res.extensionId}.task` },
            vscode.TaskScope.Workspace, target, res.extensionId, new vscode.ShellExecution(builder.getCommand(), builder.getArguments()), res.microsoftProblemMatcherId
        );
    }
}
