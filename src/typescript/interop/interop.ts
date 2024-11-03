import { ProcessArgumentBuilder } from './processArgumentBuilder';
import { ProcessRunner } from './processRunner';
import { OperationStatus } from '../models/operationStatus';
import { Process } from '../models/processItem';
import { Test, TestResult } from '../models/test';
import * as path from 'path';

export class Interop {
    private static essentialsToolPath: string;

    public static init(extensionPath: string) {
        Interop.essentialsToolPath = path.join(extensionPath, "extension", "bin", "DotRush.Essentials.dll");
    }

    public static async installVsdbg(): Promise<OperationStatus> {
        return await ProcessRunner.runAsync<OperationStatus>(new ProcessArgumentBuilder("dotnet")
            .appendQuoted(Interop.essentialsToolPath)
            .append("--install-vsdbg"));
    }

    public static async getProcesses(): Promise<Process[]> {
        return await ProcessRunner.runAsync<Process[]>(new ProcessArgumentBuilder("dotnet")
            .appendQuoted(Interop.essentialsToolPath)
            .append("--list-proc"));
    }
    public static async getTests(folders: string[]): Promise<Test[]> {
        return await ProcessRunner.runAsync<Test[]>(new ProcessArgumentBuilder("dotnet")
            .appendQuoted(Interop.essentialsToolPath)
            .append("--list-tests")
            .append(...folders));
    }
    public static async getTestsResult(report: string): Promise<TestResult[]> {
        return await ProcessRunner.runAsync<TestResult[]>(new ProcessArgumentBuilder("dotnet")
            .appendQuoted(Interop.essentialsToolPath)
            .append("--parse-trx")
            .appendQuoted(report));
    }
    public static async getPropertyValue(propertyName: string, projectPath: string): Promise<string> {
        return (await ProcessRunner.runAsyncRaw(new ProcessArgumentBuilder("dotnet")
            .append("msbuild")
            .append(`-getProperty:${propertyName}`)
            .appendQuoted(projectPath)))
            .trim();
    }
}
