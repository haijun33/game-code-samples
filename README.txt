Game development code samples - Haijun Yu
Haijun Yu | https://haijunyu.com/

EcoBattery: context and contribution
EcoBattery is my game project about recycling car batteries. I began exploring
programming with AI assistance during its later development. I integrated the
Godot implementation and tested the game in the engine.

These scripts were developed with AI assistance. They are learning examples,
not a claim of independently handwritten code. I want to strengthen my code
understanding, object-oriented programming, debugging and testing through a
structured application-development apprenticeship.

The current Godot repository has recorded commits from March 2026. This date
documents the Godot version; it does not establish the date of my first-ever
programming attempt.

Selected EcoBattery files
1. EcoBattery/scripts/autoload/save_manager.gd
   Saves and loads coins and the best record as JSON, and updates game progress.
2. EcoBattery/scripts/main_game/energy_cell.gd
   A small Sprite2D class that switches between filled and empty textures.
3. EcoBattery/scripts/main_game/battery_controller.gd
   Controls a falling battery, emits gameplay signals at card/depot zones,
   resets its state and updates its energy display.

Dependencies
These are unchanged excerpts from a larger Godot 4.x project, rather than a
standalone playable build. BatteryController references the project's GameData
and AudioManager autoloads. Texture and scene assets are not included.

Project presentation
https://haijunyu.com/eco-battery/

Floating Feast: collaborative project
Floating Feast is a collaborative Godot game. My work includes pixel art, UI,
scene and asset integration, and selected technical contributions. Coda-Wu is
a programming collaborator; I do not present the full game's code as solo work.
My workflow also uses AI coding assistance.

These two unchanged scripts have haijun33 as the recorded contributor in the
source file histories:
4. FloatingFeast/src/screens/windmill_blades.gd
   Exported rotation speed and frame-rate-independent rotation for windmill
   blades and gears, with pivot/scene-integration notes.
5. FloatingFeast/src/core/ember.gd
   Finds the player within the owning scene and follows it with a shoulder
   offset, interpolation and bobbing. Follow behaviour can be disabled.

The Floating Feast excerpts depend on Godot nodes and the original scenes.
They are code-reading examples rather than a standalone playable build.

No game artwork, third-party Unity packages or account credentials are included
in this sample folder.


Banling - Unity UI C# samples
Haijun Yu

These two components are selected from my AI-assisted implementation work on
Banling, a client game project. The source histories record haijun33 as their
contributor. They demonstrate a link between interface design and engine code.

IconBarAutoFit.cs
Adapts a vertical icon bar to the available canvas height by reducing spacing
and then scaling within configured limits. Recalculates when relevant layout
inputs change. Dependencies: UnityEngine and UnityEngine.UI.

GramophoneArtworkAnimation.cs
Uses playback state to rotate a record and smoothly move a tonearm between
playing, paused and idle states. Configuration comes from the UI prefab.
Dependency: UnityEngine.

The files are unchanged project excerpts, not a complete playable build.
AI assistance was used in the implementation workflow; I am not claiming that
all code in the wider client project was written independently by me.

These two client-project excerpts are shared with project permission, confirmed
by Haijun Yu before publication.
