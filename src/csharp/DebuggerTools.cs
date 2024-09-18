using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.Json;
using DotRush.Essentials.Tools.External;
using DotRush.Essentials.Tools.Logging;
using DotRush.Essentials.Tools.Models;

namespace DotRush.Essentials.Tools;

public static class DebuggerTools {
    //vsDbgUrl = "https://aka.ms/getvsdbgsh";
    private const string OmnisharpPackageConfigUrl = "https://raw.githubusercontent.com/dotnet/vscode-csharp/main/package.json";
    private const string OmnisharpDebuggerId = "Debugger";

    public static async Task<string?> GetDebuggerInstallLink() {
        CurrentSessionLogger.Debug($"Installing vsdbg via link: {OmnisharpPackageConfigUrl}");

        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(OmnisharpPackageConfigUrl);
        if (!response.IsSuccessStatusCode) {
            CurrentSessionLogger.Error("Failed to fetch omnisharp package config");
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var packageModel = JsonSerializer.Deserialize<PackageModel>(content);
        if (packageModel == null) {
            CurrentSessionLogger.Error("Failed to deserialize omnisharp package config");
            return null;
        }

        CurrentSessionLogger.Debug($"Omnisharp package received: {packageModel.Name} - {packageModel.Version}");

        var debuggers = packageModel.RuntimeDependencies?.Where(x => x.Id == OmnisharpDebuggerId);
        if (debuggers == null || !debuggers.Any()) {
            CurrentSessionLogger.Error("No debuggers found in omnisharp package config");
            return null;
        }

        var currentRuntimeId = RuntimeInformation.RuntimeIdentifier.Replace("win-", "win32-").Replace("osx-", "darwin-").Replace("-x64", "-x86_64");
        var platformAndArch = currentRuntimeId.Split('-');
        if (platformAndArch.Length != 2) {
            CurrentSessionLogger.Error($"Incorrect runtimeId: {currentRuntimeId}");
            return null;
        }
        
        var result = debuggers
            .Where(d => d.Platforms.Contains(platformAndArch[0]) && d.Architectures.Contains(platformAndArch[1]))
            .OrderBy(d => d.Architectures.Count())
            .FirstOrDefault();

        if (result == null) {
            CurrentSessionLogger.Error($"No suitable debugger found for {platformAndArch[0]}-{platformAndArch[1]}");
            return null;
        }

        return result.Url;
    }
    public static async Task<string?> InstallDebugger(string downloadUrl) {
        var workingDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".."));
        var downloadPath = Path.Combine(workingDirectory, "vsdbg.zip");
        var extractPath = Path.Combine(workingDirectory, "debugger");

        CurrentSessionLogger.Debug($"Downloading debugger from '{downloadUrl}'");

        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(downloadUrl);
        if (!response.IsSuccessStatusCode) {
            CurrentSessionLogger.Error($"Failed to download debugger: {response.StatusCode}");
            return null;
        }

        CurrentSessionLogger.Debug($"Extracting debugger to '{extractPath}'");

        using var archive = new ZipArchive(response.Content.ReadAsStream());
        foreach (var entry in archive.Entries) {
            var targetPath = Path.Combine(extractPath, entry.FullName);
            var targetDirectory = Path.GetDirectoryName(targetPath)!;

            if (string.IsNullOrEmpty(Path.GetFileName(targetPath)))
                continue;
            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

            using var fileStream = File.Create(targetPath);
            using var stream = entry.Open();
            stream.CopyTo(fileStream);
        }

        var executable = Path.Combine(extractPath, "vsdbg" + Program.ExecExtension);
        if (!File.Exists(executable)) {
            CurrentSessionLogger.Error($"Debugger executable not found: '{executable}'");
            return null;
        }

        return executable;
    }
    public static async Task<bool> RegisterDebugger(string executable) {
        if (Program.IsWindows)
            return true;

        var processInfo = ProcessRunner.CreateProcess("chmod", $"+x {executable}");
        var result = await processInfo.Result;
        return result.ExitCode == 0;
    }

    public static IEnumerable<ProcessModel> GetProcesses() {
        return Process.GetProcesses().Select(it => new ProcessModel(it)).ToImmutableList();
    }
}