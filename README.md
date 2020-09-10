# UnityInjector loader for BepInEx

This is UnityInjector loader for [BepInEx](https://github.com/BepInEx/BepInEx).

## Requirements

* BepInEx 5.3 or newer

All other dependencies (UnityInjector API, ExIni) are bundled in the loader.

## Installation

0. Install BepInEx [BepInEx](https://github.com/BepInEx/BepInEx/releases)
1. Exctract the archive into `BepInEx` folder. The files will be automatically put into their correct places
2. Run the game once to generate the config file
2. Modify the configuration file found in `BepInEx\config\org.bepinex.plugins.unityinjectorloader.cfg` if needed
3. Move UnityInjector plug-ins to the specified folder
4. Run the game

> ⚠️ **NOTE** ⚠️
> 
> You **must** remove the following files from `Sybaris` folder in order for this to work:
> * `UnityInjector.dll`
> * `ExIni.dll`
> * Any of UnityInjector patchers for Sybaris (i.e. `COM3D2.UnityInjector.Patcher.dll`)

## Building

To build, you will need the latest `BepInEx.dll`.  
ExIni is bundled in the source, so no need to get it.

The original UnityInjector is also not needed, as the loader implements a wrapper around the public API.
