
using System.Text.Json.Serialization;

namespace DotRush.Essentials.Tools.Models;

public class OperationStatus {
    [JsonPropertyName("isSucceded")]
    public bool IsSucceded { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("helpLink")]
    public string? HelpLink { get; set; }

    public static OperationStatus Success() {
        return new OperationStatus { IsSucceded = true };
    }
    public static OperationStatus Failure(string message) {
        return new OperationStatus {
            Message = message,
            HelpLink = new Uri(Logging.LogConfig.DebugLogFile).AbsoluteUri
        };
    }
}