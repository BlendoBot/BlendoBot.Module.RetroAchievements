using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlendoBot.Module.RetroAchievements.API;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
internal class AchievementOfTheWeekResponse {
	public AchievementDetails Achievement { get; set; } = default!;
	public ConsoleDetails Console { get; set; } = default!;
	public ForumTopicDetails ForumTopic { get; set; } = default!;
	public GameDetails Game { get; set; } = default!;
	[JsonConverter(typeof(DateTimeConverter))]
	public DateTime StartAt { get; set; }
	public int TotalPlayers { get; set; }
	public List<UnlockDetails> Unlocks { get; set; } = default!;
	public int UnlocksCount { get; set; }

	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	internal class AchievementDetails {
		public ulong ID { get; set; }
		public string Title { get; set; } = default!;
		public string Description { get; set; } = default!;
		public int Points { get; set; }
		public int TrueRatio { get; set; }
		public string Author { get; set; } = default!;
		[JsonConverter(typeof(DateTimeConverter))]
		public DateTime DateCreated { get; set; }
		[JsonConverter(typeof(DateTimeConverter))]
		public DateTime DateModified { get; set; }
	}

	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	internal class ConsoleDetails {
		public ulong ID { get; set; }
		public string Title { get; set; } = default!;
	}

	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	internal class ForumTopicDetails {
		public ulong ID { get; set; }
	}

	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	internal class GameDetails {
		public ulong ID { get; set; }
		public string Title { get; set; } = default!;
	}

	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	internal class UnlockDetails {
		public string User { get; set; } = default!;
		public int RAPoints { get; set; }
		[JsonConverter(typeof(DateTimeConverter))]
		public DateTime DateAwarded { get; set; }
		public int HardcoreMode { get; set; }
	}
}
