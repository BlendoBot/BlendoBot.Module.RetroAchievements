using System.Text.Json.Serialization;

namespace BlendoBot.Module.RetroAchievements.API;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
internal class TopUserResponse {
	[JsonPropertyName("1")]
	public string Name { get; set; } = default!;
	[JsonPropertyName("2")]
	public ulong Score { get; set; }
	[JsonPropertyName("3")]
	public ulong TrueScore { get; set; }
}
