using System.Text.Json;
using System.Text.Json.Serialization;

namespace RiotArcaneMissionsBot;

internal record Missions(
    [property: JsonPropertyName("available_missions")]
    IEnumerable<Mission>? AvailableMissions,
    [property: JsonPropertyName("completed_missions")]
    IEnumerable<Mission>? CompletedMissions
);

internal record Mission(
    [property: JsonPropertyName("uid")]
    string? Uid,
    [property: JsonPropertyName("parent")]
    string? Parent,
    [property: JsonPropertyName("mission_type"), JsonConverter(typeof(MissionTypeConverter))]
    MissionType MissionType,
    [property: JsonPropertyName("title")]
    string? Title,
    [property: JsonPropertyName("children")]
    IEnumerable<string>? Children,
    [property: JsonPropertyName("num_children")]
    long? NumChildren,
    [property: JsonPropertyName("default_visible")]
    bool DefaultVisible
);

public enum MissionType
{
    ComplexParallel,
    ComplexSerial,
    SimpleInteractive
}

internal class MissionTypeConverter : JsonConverter<MissionType>
{
    public override MissionType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString() switch
        {
            "complex_parallel" => MissionType.ComplexParallel,
            "complex_serial" => MissionType.ComplexSerial,
            "simple_interactive" => MissionType.SimpleInteractive,
            _ => throw new Exception("Cannot unmarshal type MissionType")
        };
    }

    public override void Write(Utf8JsonWriter writer, MissionType value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case MissionType.ComplexParallel:
                writer.WriteStringValue("complex_parallel");
                return;
            case MissionType.ComplexSerial:
                writer.WriteStringValue("complex_serial");
                return;
            case MissionType.SimpleInteractive:
                writer.WriteStringValue("simple_interactive");
                return;
            default:
                throw new Exception("Cannot marshal type MissionType");
        }
    }
}