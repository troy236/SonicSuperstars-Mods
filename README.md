# Sonic Superstars Mods
This repository contains all the Mods I have written for Sonic Superstars

All the Mods use the BepInEx modding framework

# Always On Music
Stops the game turning off music/sound effects while in the background

# Debug View
Enables the Debug View showing Player Position/Memory Usage/Scene Name and more 

# Emerald Powers Boss
Stops the game turning off/deselecting your Emerald Power when entering a Boss Arena/beating the boss.

This also works in some other locations such as the DRILL sections in Golden Carnival Act 1.

This does not prevent the game removing your Emerald Power when hit by a enemy

# Emerald Powers Cancellable
Adds the ability to cancel Emerald Powers while in-use.

This acts the same as cancelling Super Form

# Emerald Powers Music
Blocks the Emerald Power music.
Modify `EmeraldMusicConfig.txt` to configure which Emerald Power sounds play

# Knuckles Glide Speed
Allows customising Knuckles Glide Speed

Modify `KnucklesGlideSpeedConfig.txt` to configure

Default: Glide Speed being your X Speed at the start of the glide.

With the default the minimum speed is 30 as shown on Speed Tracker even if you were moving slower than that at the start of the glide

# Player Warp
Adds the ability to warp to a custom position or cycle through available checkpoints with a button combo

Modify `PlayerWarpConfig.txt` to configure

Default button combo is ZL,ZR,RUp

# Press Factory Disable Drone
Disables the Drone. It will only follow you and not charge up

# Quick Death
Adds a customisable Quick Death/Restart button combo.
You can have both a Quick Death and Quick Restart combo or just one of them.

Defaults:
ZL,ZR,L,R for Quick Death
ZL,ZR,RDown,RLeft for Quick Restart

# Remove Induction Emerald
Removes the context-sensitive Emerald usage prompt

# Remove Voxel Player
Removes the Player Voxel model from Cyber Station

Modify `RemoveVoxelPlayerConfig.txt` to configure

Default is the main Voxel model being removed. Jellyfish/Mouse/Rocket will show

# Speed Tracker
Enables the Speed Tracker showing the Players speed

# Spin Dash Fix
Fixes the Spin dash charge at higher framerates

# Installation
1. Download the latest BepInEx Bleeding Edge Build Artifact - https://builds.bepinex.dev/projects/bepinex_be - Select the zip with the tag `BepInEx Unity (IL2CPP) for Windows (x64) games`
2. Unzip the contents to the Sonic Superstars game directory. You can locate this by opening Steam -> Right click Sonic Superstars -> Manage -> Browse local files
3. Run the game. This may take a couple of minutes for the first time. Once the game has fully launched then close the game - If the game UI appears briefly and then crashes that is fine too
4. Navigate to `{Game_Directory}\BepInEx\config` - Open BepInEx.cfg in a text editor
5. Change `UnityLogListening` value from `true` to `false`
6. Download the latest Sonic Superstars Mods zip from the `Releases` section
7. Extract the mods you wish to use to `{Game_Directory}\BepInEx\plugins`
8. Launch game. The mods should be active
