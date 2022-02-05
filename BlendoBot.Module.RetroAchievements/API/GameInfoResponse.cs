using System.Text.Json.Serialization;

namespace BlendoBot.Module.RetroAchievements.API;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
internal class GameInfoResponse {
	public string Title { get; set; } = default!;
	public string ForumTopicID { get; set; } = default!;
	public ulong ConsoleID { get; set; }
	public string ConsoleName { get; set; } = default!;
	public ulong Flags { get; set; }
	public string ImageIcon { get; set; } = default!;
	public string GameIcon { get; set; } = default!;
	public string ImageTitle { get; set; } = default!;
	public string ImageIngame { get; set; } = default!;
	public string ImageBoxArt { get; set; } = default!;
	public string Publisher { get; set; } = default!;
	public string Developer { get; set; } = default!;
	public string Genre { get; set; } = default!;
	public string Released { get; set; } = default!;
	public string GameTitle { get; set; } = default!;
	public string Console { get; set; } = default!;
}
