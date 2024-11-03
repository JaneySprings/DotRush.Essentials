
using System.Text.Json.Serialization;

namespace DotRush.Essentials.Tools.Models;

public class TestResultModel {

    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    [JsonPropertyName("state")] // Passed, Failed, Skipped
    public string? State { get; set; }

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }
}

public class TestModel {

    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("filePath")]
    public string? FilePath { get; set; }

    [JsonPropertyName("range")]
    public Range? Range { get; set; }

    [JsonPropertyName("children")]
    public List<TestModel>? Children { get; set; }
}

public class Range {

    [JsonPropertyName("start")]
    public Position? Start { get; set; }

    [JsonPropertyName("end")]
    public Position? End { get; set; }
}

public class Position {

    [JsonPropertyName("line")]
    public int Line { get; set; }

    [JsonPropertyName("character")]
    public int Character { get; set; }
}