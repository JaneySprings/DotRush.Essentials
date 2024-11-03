import { spawnSync, exec } from 'child_process';
import { ProcessArgumentBuilder } from './processArgumentBuilder';

export class ProcessRunner {
    public static runSync(command: string, ...args: string[]): string | undefined {
        const result = spawnSync(command, args);
        if (result.error) {
            console.error(result.error);
            return undefined;
        }
        return result.stdout.toString().trimEnd();
    }
    public static async runAsync<TModel>(builder: ProcessArgumentBuilder): Promise<TModel> {
        const result = await ProcessRunner.runAsyncRaw(builder);
        return JSON.parse(result);
    }
    public static async runAsyncRaw(builder: ProcessArgumentBuilder): Promise<string> {
        return new Promise<string>((resolve, reject) => {
            exec(builder.build(), (error, stdout, stderr) => {
                if (error) {
                    console.error(stderr);
                    reject(stderr);
                }

                resolve(stdout.toString());
            })
        });
    }
    public static async getResultAsync(builder: ProcessArgumentBuilder): Promise<ProcessResult> {
        return new Promise<ProcessResult>((resolve, reject) => {
            exec(builder.build(), (error, stdout, stderr) => {
                resolve({
                    isSucceded: error === null,
                    stdout: stdout.toString(),
                    stderr: stderr.toString()
                });
            })
        });
    }
}

export interface ProcessResult {
    isSucceded: boolean;
    stdout: string;
    stderr: string;
}