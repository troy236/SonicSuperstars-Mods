# Sonic Superstars Mods
This repository contains all the Mods I have written for Sonic Superstars

All the Mods use the BepInEx modding framework

# Debug View
Enables the Debug View showing Player Position/Memory Usage/Scene Name and more 

# Speed Tracker
Enables the Speed Tracker showing the Players speed

# Emerald Powers
Stops the game turning off/deselecting your Emerald Power when entering a Boss Arena/beating the boss.

This also works in some other locations such as the DRILL sections in Golden Carnival Act 1.

This does not prevent the game removing your Emerald Power when hit by a enemy

# Installation
1. Download BepInEx Bleeding Edge build - https://builds.bepinex.dev/projects/bepinex_be
2. Download the latest BepInEx Artifact selecting the zip with the tag `BepInEx Unity (IL2CPP) for Windows (x64) games`
3. Unzip the contents to the Sonic Superstars game directory. You can locate this by opening Steam -> Right click Sonic Superstars -> Manage -> Browse local files
4. Run the game. This may take a couple of minutes for the first time. Once the game has fully launched then close the game - If the game UI appears briefly and then crashes that is fine too
5. Navigate to `{Game_Directory}\BepInEx\config` - Open BepInEx.cfg in a text editor
6. Change `UnityLogListening` value from `true` to `false`
7. Download the latest Sonic Superstars Mods zip from the `Releases` section
8. Extract the mods you wish to use to `{Game_Directory}\BepInEx\plugins`
9. Launch game. The mods should be active
