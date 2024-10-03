using System.Runtime.InteropServices;

namespace DotRush.Essentials.Tools.Extensions;

public static class RuntimeExtensions {
    public static string GetArchitecture() {
        var arch = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "arm64" : "x86_64";
        return arch;
    }
    public static string GetOperationSystem() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "win32";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "darwin";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "linux";

        return "unknown";
    }
}