# BlendoBot.Module.RetroAchievements
## Provides commands to view details about RetroAchievements.
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/BlendoBot/BlendoBot.Module.RetroAchievements/Tests)

The RetroAchievements module allows users to associate RA profiles with them, and search for people, games, and specific achievements on the site.

## What is RetroAchievements?
[RetroAchievements.org](https://retroachievements.org/) is a zero profit community who collaborate and compete to earn custom-made achievements in classic games through emulation. Achievements are made by and for the community. You can find out more about RetroAchievements through their [FAQ page](https://docs.retroachievements.org/FAQ/).

## Discord Usage
- `?ra aotw`
  - Shows the current Achievement of the Week.
- `?ra (cheevo|achievement) [ID]`
  - Shows a specific achievement by ID.
- `?ra game [(game name|ID)]`
  - Shows a specific game by a searched name or ID.
- `?ra last [profile name]`
  - Shows the last achievement achieved by you, or optionally a specific profile.
- `?ra profile [profile name]`
  - Shows either your profile, or optionally a specific profile.
- `?ra setprofile [profile name]`
  - Assigns a RetroAchievements profile to you. Some other commands such as `game`, `last`, and `profile` can show information specific to you when available.

## Config
This module requires a RetroAchievements username and API key, which can be acquired via [the RetroAchievements control panel](https://retroachievements.org/controlpanel.php).
```cfg
[Retro Achievements]
User=YOUR_USER_NAME
ApiKey=YOUR_API_KEY
```