using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlendoBot.Module.RetroAchievements.API;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
internal class GameInfoAndUserProgressResponse {
	public ulong ID { get; set; }
	public string Title { get; set; } = default!;
	public ulong ConsoleID { get; set; }
	public ulong ForumTopicID { get; set; }
	public ulong Flags { get; set; }
	public string ImageIcon { get; set; } = default!;
	public string ImageTitle { get; set; } = default!;
	public string ImageIngame { get; set; } = default!;
	public string ImageBoxArt { get; set; } = default!;
	public string Publisher { get; set; } = default!;
	public string Developer { get; set; } = default!;
	public string Genre { get; set; } = default!;
	public string Released { get; set; } = default!;
	public bool IsFinal { get; set; }
	public string ConsoleName { get; set; } = default!;
	public string RichPresencePatch { get; set; } = default!;
	public int NumAchievements { get; set; }
	public int NumDistinctPlayersCasual { get; set; }
	public int NumDistinctPlayersHardcore { get; set; }
	[JsonConverter(typeof(SometimesEmptyListConverter<string, Achievement>))]
	public Dictionary<string, Achievement> Achievements { get; set; } = default!;
	public int NumAwardedToUser { get; set; }
	public int NumAwardedToUserHardcore { get; set; }
	[JsonConverter(typeof(PercentageConverter))]
	public double UserCompletion { get; set; }
	[JsonConverter(typeof(PercentageConverter))]
	public double UserCompletionHardcore { get; set; }

	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	internal class Achievement {
		public ulong ID { get; set; }
		public int NumAwarded { get; set; }
		public int NumAwardedHardcore { get; set; }
		public string Title { get; set; } = default!;
		public string Description { get; set; } = default!;
		public int Points { get; set; }
		public int TrueRatio { get; set; }
		public string Author { get; set; } = default!;
		public string DateModified { get; set; } = default!;
		public string DateCreated { get; set; } = default!;
		public ulong BadgeName { get; set; }
		public ulong DisplayOrder { get; set; }
		public string MemAddr { get; set; } = default!;
		public string? DateEarned { get; set; } = default!;
		public string? DateEarnedHardcore { get; set; } = default!;
	}
}
