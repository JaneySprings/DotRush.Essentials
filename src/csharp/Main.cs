using System.Runtime.InteropServices;
using System.Text.Json;
using DotRush.Essentials.Tools.Models;

namespace DotRush.Essentials.Tools;

public class Program {
    public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static string ExecExtension => IsWindows ? ".exe" : "";

    public static async Task Main(string[] args) {
        if (args.Length == 0)
            Environment.Exit(1);

        if (args[0] == "--install-vsdbg") {
            try {
                var url = await DebuggerTools.GetDebuggerInstallLink();
                if (string.IsNullOrEmpty(url))
                    SetResult(OperationResponse.Failure("Cannot optain debugger download link"));

                var executable = await DebuggerTools.InstallDebugger(url!);
                if (string.IsNullOrEmpty(executable))
                    SetResult(OperationResponse.Failure("Cannot locate debugger executable"));

                var registerResult = await DebuggerTools.RegisterDebugger(executable!);
                if (!registerResult)
                    SetResult(OperationResponse.Failure("Cannot register debugger"));

            } catch (Exception ex) {
                SetResult((OperationResponse.Failure(ex.Message)));
            }
            SetResult(OperationResponse.Success());
        }

        if (args[0] == "--list-proc") {
            try {
                SetResult(DebuggerTools.GetProcesses());
            } catch {
                SetResult(Enumerable.Empty<object>());
            }
        }

        if (args[0] == "--list-tests") {
            try {
                var tests = TestTools.DiscoverTests(args.Skip(1).ToArray());
                SetResult(tests);
            } catch {
                SetResult(Enumerable.Empty<object>());
            }
        }
        if (args[0] == "--parse-trx") {
            try {
                var result = TestTools.ProcessTrxFile(args[1]);
                SetResult(result);
            } catch {
                SetResult(Enumerable.Empty<object>());
            }
        }
    }

    private static void SetResult(object result) {
        Console.Out.WriteLine(JsonSerializer.Serialize(result));
        Environment.Exit(0);
    }
}