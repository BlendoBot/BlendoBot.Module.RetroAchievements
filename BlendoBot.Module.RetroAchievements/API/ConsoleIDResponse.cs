using System.Text.Json.Serialization;

namespace BlendoBot.Module.RetroAchievements.API;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
internal class ConsoleIDResponse {
	public ulong ID { get; set; }
	public string Name { get; set; } = default!;
}
