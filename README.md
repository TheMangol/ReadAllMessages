# ReadAllMessages

*Read this in other languages: [Русский](README.ru.md)*

A tiny quality-of-life mod for **Schedule I** that adds a **"Read All"** button to the Messages app on your phone. One click marks every conversation as read and clears the unread notification badge. That's it — no gameplay changes, no automation, nothing else.

Tired of tapping through 30 dealer texts just to get rid of the notification dot? This is for you.

## Features

- Adds a blue **Read All** button in the Messages app, in the header bar next to the *Messages* title
- Marks **all** conversations as read with a single click
- Hotkey fallback: press **F8** to do the same from anywhere
- Local-only — does not touch save logic or network state, safe to use in multiplayer
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

> Get the DLL from the [latest release](https://github.com/TheMangol/ReadAllMessages/releases/latest), or build it yourself (see below).

## Usage

1. Load your save.
2. Open your phone and go to the **Messages** app.
3. Click the blue **Read All** button in the header (next to the *Messages* title).
4. All conversations are instantly marked as read and the notification badge disappears.

Alternatively, press **F8** at any time in-game to mark everything as read without opening the phone.

## Building from source

You only need the **.NET SDK 6.0** (or newer) — plus a MelonLoader-patched copy of the game for the reference assemblies.

1. Run the game with MelonLoader at least once and let it reach the main menu. This generates the proxy assemblies in `Schedule I\MelonLoader\Il2CppAssemblies\` that the build needs.
2. Open `src/ReadAllMessages/ReadAllMessages.csproj` and check the `<GamePath>` line — it must point to your game folder (default: `C:\Program Files (x86)\Steam\steamapps\common\Schedule I`).
3. Build from the `src/ReadAllMessages` folder:

   ```bash
   dotnet build -c Il2Cpp   # default/main Steam branch (IL2CPP)
   dotnet build -c Mono     # alternate (Mono) beta branch
   ```

4. If `GamePath` contains a `Mods` folder, the built `ReadAllMessages.dll` is copied there automatically. Otherwise grab it from `bin\Il2Cpp\` (or `bin\Mono\`) and drop it into `Schedule I\Mods\` manually.

## Troubleshooting

- **No button in the Messages app?** Check the MelonLoader console / `MelonLoader\Latest.log` for `ReadAllMessages` messages. The mod waits up to 2 minutes after the save loads for the Messages app to initialize. The F8 hotkey works even if the button could not be created.
- **Build fails — no Il2CppAssemblies?** Run the game with MelonLoader once and wait for the main menu, then build again.
- **Reference assemblies named differently?** Open `Schedule I\MelonLoader\` and confirm `net6\MelonLoader.dll` and `Il2CppAssemblies\Assembly-CSharp.dll` exist. If the paths differ, adjust them in the `.csproj`.
- **Game on the `alternate` (Mono) beta branch?** The released DLL targets the default IL2CPP branch; build from source with `-c Mono` for the Mono branch.
- **Something broke after a game update?** Game updates can change internal class names. Open an issue and attach `MelonLoader\Latest.log`.

## Notes

- The mod only calls the game's own "mark as read" function for each conversation — exactly what happens when you open a conversation manually.
- Read state is stored in your save like normal. No files outside the game are touched.
- One source, two branches: game types are referenced as `ScheduleOne.*` (Mono) and `Il2CppScheduleOne.*` (IL2CPP), selected at compile time.

## Credits

Made by **Mangol**.
