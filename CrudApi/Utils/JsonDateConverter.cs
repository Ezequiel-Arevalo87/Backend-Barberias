using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string timeZoneId = "SA Pacific Standard Time";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Leer como hora local sin conversión adicional
        return DateTime.SpecifyKind(reader.GetDateTime(), DateTimeKind.Unspecified);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var horaColombia = TimeZoneInfo.ConvertTime(value, tz);
        writer.WriteStringValue(horaColombia.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}
