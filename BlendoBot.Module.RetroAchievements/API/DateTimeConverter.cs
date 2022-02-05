using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlendoBot.Module.RetroAchievements.API;

internal class DateTimeConverter : JsonConverter<DateTime> {
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		return DateTime.ParseExact(reader.GetString()!, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
	}

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) {
		return;
	}
}
