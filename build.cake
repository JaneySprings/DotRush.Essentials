using _Path = System.IO.Path;

public string RootDirectory => MakeAbsolute(Directory("./")).ToString();
public string ArtifactsDirectory => _Path.Combine(RootDirectory, "artifacts");
public string ExtensionDirectory => _Path.Combine(RootDirectory, "extension");
public string DotRushEssentialsProjectPath => _Path.Combine(RootDirectory, "src", "csharp", "DotRush.Essentials.Tools.csproj");

var target = Argument("target", "vsix");
var version = Argument("release-version", "1.0.0");
var configuration = Argument("configuration", "debug");


Setup(context => {
	var date = DateTime.Now;
	version = $"{DateTime.Now.ToString("yy")}.{date.ToString("%M")}.{date.DayOfYear}";
	EnsureDirectoryExists(ArtifactsDirectory);
});

Task("clean").Does(() => {
	EnsureDirectoryExists(ArtifactsDirectory);
	CleanDirectories(_Path.Combine(RootDirectory, "src", "**", "bin"));
	CleanDirectories(_Path.Combine(RootDirectory, "src", "**", "obj"));
	CleanDirectories(ExtensionDirectory);
});

Task("tools").Does(() => DotNetPublish(DotRushEssentialsProjectPath, new DotNetPublishSettings {
	MSBuildSettings = new DotNetMSBuildSettings { AssemblyVersion = version },
	OutputDirectory = _Path.Combine(ExtensionDirectory, "bin"),
	Configuration = configuration,
}));

Task("vsix")
	.IsDependentOn("clean")
	.IsDependentOn("tools")
	.Does(() => {
		var output = _Path.Combine(ArtifactsDirectory, $"DotRush.Essentials.v{version}_Universal.vsix");
		ExecuteCommand("npm", "install");
		ExecuteCommand("vsce", $"package --out {output} --no-git-tag-version {version}");
	});


void ExecuteCommand(string command, string arguments) {
	if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
		arguments = $"/c \"{command} {arguments}\"";
		command = "cmd";
	}
	if (StartProcess(command, arguments) != 0)
		throw new Exception("Command exited with non-zero exit code.");
}

RunTarget(target);