using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlendoBot.Module.RetroAchievements.API;

internal class SometimesEmptyListConverter<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>> where TKey : notnull {
	public override Dictionary<TKey, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		if (reader.TokenType == JsonTokenType.StartArray) {
			// Bound to be an empty list.
			reader.Read();
			return new();
		} else {
			return JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(ref reader, options);
		}
	}

	public override void Write(Utf8JsonWriter writer, Dictionary<TKey, TValue> value, JsonSerializerOptions options) {
		return;
	}
}
