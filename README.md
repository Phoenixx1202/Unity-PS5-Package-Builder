# Ready-to-Use Unity PS5 Package Builder

This is a precompiled Windows x64 toolkit for building a Unity PS5 player and creating a PKG. Recipients do not need the .NET SDK and do not need to compile the package builder. Copy the included folders and files to a Unity project root, configure `param.json`, and build.

## What is included

- `Assets/Editor/BuildPs5Package.cs` adds the **PS5** menu to Unity.
- `Tools/BuildPs5Pkg/BuildPs5Pkg.exe` is the complete single-file builder with its runtime and dependencies embedded.
- `build_Pkg.bat` runs the precompiled builder.
- `param.json` is the package metadata template.

The toolkit uses only paths relative to the Unity project root. Do not rename or move `Tools/BuildPs5Pkg/BuildPs5Pkg.exe`.

## Requirements

- Windows x64.
- Unity with PlayStation 5 platform support.
- The licensed PS5 SDK and Unity PS5 integration required to produce `eboot.bin`.
- A Unity project with at least one build scene.

The package builder itself is already compiled as one self-contained executable and does not require Visual Studio, the .NET SDK, or a separate builder SDK. At runtime, it automatically extracts its embedded native publishing component to the current Windows user's temporary directory. Unity and the licensed PS5 platform components cannot be embedded in this toolkit; each recipient must have authorized access to them.

The included third-party package and publishing libraries may have separate license terms. Confirm that you have redistribution rights before sharing the toolkit outside your authorized team.

## Install

Copy these items to the root of the target Unity project while preserving their relative paths:

```text
Assets/Editor/BuildPs5Package.cs
Assets/Editor/BuildPs5Package.cs.meta
Tools/
build_Pkg.bat
param.json
README.md
```

The final layout must look like this:

```text
YourUnityProject/
|-- Assets/
|   `-- Editor/
|       |-- BuildPs5Package.cs
|       `-- BuildPs5Package.cs.meta
|-- Tools/
|   `-- BuildPs5Pkg/
|       `-- BuildPs5Pkg.exe
|-- build_Pkg.bat
|-- param.json
`-- README.md
```

## Configure package metadata

Edit these values in `param.json` before the first build:

```json
{
  "contentId": "UP0000-PPSA99999_00-MYGAMEEXAMPLE000",
  "titleId": "PPSA99999",
  "localizedParameters": {
    "defaultLanguage": "en-US",
    "en-US": {
      "titleName": "My Game"
    }
  }
}
```

The title ID must be `PPSA` followed by five digits. The content ID must use the format `UP0000-TITLEID_00-` followed by exactly 16 uppercase letters or digits. Keep `"attribute": 536870912` unchanged.

## One-click build from Unity

1. Open **File > Build Settings**.
2. Switch the active platform to **PlayStation 5**.
3. Disable **Development Build**.
4. Add at least one scene to **Scenes In Build**, or save the active scene.
5. Select **PS5 > 3. Full Build (Player + PKG)**.
<img width="657" height="137" alt="image" src="https://github.com/user-attachments/assets/80807b37-fc6c-4185-973e-4e2c2506186e" />


Unity always writes the player into a clean `build/Build/` directory. Before each build, only that generated directory is deleted and recreated. The precompiled builder writes the PKG separately under `build/Build-pkg/`.

Never select the `build/` root as the Unity PS5 build folder. It may contain `Build-pkg/` and Unity will reject it with an **Invalid Build Folder** error. When building manually, always select `build/Build/`.

## Create a PKG from an existing Unity player

Build the Unity player into `build/Build`, then run `build_Pkg.bat` from the project root. No source compilation occurs.
