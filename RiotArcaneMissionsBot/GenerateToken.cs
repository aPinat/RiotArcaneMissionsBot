using System.Text.Json.Serialization;

namespace RiotArcaneMissionsBot;

internal record GenerateToken(
    [property: JsonPropertyName("token")]
    string Token
);