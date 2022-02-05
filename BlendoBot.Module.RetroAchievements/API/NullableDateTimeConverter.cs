using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlendoBot.Module.RetroAchievements.API;

internal class NullableDateTimeConverter : JsonConverter<DateTime?> {
	public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		if (reader.TokenType == JsonTokenType.Null) {
			return null;
		} else {
			return DateTime.ParseExact(reader.GetString()!, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
		}
	}

	public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options) {
		return;
	}
}
