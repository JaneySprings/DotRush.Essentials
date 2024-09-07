
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace DotRush.Essentials.Tools.Models;

public class ProcessModel {

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    public ProcessModel() {}
    public ProcessModel(Process process) {
        Id = process.Id;
        Name = process.ProcessName;
    }
}