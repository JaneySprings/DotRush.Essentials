using System.Text.Json.Serialization;

namespace DotRush.Essentials.Tools.Models;

public class PackageModel {

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("runtimeDependencies")]
    public IEnumerable<RuntimeDependencyModel>? RuntimeDependencies { get; set; }
}

public class RuntimeDependencyModel {

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("platforms")]
    public IEnumerable<string> Platforms { get; set; } = Enumerable.Empty<string>();

    [JsonPropertyName("architectures")]
    public IEnumerable<string> Architectures { get; set; } = Enumerable.Empty<string>();

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    public override string ToString() {
        return $"{{ {Id}-{Url}, Platforms: [{string.Join(", ", Platforms)}], Architectures: [{string.Join(", ", Architectures)}] }}";
    }
}