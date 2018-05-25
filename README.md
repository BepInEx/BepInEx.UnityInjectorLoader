# UnityInjector loader for BepInEx

This is UnityInjector loader for [BepInEx](https://github.com/BepInEx/BepInEx).

## Requirements

* BepInEx 4.0+

All other dependencies (UnityInjector API, ExIni) are bundled in the loader.

## Installation

0. Install BepInEx [BepInEx](https://github.com/BepInEx/BepInEx/releases)
1. Put `BepInEx.UnityInjectorLoader.dll` into `BepInEx` folder
2. Add the following configuration option to `BepInEx\config.ini`:
    
    ```ini
    [org_bepinex_plugins_unityinjectorloader]
    ; Specifies the UnityInjector folder
    ; The location can be absolute or relative to the game's root folder
    ; Thus specifying `Sybaris\UnityInjector` will cause the loader to look for UnityInjector folder in Sybaris folder
    unityInjectorLocation=UnityInjector
    ```
3. Move UnityInjector plug-ins to the specified folder
4. Run the game

## Building

To build, you will need the latest `BepInEx.dll`.  
ExIni is bundled in the source, so no need to get it.

The original UnityInjector is also not needed, as the loader implements a wrapper around the public API.