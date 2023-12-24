# Abandoned Building Remover Mod
 
Automatically demolishes abandoned buildings instantly.

##Installation - Thunderstore
If you do not wish to manually install and update the mod, you can download and install it with a mod manager from [Thunderstore.io](https://thunderstore.io/c/cities-skylines-ii/p/Wayzware/AbandonedBuildingRemover/).

## Installation - Manual
1. Install BepInEx 5 or 6, and download the correct version of the mod. The BepInEx 5 version is on Thunderstore, and both BepInEx 5 and 6 versions can be found on [the GitHub page](https://github.com/Wayzware/AbandonedBuildingRemover).

2. Run the game once, then close it. You can close it when the main menu appears

3. Download the mod from Thunderstore.io or the [release page](https://github.com/Wayzware/AbandonedBuildingRemover/releases). Make sure you select the download that is compatible with your version of BepInEx. Unzip it into the `Cities Skylines II/BepInEx/plugins` folder.

4. Launch the game, and your mods should be loaded automatically

## Compiling the Mod Yourself
You will need to add references to Unity yourself if you wish to compile the project.

In the .csproj, you can set the location of your game install, switch between BepInEx 5 and 6, and enable the PostBuild install step to automatically install the mod after build.
