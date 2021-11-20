using System.Text.Json.Serialization;

namespace RiotArcaneMissionsBot;

internal record Authorization(
    [property: JsonPropertyName("type")]
    string Type,
    [property: JsonPropertyName("error")]
    string? Error,
    [property: JsonPropertyName("country")]
    string Country,
    [property: JsonPropertyName("response")]
    Response? Response
);

internal record Response(
    [property: JsonPropertyName("mode")]
    string Mode,
    [property: JsonPropertyName("parameters")]
    Parameters Parameters
);

internal record Parameters(
    [property: JsonPropertyName("uri")]
    Uri Uri
);