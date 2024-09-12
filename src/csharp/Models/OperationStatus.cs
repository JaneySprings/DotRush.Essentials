
using System.Text.Json.Serialization;

namespace DotRush.Essentials.Tools.Models;

public class OperationResponse {
    [JsonPropertyName("isSucceded")]
    public bool IsSucceded { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("helpLink")]
    public string? HelpLink { get; set; }

    public static OperationResponse Success() {
        return new OperationResponse { IsSucceded = true };
    }
    public static OperationResponse Failure(string message) {
        return new OperationResponse {
            Message = message,
            HelpLink = new Uri(Logging.LogConfig.DebugLogFile).AbsoluteUri
        };
    }
}