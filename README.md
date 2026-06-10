# ReadAllMessages

A tiny quality-of-life mod for **Schedule I** that adds a **"Read All"** button to the Messages app on your phone. One click marks every conversation as read and clears the unread notification badge. That's it — no gameplay changes, no automation, nothing else.

Tired of tapping through 30 dealer texts just to get rid of the notification dot? This is for you.

## Features

- Adds a blue **Read All** button in the bottom-right corner of the Messages app (conversation list screen)
- Marks **all** conversations as read with a single click
- Hotkey fallback: press **F8** to do the same from anywhere
- Local-only — does not touch game saves' logic or network state, safe to use in multiplayer
- Safe to install and remove at any time

## Requirements

- **Schedule I** (Steam, default/main branch — IL2CPP)
- **[MelonLoader](https://melonwiki.xyz/#/?id=automated-installation) 0.7.0 or newer**

## Installation

1. Install MelonLoader into Schedule I (run the MelonLoader installer, select `Schedule I.exe`).
2. Launch the game once so MelonLoader finishes setting up, then quit.
3. Drop `ReadAllMessages.dll` into the `Mods` folder inside your game directory:
   `...\steamapps\common\Schedule I\Mods\`
4. Launch the game. You should see `ReadAllMessages` listed in the MelonLoader console on startup.

## Usage

1. Load your save.
2. Open your phone and go to the **Messages** app.
3. Click the blue **Read All** button in the bottom-right corner.
4. All conversations are instantly marked as read and the notification badge disappears.

Alternatively, press **F8** at any time in-game to mark everything as read without opening the phone.

## Troubleshooting

- **No button in the Messages app?** Check the MelonLoader console/`MelonLoader\Latest.log` for `ReadAllMessages` messages. The mod waits up to 2 minutes after the save loads for the Messages app to initialize. The F8 hotkey works even if the button could not be created.
- **Game on the `alternate` (Mono) beta branch?** This build targets the default IL2CPP branch. Open an issue if you need a Mono build.
- **Something broke after a game update?** Game updates can change internal class names. Open an issue and attach `MelonLoader\Latest.log`.

## Notes

- The mod only calls the game's own "mark as read" function for each conversation — exactly what happens when you open a conversation manually.
- Read state is stored in your save like normal. No files outside the game are touched.

## Credits

Made by **Mangol**.
