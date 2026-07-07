# TheCube - VR Room Escape Game with Puzzle

Final VR Project is a Unity-based VR puzzle adventure. The player explores a sequence of puzzle rooms and solves each challenge through direct VR interactions such as grabbing objects, rotating puzzle pieces, placing items into sockets, pressing buttons, and teleporting through the environment.

The project is built around Unity's XR Interaction Toolkit and OpenXR. It uses controller ray interaction, grab interaction, socket snapping, teleportation, input actions, spatial audio, and editor-side XR simulation to create an immersive VR puzzle experience.

## Project Overview

This project is designed as a room-based VR puzzle experience. Each room focuses on a different type of interaction so that the player must observe the environment, manipulate objects in 3D space, and use VR movement mechanics to progress.

The main gameplay flow is:

1. Start the game from the `Start` scene.
2. Solve the pipe connection puzzle in `PipePuzzleRoom`.
3. Complete the shape-order teleportation puzzle in `TeleportPuzzleRoom`.
4. Solve the button combination puzzle in `ButtonPuzzelRoom`.

## Key Features

- VR controller-based object interaction
- Long-range object targeting through XR Ray Interactors
- Physical object manipulation through XR Grab Interactables
- Puzzle-piece placement through XR Socket Interactors
- Teleportation-based room navigation and puzzle rules
- Button selection, answer validation, and reset logic
- Visual feedback through material color changes
- Audio feedback for interaction, success, and failure states
- Clear UI display after puzzle completion
- XR Device Simulator support for testing without a physical headset

## VR Technology

### XR Interaction Toolkit

The project uses Unity's XR Interaction Toolkit as the main framework for VR interaction. Instead of building custom low-level controller logic, the project relies on Unity's interaction system to connect interactors, interactables, input actions, hover events, select events, and teleportation events.

The main XR Interaction Toolkit components used in this project include:

- `XRRayInteractor`: Allows the player to point at distant objects with a controller ray. This is used for selecting interactable objects and identifying the currently hovered pipe piece.
- `XRGrabInteractable`: Allows the player to grab and move pipe puzzle pieces in 3D space.
- `XRSocketInteractor`: Creates socket positions on the pipe puzzle board. When a pipe piece is placed into a socket, it snaps into the board and becomes part of the puzzle grid.
- `XRSimpleInteractable`: Handles simple select interactions for puzzle buttons.
- `BaseTeleportationInteractable`: Provides teleportation targets used by the teleport puzzle.

The project uses event-driven interaction patterns. For example, button objects listen to `selectEntered` events, pipe sockets listen to `selectEntered` and `selectExited` events, and teleportation targets listen to `teleporting` events. This keeps the puzzle logic connected to VR interactions without requiring constant manual polling for every object.

### OpenXR and XR Management

OpenXR is used as the main XR runtime layer. OpenXR provides a common interface between Unity and supported VR platforms, which helps the project remain more portable across different devices and runtimes.

The project includes XR-related packages such as:

- OpenXR Plugin
- XR Management
- Meta OpenXR
- Android XR OpenXR
- XR Core Utilities
- XR Hands

XR Management is responsible for loading the correct XR loader for the selected build target. The project also includes OpenXR and simulation-related settings under the `Assets/XR` and `ProjectSettings` folders.

### Controller Input and Unity Input System

The project uses Unity's Input System to process VR controller actions. In the pipe puzzle, input actions are connected to pipe rotation controls. When the player is hovering over or selecting a pipe piece, the rotation action can rotate that pipe left or right.

The `PipeRotationController` script reads both XR input actions and keyboard fallback inputs for simulation. This allows the same puzzle logic to be tested in two contexts:

- Physical VR device input through controller action bindings
- Editor testing through XR Device Simulator and keyboard keys

### Grab, Snap, and Rotate Interaction

The pipe puzzle demonstrates a core VR interaction loop:

1. The player targets a pipe piece with the controller.
2. The player grabs the pipe using XR Grab interaction.
3. The player places the pipe into a socket on the board.
4. The socket snaps the pipe into a fixed grid position.
5. The player rotates the pipe while it is selected or hovered.
6. The puzzle system checks whether all pipe openings form a valid route.

The `PipePuzzlePiece` script stores each pipe's open directions and recalculates those directions based on the current rotation step. The `PipePuzzleLogic` script then treats the board as a grid and uses a graph search approach to determine whether the entry point is connected to the exit point.

### Teleportation Interaction

Teleportation is used not only as a movement method but also as a puzzle mechanic. The `TeleportPuzzleSequenceRule` script listens to teleportation events and checks whether the player teleports through shapes in the expected order.

The current teleportation sequence is:

```text
Ractangle -> Triangle -> Circle
```

If the player teleports to the wrong shape or touches the floor, the sequence resets and the player is moved back to the reset position. This turns VR locomotion into a rule-based puzzle instead of using it only for navigation.

### Spatial Interaction Feedback

The project uses several feedback methods to make VR interactions easier to understand:

- Button color changes show selected, correct, and incorrect states.
- Press animations make buttons feel physically responsive.
- Audio clips provide immediate feedback for presses, success, and failure.
- Clear UI is positioned in front of the player's camera after puzzle completion.
- Pipe placement and rotation are validated immediately after relevant interaction events.

These feedback systems are important in VR because the player needs clear confirmation that their hand/controller actions have been recognized.

### XR Device Simulator

The project includes XR Device Simulator sample assets. This makes it possible to test parts of the VR experience in the Unity Editor without immediately deploying to a headset.

For pipe rotation testing in the simulator:

- `X`: Rotate the hovered or selected pipe to the right
- `C`: Rotate the hovered or selected pipe to the left

This is useful during development because puzzle logic, socket behavior, ray interaction, and scene transitions can be tested faster before full device testing.

## Scene Structure

| Scene | Description |
| --- | --- |
| `Assets/01_Scenes/Start.unity` | Start scene. The start button loads the pipe puzzle room. |
| `Assets/01_Scenes/PipePuzzleRoom.unity` | Pipe puzzle room where the player places and rotates pipe pieces to connect the entry and exit. |
| `Assets/01_Scenes/TeleportPuzzleRoom.unity` | Teleportation puzzle room where the player must move through shapes in a specific order. |
| `Assets/01_Scenes/ButtonPuzzelRoom.unity` | Button puzzle room where the player selects the correct button combination. |

## Main Scripts

| Script | Role |
| --- | --- |
| `Assets/02_Scripts/Start/Start.cs` | Loads `PipePuzzleRoom` when the start button is pressed. |
| `Assets/02_Scripts/PipePuzzle/PipePuzzleLogic.cs` | Checks the pipe connection state and loads the next scene when solved. |
| `Assets/02_Scripts/PipePuzzle/PipePuzzlePiece.cs` | Handles pipe rotation, socket snapping behavior, and open-direction calculation. |
| `Assets/02_Scripts/PipePuzzle/PipeRotationController.cs` | Rotates the currently hovered or selected pipe using controller input or simulator keys. |
| `Assets/02_Scripts/TeleportPuzzleRoom/TeleportPuzzleSequenceRule.cs` | Validates the teleportation shape sequence and resets the player after mistakes. |
| `Assets/02_Scripts/TeleportPuzzleRoom/CakeRabbitTrigger.cs` | Loads the button puzzle room when the player reaches the target trigger. |
| `Assets/02_Scripts/ButtonPuzzel/ButtonManager.cs` | Manages selected buttons, answer checking, success feedback, and failure reset behavior. |
| `Assets/02_Scripts/ButtonPuzzel/SingleButton.cs` | Handles individual button selection, press animation, color state, and press sound. |

## Development Environment

- Unity: `6000.3.10f1`
- Render Pipeline: Universal Render Pipeline `17.3.0`
- XR Interaction Toolkit: `3.4.1`
- XR Hands: `1.7.3`
- OpenXR Plugin: `1.16.1`
- XR Management: `4.5.4`
- Input System: `1.18.0`

## Installation and Run Instructions

1. Install Unity `6000.3.10f1` or a compatible Unity 6 version through Unity Hub.
2. Add this repository to Unity Hub using `Add project from disk`.
3. Open the project and wait for Unity to restore packages.
4. Check that the following scenes are included in `File > Build Profiles` or `Build Settings`:
   - `Assets/01_Scenes/ButtonPuzzelRoom.unity`
   - `Assets/01_Scenes/Start.unity`
   - `Assets/01_Scenes/PipePuzzleRoom.unity`
   - `Assets/01_Scenes/TeleportPuzzleRoom.unity`
5. For editor testing, open `Start.unity` or a specific puzzle room scene and enter Play Mode.
6. For headset testing, verify the OpenXR runtime and device-specific XR settings before building.

## Controls

### VR Headset

- Aim at objects using the controller ray.
- Use Grab or Select input to pick up pipe pieces or press buttons.
- Place pipe pieces into socket positions on the puzzle board.
- Use rotation input to adjust pipe direction.
- Select teleportation targets to move through the room.

### XR Device Simulator

The project includes XR Device Simulator support for editor-side testing. Pipe rotation can be tested with the following keys:

- `X`: Rotate the hovered or selected pipe to the right
- `C`: Rotate the hovered or selected pipe to the left

## Puzzle Descriptions

### Pipe Puzzle

The player grabs pipe pieces, places them into board sockets, and rotates them until the entry and exit are connected. Each pipe stores its available open directions. After placement or rotation, the logic checks whether the board forms a valid connected route from the entry slot to the exit slot.

When the puzzle is solved, the project loads `TeleportPuzzleRoom`.

### Teleport Puzzle

The player must teleport through shapes in the correct order. The current rule is:

```text
Ractangle -> Triangle -> Circle
```

If the player chooses an incorrect shape or touches the floor, the sequence resets and the player is moved back to the reset position. After completing the room and reaching the trigger object, the project loads `ButtonPuzzelRoom`.

### Button Puzzle

The player selects a set of buttons that match the correct answer. Selected buttons change color. If the selected set matches the answer, the puzzle plays a success sound and displays the clear UI. If the selected set is incorrect, the buttons turn red, a failure sound plays, and the selection resets after a short delay.

## Folder Structure

```text
Assets/
  01_Scenes/        Main game scenes
  02_Scripts/       Puzzle, scene transition, and interaction scripts
  03_Prefabs/       Button, pipe, and teleport-related prefabs
  04_Images/        Hint and UI image resources
  05_Models/        Model and material resources
  06_Sounds/        Puzzle and background audio
  07_Material/      Additional materials
  XRI/              XR Interaction Toolkit settings
  XR/               XR Management, OpenXR, and simulation settings
```

## Notes

- Some names, such as `ButtonPuzzel` and `Ractangle`, appear to contain spelling mistakes. They are currently used by scene references, tags, file names, or scripts, so changing them requires updating all related references together.
- `Library`, `Temp`, and `Logs` are Unity-generated local cache folders and are usually excluded from version control.
