using BlendoBot.Core.Command;
using BlendoBot.Core.Entities;
using BlendoBot.Core.Module;
using BlendoBot.Core.Utility;
using BlendoBot.Module.RetroAchievements.API;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlendoBot.Module.RetroAchievements;

internal class RetroAchievementsCommand : ICommand {
	public RetroAchievementsCommand(RetroAchievements module) {
		this.module = module;
	}

	private readonly RetroAchievements module;
	public IModule Module => module;

	public string Guid => "retroachievements.command";
	public string DesiredTerm => "ra";
	public string Description => "Shows information about RetroAchievements";
	public Dictionary<string, string> Usage => new() {
		{ "aotw", "Shows the Achievement of the Week." },
		{ "(cheevo|achievement) [ID]", "Shows a specific achievement by ID." },
		{ "game [(game name|ID)}", "Shows a specific game by a searched name or ID." },
		{ "last [profile name]", "Shows the last achievement achieved by you, or optionally a specific profile." },
		{ "profile [profile name]", "Shows either your profile, or optionally a specific profile." },
		{ "setprofile [profile name]", $"Assigns a RetroAchievements profile to you. Some other commands such as {"game".Code()}, {"last".Code()}, and {"profile".Code()} can show information specific to you when available." },
	};

	public async Task OnMessage(MessageCreateEventArgs e, string[] tokenizedInput) {
		await e.Channel.TriggerTypingAsync();
		if (tokenizedInput.Length > 0) {
			switch (tokenizedInput[0].ToLower()) {
				case "setprofile":
					await OnMessageSetProfile(e, tokenizedInput[1..]);
					break;
				case "profile":
					await OnMessageProfile(e, tokenizedInput[1..]);
					break;
				case "last":
					await OnMessageLast(e, tokenizedInput[1..]);
					break;
				case "game":
					await OnMessageGame(e, tokenizedInput[1..]);
					break;
				case "aotw":
					await OnMessageAotw(e);
					break;
				case "cheevo":
				case "achievement":
					await OnMessageCheevo(e, tokenizedInput[1..]);
					break;
				default:
					await module.DiscordInteractor.SendUnknownArgumentsMessage(this, e.Channel, this);
					break;
			}
		} else {
			await module.DiscordInteractor.SendUnknownArgumentsMessage(this, e.Channel, this);
		}
	}

	private async Task OnMessageSetProfile(MessageCreateEventArgs e, string[] tokenizedInput) {
		if (tokenizedInput.Length > 0) {
			bool success = await module.SetUserProfileName(e.Author.Id, tokenizedInput[0]);
			if (success) {
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Message = $"Successfully set your RA profile to {tokenizedInput[0].Code()}.",
					Channel = e.Channel,
					Tag = "RetroAchievementsSetProfileSuccess"
				});
			} else {
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Message = $"Could not set your RA Profile, {tokenizedInput[0].Code()} does not exist as an account!",
					Channel = e.Channel,
					Tag = "RetroAchievementsSetProfileFailure"
				});
			}
		} else {
			string? raProfileName = module.GetUserProfileName(e.Author.Id);
			if (raProfileName == null) {
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Message = $"You are currently not registered with any RetroAchievements profile.",
					Channel = e.Channel,
					Tag = "RetroAchievementsSetProfileNoName"
				});
			} else {
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Message = $"You are currently set as {raProfileName.Code()}.",
					Channel = e.Channel,
					Tag = "RetroAchievementsSetProfileName"
				});
			}
		}
	}

	private string? ExtractProfileName(string mentionedProfile) {
		if (mentionedProfile.StartsWith("<@!")) {
			return module.GetUserProfileName(ulong.Parse(mentionedProfile[3..^1]));
		} else {
			return mentionedProfile;
		}
	}

	private static ulong SecondsSinceEpoch(DateTime d) => (ulong)(d - DateTime.UnixEpoch).TotalSeconds;

	private async Task OnMessageProfile(MessageCreateEventArgs e, string[] tokenizedInput) {
		string? raProfileName;
		if (tokenizedInput.Length > 0) {
			raProfileName = ExtractProfileName(tokenizedInput[0]);
			if (raProfileName == null) {
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Message = "That user is currently not registered with any RetroAchievements profile.",
					Channel = e.Channel,
					Tag = "RetroAchievementsProfileGetNoName"
				});
				return;
			}
		} else {
			raProfileName = module.GetUserProfileName(e.Author.Id);
			if (raProfileName == null) {
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Message = "You are currently not registered with any RetroAchievements profile.",
					Channel = e.Channel,
					Tag = "RetroAchievementsProfileGetNoName"
				});
				return;
			}
		}
		UserSummaryResponse? response = await module.ApiClient.GetUserSummary(raProfileName, 5, 0);
		if (response == null) {
			await module.DiscordInteractor.Send(this, new SendEventArgs {
				Message = "That user could not be found.",
				Channel = e.Channel,
				Tag = "RetroAchievementsProfileGetCannotFind"
			});
			return;
		}
		DiscordEmbedBuilder builder = new();
		builder.Title = raProfileName;
		builder.Url = module.ApiClient.GetUserUrl(raProfileName);
		builder.WithThumbnail(module.ApiClient.GetImageFullPath(response.UserPic));
		builder.Color = Optional.FromValue(DiscordColor.Green);
		builder.Timestamp = DateTime.UtcNow;
		builder.Description = $"({response.Points.ToString().Bold()} points) ({response.TotalTruePoints.ToString().Italics()})";
		builder.WithFooter($"Profile ID: {raProfileName}");
		builder.AddField("Ranked", $"{response.Rank} / {response.TotalRanked} ranked users", true);
		if (response.LastGame != null) {
			builder.AddField("Last Seen In", $"{response.LastGame.Title.Bold()} ({response.LastGame.ConsoleName})\nLast played <t:{SecondsSinceEpoch(response.RecentlyPlayed[0].LastPlayed)}:f>\nEarned {response.Awarded[response.RecentlyPlayed[0].GameID.ToString()].NumAchieved} of {response.Awarded[response.RecentlyPlayed[0].GameID.ToString()].NumPossibleAchievements} achievements, {response.Awarded[response.RecentlyPlayed[0].GameID.ToString()].ScoreAchieved}/{response.Awarded[response.RecentlyPlayed[0].GameID.ToString()].PossibleScore} points.", false);
			builder.AddField("Presence", response.RichPresenceMsg, false);
		}
		await module.DiscordInteractor.Send(this, new SendEventArgs {
			Embed = builder.Build(),
			Channel = e.Channel,
			Tag = "RetroAchievementsGetProfile"
		});
	}

	private async Task OnMessageLast(MessageCreateEventArgs e, string[] tokenizedInput) {
		string? raProfileName;
		if (tokenizedInput.Length > 0) {
			raProfileName = ExtractProfileName(tokenizedInput[0]);
			if (raProfileName == null) {
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Message = "That user is currently not registered with any RetroAchievements profile.",
					Channel = e.Channel,
					Tag = "RetroAchievementsLastNoName"
				});
				return;
			}
		} else {
			raProfileName = module.GetUserProfileName(e.Author.Id);
			if (raProfileName == null) {
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Message = "You are currently not registered with any RetroAchievements profile.",
					Channel = e.Channel,
					Tag = "RetroAchievementsLastNoName"
				});
				return;
			}
		}
		UserSummaryResponse? response = await module.ApiClient.GetUserSummary(raProfileName, 5, 0);
		if (response == null) {
			await module.DiscordInteractor.Send(this, new SendEventArgs {
				Message = "That user could not be found.",
				Channel = e.Channel,
				Tag = "RetroAchievementsLastCannotFind"
			});
			return;
		}
		if (response.RecentAchievements == null) {
			await module.DiscordInteractor.Send(this, new SendEventArgs {
				Message = "That user has not unlocked an achievement yet.",
				Channel = e.Channel,
				Tag = "RetroAchievementsLastNoAchievements"
			});
			return;
		}
		UserSummaryResponse.RecentAchievementDetails lastAchievement = response.RecentAchievements.SelectMany(d => d.Value).Select(d => d.Value).Where(a => a.DateAwarded != null).MaxBy(a => a.DateAwarded)!;
		GameInfoExtendedResponse gameDetails = await module.ApiClient.GetGameInfoExtended(lastAchievement.GameID);
		GameInfoExtendedResponse.Achievement achievementDetails = gameDetails.Achievements[lastAchievement.ID.ToString()];
		DiscordEmbedBuilder builder = new();
		builder.Title = $"{lastAchievement.Title.Bold()} ({lastAchievement.Points.ToString().Bold()}) ({achievementDetails.TrueRatio.ToString().Italics()})";
		builder.Description = lastAchievement.Description;
		builder.Url = module.ApiClient.GetAchievementUrl(lastAchievement.ID);
		builder.WithThumbnail(module.ApiClient.GetBadgeNamePath(lastAchievement.BadgeName));
		builder.Color = Optional.FromValue(DiscordColor.Blue);
		builder.Timestamp = DateTime.UtcNow;
		builder.WithFooter($"Achievement ID: {lastAchievement.ID}");
		builder.AddField("Game", $"{lastAchievement.GameTitle} ({gameDetails.ConsoleName})", false);
		builder.AddField("Won By", $"{achievementDetails.NumAwarded.ToString().Bold()} of {gameDetails.NumDistinctPlayersCasual.ToString().Bold()} possible players", true);
		builder.AddField("Achieved On", $"<t:{SecondsSinceEpoch(lastAchievement.DateAwarded.GetValueOrDefault())}:f>", true);
		await module.DiscordInteractor.Send(this, new SendEventArgs {
			Embed = builder.Build(),
			Channel = e.Channel,
			Tag = "RetroAchievementsLast"
		});
	}

	private async Task OnMessageGame(MessageCreateEventArgs e, string[] tokenizedInput) {
		if (tokenizedInput.Length > 0) {
			if (!ulong.TryParse(tokenizedInput[0], out ulong gameId)) {
				ulong? foundId = await module.ApiClient.GetGameIDByName(string.Join(' ', tokenizedInput));
				if (foundId != null) {
					gameId = foundId.Value;
				} else {
					await module.DiscordInteractor.Send(this, new SendEventArgs {
						Message = $"Could not find a game that matched {string.Join(' ', tokenizedInput).Code()}!",
						Channel = e.Channel,
						Tag = "RetroAchievementsGetGameCannotParse"
					});
					return;
				}
			}
			string? user = module.GetUserProfileName(e.Author.Id);
			GameInfoAndUserProgressResponse response = await module.ApiClient.GetGameInfoAndUserProgress(user ?? string.Empty, gameId);
			if (response.Title == default) {
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Message = $"No game with that ID was found.",
					Channel = e.Channel,
					Tag = "RetroAchievementsGetGameNotFound"
				});
			} else {
				DiscordEmbedBuilder builder = new();
				builder.Title = response.Title;
				builder.Url = module.ApiClient.GetGameUrl(gameId);
				builder.WithThumbnail(module.ApiClient.GetImageFullPath(response.ImageIcon));
				builder.Color = Optional.FromValue(DiscordColor.Yellow);
				builder.Timestamp = DateTime.UtcNow;
				builder.WithFooter($"Game ID: {gameId}");
				if (response.ConsoleName != default) builder.AddField("Console", response.ConsoleName, true);
				if (response.Developer != default) builder.AddField("Developer", response.Developer, true);
				if (response.Publisher != default) builder.AddField("Publisher", response.Publisher, true);
				if (response.Genre != default) builder.AddField("Genre", response.Genre, true);
				if (response.Released != default) builder.AddField("Release Date", response.Released, true);
				if (response.NumAchievements > 0) {
					int achievementPointTotal = response.Achievements.Sum(a => a.Value.Points);
					int achievementTruePointTotal = response.Achievements.Sum(a => a.Value.TrueRatio);
					builder.Description = $"There are {response.NumAchievements.ToString().Bold()} achievements worth {achievementPointTotal.ToString().Bold()} ({achievementTruePointTotal.ToString().Italics()}) points.";
					if (user != null) {
						builder.Description += $"\nYou've won {response.NumAwardedToUser.ToString().Bold()} achievements, worth {response.Achievements.Where(a => a.Value.DateEarned != null).Sum(a => a.Value.Points).ToString().Bold()} ({response.Achievements.Where(a => a.Value.DateEarned != null).Sum(a => a.Value.TrueRatio).ToString().Italics()}) points.";
						builder.Description += $"\nYou've won {response.NumAwardedToUserHardcore.ToString().Bold()} HARDCORE achievements, worth a further {response.Achievements.Where(a => a.Value.DateEarnedHardcore != null).Sum(a => a.Value.Points).ToString().Bold()} ({response.Achievements.Where(a => a.Value.DateEarnedHardcore != null).Sum(a => a.Value.TrueRatio).ToString().Italics()}) points.";
					}
				}
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Embed = builder.Build(),
					Channel = e.Channel,
					Tag = "RetroAchievementsGetGame"
				});
			}
		} else {
			await module.DiscordInteractor.SendUnknownArgumentsMessage(this, e.Channel, this);
		}
	}

	private async Task OnMessageAotw(MessageCreateEventArgs e) {
		AchievementOfTheWeekResponse response = await module.ApiClient.GetAchievementOfTheWeek();
		GameInfoExtendedResponse gameDetails = await module.ApiClient.GetGameInfoExtended(response.Game.ID);
		GameInfoExtendedResponse.Achievement achievementDetails = gameDetails.Achievements[response.Achievement.ID.ToString()];
		DiscordEmbedBuilder builder = new();
		builder.Title = $"{response.Achievement.Title.Bold()} ({response.Achievement.Points.ToString().Bold()}) ({response.Achievement.TrueRatio.ToString().Italics()})";
		builder.Description = response.Achievement.Description;
		builder.Url = module.ApiClient.GetAchievementUrl(response.Achievement.ID);
		builder.WithThumbnail(module.ApiClient.GetBadgeNamePath(achievementDetails.BadgeName));
		builder.Color = Optional.FromValue(DiscordColor.Red);
		builder.Timestamp = DateTime.UtcNow;
		builder.WithFooter($"Achievement ID: {response.Achievement.ID}");
		builder.AddField("Game", $"{gameDetails.Title} ({gameDetails.ConsoleName})", false);
		builder.AddField("Won By", $"{achievementDetails.NumAwarded.ToString().Bold()} of {gameDetails.NumDistinctPlayersCasual.ToString().Bold()} possible players", true);
		await module.DiscordInteractor.Send(this, new SendEventArgs {
			Embed = builder.Build(),
			Channel = e.Channel,
			Tag = "RetroAchievementsAOTW"
		});
	}

	private async Task OnMessageCheevo(MessageCreateEventArgs e, string[] tokenizedInput) {
		if (tokenizedInput.Length > 0) {
			if (ulong.TryParse(tokenizedInput[0], out ulong achievementId)) {
				AchievementUnlocksResponse response = await module.ApiClient.GetAchievementUnlocks(achievementId);
				if (response.Achievement.ID == null) {
					await module.DiscordInteractor.Send(this, new SendEventArgs {
						Message = $"No achievement with that ID was found.",
						Channel = e.Channel,
						Tag = "RetroAchievementsGetCheevoNotFound"
					});
				} else {
					string? user = module.GetUserProfileName(e.Author.Id);
					GameInfoExtendedResponse gameDetails = await module.ApiClient.GetGameInfoExtended(response.Game.ID.GetValueOrDefault());
					DiscordEmbedBuilder builder = new();
					builder.Title = $"{response.Achievement.Title.Bold()} ({response.Achievement.Points.ToString().Bold()}) ({response.Achievement.TrueRatio.ToString().Italics()})";
					builder.Description = response.Achievement.Description;
					builder.Url = module.ApiClient.GetAchievementUrl(response.Achievement.ID.GetValueOrDefault());
					builder.WithThumbnail(module.ApiClient.GetBadgeNamePath(gameDetails.Achievements[response.Achievement.ID.GetValueOrDefault().ToString()].BadgeName));
					builder.Color = Optional.FromValue(DiscordColor.Red);
					builder.Timestamp = DateTime.UtcNow;
					builder.WithFooter($"Achievement ID: {achievementId}");
					builder.AddField("Game", $"{gameDetails.Title} ({gameDetails.ConsoleName})", false);
					builder.AddField("Won By", $"{response.UnlocksCount.ToString().Bold()} of {response.TotalPlayers.ToString().Bold()} possible players", true);
					if (user != null) {
						AchievementUnlocksResponse.UnlockDetails? unlock = response.Unlocks.FirstOrDefault(u => u.User == user);
						if (unlock != null) {
							builder.AddField("Achieved On", $"<t:{SecondsSinceEpoch(unlock.DateAwarded)}:f>", true);
						}
					}
					await module.DiscordInteractor.Send(this, new SendEventArgs {
						Embed = builder.Build(),
						Channel = e.Channel,
						Tag = "RetroAchievementsGetCheevo"
					});
				}
			} else {
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Message = $"Could not parse {tokenizedInput[0].Code()} as a number, please type a valid achievement ID number instead!",
					Channel = e.Channel,
					Tag = "RetroAchievementsGetCheevoCannotParse"
				});
			}
		} else {
			await module.DiscordInteractor.SendUnknownArgumentsMessage(this, e.Channel, this);
		}
	}
}
