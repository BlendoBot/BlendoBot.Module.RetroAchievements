using System.Text.Json.Serialization;

namespace BlendoBot.Module.RetroAchievements.API;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
internal class GameListResponse {
	public string Title { get; set; } = default!;
	public ulong ID { get; set; }
	public ulong ConsoleID { get; set; }
	public string ImageIcon { get; set; } = default!;
	public string ConsoleName { get; set; } = default!;
}
