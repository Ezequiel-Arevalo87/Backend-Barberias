using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonDateConverter : JsonConverter<DateTime>
{
    private readonly string _format = "yyyy-MM-dd HH:mm:ss";
    private readonly TimeZoneInfo _colombiaZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var input = reader.GetString();
        var utcTime = DateTime.Parse(input, null, System.Globalization.DateTimeStyles.RoundtripKind).ToUniversalTime();
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, _colombiaZone);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var utc = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        var colombiaTime = TimeZoneInfo.ConvertTimeFromUtc(utc, _colombiaZone);
        writer.WriteStringValue(colombiaTime.ToString(_format));
    }
}

