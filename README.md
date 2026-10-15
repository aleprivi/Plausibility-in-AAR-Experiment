# Plausibility in Audio Augmented Reality — Experiment

Unity project used to run the listening experiments on the **plausibility of virtual sound sources in Audio Augmented Reality (AAR)** described in our paper:

> **TODO: Paper title**
> TODO: Authors
> *TODO: Journal / Conference*, TODO: Year. DOI: [TODO](https://doi.org/TODO)

The app runs on an iPhone/iPad and renders binaural audio over Apple AirPods Pro. It combines visual tracking (ARKit), head tracking from the AirPods motion sensors, HRTF-based spatialization and a Scattering Delay Network (SDN) room-acoustics model. Participants rate virtual sources against real ones using MUSHRA-style and plausibility (Slater-style) test interfaces.

## Features

- **Room acoustics:** real-time Scattering Delay Network (SDN) reverberation with frequency-dependent wall materials and RT60 estimation ([Assets/SDNLib](Assets/SDNLib)).
- **Binaural rendering:** HRTF filtering with HRTFs from the [CIPIC database](https://www.ece.ucdavis.edu/cipic/spatial-sound/hrtf-data/) (subjects 050, 137, 165), switchable at runtime ([Assets/Resources](Assets/Resources)).
- **Tracking:** device pose from ARKit (AR Foundation) and listener head orientation from AirPods Pro via the HeadphoneMotion plugin.
- **Test procedures:** localization accuracy test, loudness calibration, training, plausibility test and MUSHRA-style tests (classic, drag-and-drop, elimination and SAQI interfaces) ([Assets/Scenes/MUSHRA_TESTs](Assets/Scenes/MUSHRA_TESTs)).
- **Stimuli:** noise, speech and footsteps, with versions equalized for AirPods Pro ([Assets/Audio](Assets/Audio)).

## Requirements

- **Unity 2021.3.7f1** (LTS) with the iOS Build Support module
- macOS with Xcode, to build and deploy to iOS
- An ARKit-capable iPhone or iPad running iOS 14 or later (headphone motion requires iOS 14)
- Apple AirPods Pro (or other headphones that support `CMHeadphoneMotionManager`)

Unity resolves the package dependencies (AR Foundation 4.2.3, ARKit 4.2.3, TextMesh Pro, [unity-webview](https://github.com/gree/unity-webview)) from [Packages/manifest.json](Packages/manifest.json) when you first open the project.

## Getting started

1. Clone the repository:
   ```bash
   git clone https://github.com/aleprivi/Plausibility-in-AAR-Experiment.git
   ```
2. Open the folder with Unity Hub using Unity 2021.3.7f1.
3. Switch the platform to iOS (*File → Build Settings → iOS → Switch Platform*).
4. Build. Unity generates an Xcode project. Open it, set your signing team and bundle identifier, then run it on the device.

The scenes are already listed in the Build Settings. The first scene is the entry point of the experiment.

## Experiment structure

The scenes in [Assets/Scenes](Assets/Scenes) are numbered in the order they are presented:

| Scenes | Purpose |
| --- | --- |
| [CORRECT/00.START](Assets/Scenes/CORRECT/00.START.unity), `0a1.ExperimentMenu`, `0a2.TestMenu` | Participant setup (ID, HRTF set) and menus |
| `0b.AccuracyTest`, `0e.RotateAroundTest` | Tracking and localization checks |
| `0d.LoudnessCalibration`, [Calibration Scenes](Assets/Scenes/Calibration%20Scenes) | Loudness matching between real and virtual sources |
| `1.ISMAR Exp_Training`, `2.ISMAR Exp_SlaterExperiment` | Training and plausibility experiment |
| `3.1.x.MUSHRA Slater …` | MUSHRA tests (source at 0° and −45°; noise, speech, steps) |
| [ForumAcousticumCHITALY](Assets/Scenes/ForumAcousticumCHITALY) | Instructions, training and MUSHRA sessions of the later study |

The conditions of each test are defined in the CSV files in [Assets/Resources](Assets/Resources) (e.g. `Slater1.csv`–`Slater6.csv`, `Forum.csv`, `CHItaly.csv`, `SAQIdef.csv`).

## Collected data

Responses are written as CSV files to the app's persistent data folder (`Application.persistentDataPath`) on the device. Participants are identified only by a random ID (`U####`, see [WriteLogs.cs](Assets/Scripts/WriteLogs.cs)). To retrieve the files, enable *File Sharing* in the Xcode project or download the app container from Xcode.

> **Note:** some lab-specific scripts ([SendToPrivi.cs](Assets/Scripts/Utils/SendToPrivi.cs), [StartReal.cs](Assets/Scripts/0c.SourceTest/StartReal.cs)) send HTTP requests to a server used during our sessions to control the real loudspeaker and to monitor object positions. They are not needed to run the experiment elsewhere. Disable them, or point them to your own server.

## Repository layout

```
Assets/
├── SDNLib/          Scattering Delay Network and HRTF rendering
├── Scripts/         Experiment logic (procedure, calibration, training, tracking, GUI, logging)
├── Scenes/          Experiment scenes and MUSHRA test framework
├── Resources/       HRTF sets (CIPIC) and test-condition CSV files
├── Audio/           Stimuli and calibration signals
├── Hear XR/         HeadphoneMotion plugin (AirPods head tracking)
├── RequiredLibs/    AForge.NET / AI4Unity libraries
└── RoomPrefabs/, Materials/, Images/, 3DTOYS/, XR/
```

## Citation

If you use this code, please cite:

```bibtex
@article{TODO,
  title   = {TODO},
  author  = {TODO},
  journal = {TODO},
  year    = {TODO},
  doi     = {TODO}
}
```

## License

TODO: choose a license and add a `LICENSE` file (e.g. MIT for the code).

### Third-party components

This repository includes third-party material under its own terms:

- **HeadphoneMotion** by Anastasia Devana: MIT License ([Assets/Hear XR/HeadphoneMotion/LICENSE](Assets/Hear%20XR/HeadphoneMotion/LICENSE))
- **AForge.NET** libraries: LGPL v3 ([aforgenet.com](http://www.aforgenet.com/framework/license.html))
- **CIPIC HRTF database**: V. R. Algazi, R. O. Duda, D. M. Thompson and C. Avendano, "The CIPIC HRTF Database," *Proc. IEEE WASPAA*, 2001
- **3D toy models** by Psionic Games: free to use, but not to resell ([readme](Assets/3DTOYS/readme.txt))
- **unity-webview** by GREE, Inc.: Apache License 2.0

## Acknowledgements

Developed at the University of Padova and the University of Udine.

## Contact

TODO: corresponding author name and email
