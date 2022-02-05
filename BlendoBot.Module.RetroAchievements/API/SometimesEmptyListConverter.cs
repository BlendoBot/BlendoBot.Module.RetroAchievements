using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlendoBot.Module.RetroAchievements.API;

internal class PercentageConverter : JsonConverter<double> {
	public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		if (reader.TokenType == JsonTokenType.String) {
			string nextString = reader.GetString()!;
			return double.Parse(nextString.TrimEnd(new char[] { '%', ' ' }));
		} else {
			return reader.GetDouble();
		}
	}

	public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options) {
		return;
	}
}
