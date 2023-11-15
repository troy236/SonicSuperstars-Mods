# Sonic Superstars Mods
This repository contains all the Mods I have written for Sonic Superstars

All the Mods use the BepInEx modding framework

# Always On Music
Stops the game turning off music/sound effects while in the background

# Debug View
Enables the Debug View showing Player Position/Memory Usage/Scene Name and more 

# Emerald Powers Boss
Stops the game turning off/deselecting your Emerald Power when entering a Boss Arena/beating the boss.

# Emerald Powers Music
Blocks the Emerald Power music. This includes Super form

# Emerald Powers Music No Long SE
Blocks the Emerald Power music and any long start-up effects. This is particularly noticeable with Vision/Slow/Ivy

# Speed Tracker
Enables the Speed Tracker showing the Players speed

This also works in some other locations such as the DRILL sections in Golden Carnival Act 1.

This does not prevent the game removing your Emerald Power when hit by a enemy

# Installation
1. Download the latest BepInEx Bleeding Edge Build Artifact - https://builds.bepinex.dev/projects/bepinex_be - Select the zip with the tag `BepInEx Unity (IL2CPP) for Windows (x64) games`
2. Unzip the contents to the Sonic Superstars game directory. You can locate this by opening Steam -> Right click Sonic Superstars -> Manage -> Browse local files
3. Run the game. This may take a couple of minutes for the first time. Once the game has fully launched then close the game - If the game UI appears briefly and then crashes that is fine too
4. Navigate to `{Game_Directory}\BepInEx\config` - Open BepInEx.cfg in a text editor
5. Change `UnityLogListening` value from `true` to `false`
6. Download the latest Sonic Superstars Mods zip from the `Releases` section
7. Extract the mods you wish to use to `{Game_Directory}\BepInEx\plugins`
8. Launch game. The mods should be active
