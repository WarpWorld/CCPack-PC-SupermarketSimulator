# Supermarket Simulator

## Pack metadata

- **Game:** Supermarket Simulator
- **Crowd Control game ID:** `SupermarketSimulator`
- **Connector:** `SimpleTCPServerConnector`
- **Port:** `51337`
- **Mod frameworks:** BepInEx; MelonLoader

This repository contains the Crowd Control desktop pack, an archived BepInEx
payload (`mod\SupermarketSimulator-CC.zip`), and a newer
MelonLoader-oriented source project in `src\MelonLoaderExample`. These are
different loader layouts; do not combine files from both approaches.

## Requirements

- Supermarket Simulator.
- Crowd Control with the **Supermarket Simulator** pack selected.
- The loader required by the selected integration:
  - the archived payload contains a BepInEx layout;
  - the current source project references MelonLoader's IL2CPP assemblies.

## Setup

### Archived payload

Close the game, extract `mod\SupermarketSimulator-CC.zip` over the game
directory, preserving its `BepInEx` layout and loader files. Start Crowd
Control, select Supermarket Simulator, then launch the game.

### Building the current source

1. Install MelonLoader for Supermarket Simulator.
2. Set `GameBaseDir` in
   `src\MelonLoaderExample\MelonLoaderExample.csproj` to the game directory.
3. Build the project and install its output with the matching MelonLoader
   layout.

## Connection behavior

The game-side client connects to the Crowd Control server at
`127.0.0.1:51337`. It checks for the Crowd Control process and reconnects
through its local TCP client.

## Troubleshooting

- **The game fails to start after installation:** remove the mixed loader
  files, then reinstall only the chosen BepInEx archive or MelonLoader build.
- **The source project cannot resolve assemblies:** launch the game once after
  installing MelonLoader and confirm `GameBaseDir` points to its installation.
- **No connection:** start the Crowd Control desktop app and verify port
  `51337` is available locally.
