import { DebuggerController } from '../controllers/debuggerController';
import * as res from '../resources/constants';
import * as vscode from 'vscode';

export class DotNetDebugConfigurationProvider implements vscode.DebugConfigurationProvider {
	async resolveDebugConfiguration(folder: vscode.WorkspaceFolder | undefined,
									config: vscode.DebugConfiguration, 
									token?: vscode.CancellationToken): Promise<vscode.DebugConfiguration | undefined> {

		if (!config.type && !config.request && !config.name) {
			config.name = res.debuggerVsdbgTitle;
			config.type = res.debuggerVsdbgId;
			config.request = 'launch';
			config.preLaunchTask = `${res.extensionId}: Build`;
		}

		if (!config.program && config.request === 'launch')
			config.program = await DebuggerController.getProgramPath();

        return config;
	}
}