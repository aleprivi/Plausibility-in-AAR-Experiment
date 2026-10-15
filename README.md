# D&D-MUSHRA+ — Plausibility in Audio Augmented Reality

Code and experimental software accompanying the paper:

> ### Screening Multiple Interacting Factors with D&D-MUSHRA+: a Pilot Study on Plausibility in Augmented Reality involving Hear-Through Technology
> TODO: Authors
> *TODO: Journal / Conference*, TODO: Year. DOI: [TODO](https://doi.org/TODO)

This repository contains the Unity app used for the study:

- **D&D-MUSHRA+** is a drag-and-drop extension of MUSHRA. With it, many rendering conditions can be compared side by side and rated in a single session. This makes it suited to **screening multiple interacting factors** quickly, before running full factorial tests.
- **Plausibility in Augmented Reality.** Participants stand in a real room next to a real loudspeaker. They judge whether virtual renderings of the same source can be told apart from the real one.
- **Hear-through technology.** Listeners wear Apple AirPods Pro in hear-through (transparency) mode. They hear the real loudspeaker through the earbuds, and the binaural virtual source is mixed on top of it.
- **Pilot study.** The app runs on an iPhone/iPad, using ARKit tracking and AirPods head tracking. The whole test, including playback and rating, happens on the tablet, in the room.

## Features

- **D&D-MUSHRA+:** a MUSHRA-style multi-stimulus test for touch screens. Participants rate each condition by dragging its token along a scale, and can play back any condition at any time. A classic slider-based MUSHRA, an elimination (ranking) procedure and SAQI attribute ratings are available in the same framework ([Assets/Scenes/MUSHRA_TESTs](Assets/Scenes/MUSHRA_TESTs)). See [Listening test GUI](#listening-test-gui-dd-mushra).
- **Multiple interacting factors:** each condition is a combination of up to eight rendering factors: HRTF, reverberation, level, headphone EQ, orientation tracking, position tracking, source elevation and stimulus. Factors are combined freely in a CSV file, so a single test can cover many factor combinations at once.
- **Condition switching at runtime:** selecting a condition re-configures the virtual source immediately, so listeners can switch between conditions, or back to the real reference, while staying in the room.
- **Hear-through AR:** with AirPods Pro in transparency mode, the real environment stays audible. Virtual sources are equalized for the AirPods Pro ([Assets/Audio/AudioEarPodsPro_Eq](Assets/Audio/AudioEarPodsPro_Eq)), and their loudness is matched to the real loudspeaker in a calibration step.
- **Room acoustics:** real-time Scattering Delay Network (SDN) reverberation with frequency-dependent wall materials and RT60 estimation ([Assets/SDNLib](Assets/SDNLib)).
- **Binaural rendering:** HRTF filtering with HRTFs from the [CIPIC database](https://www.ece.ucdavis.edu/cipic/spatial-sound/hrtf-data/) (subjects 050, 137, 165), switchable at runtime ([Assets/Resources](Assets/Resources)).
- **Tracking:** device pose from ARKit (AR Foundation) and listener head orientation from AirPods Pro via the HeadphoneMotion plugin.
- **Stimuli:** noise, speech and footsteps, with versions equalized for AirPods Pro ([Assets/Audio](Assets/Audio)).

## Listening test GUI: D&D-MUSHRA+

![TODO: screenshot of the D&D-MUSHRA+ interface](docs/dd-mushra.png)

In standard MUSHRA, each condition has its own slider. D&D-MUSHRA+ replaces the sliders with **draggable tokens on a shared scale**. All conditions on a page are therefore visible on the same axis at once, and listeners rate them by placing them relative to each other. Each token is also its play button. Together with instant switching between conditions, this lets participants compare many conditions quickly in the field. That is the requirement for screening several interacting factors in one session.

### How stimuli are presented

- **One source, many renderings.** A single virtual source is anchored in the AR scene at the position of the real loudspeaker (at ear height, 0°, or at floor level, −45°). Each button in the GUI is a condition. Tapping it applies that condition's rendering parameters to the source and restarts the stimulus. Listeners can therefore switch between conditions instantly, as often as they like, and in any order.
- **Real reference.** The *Ref* button (`Real = real`) mutes the virtual source, so the participant hears the physical loudspeaker. The tablet triggers the real playback over the network ([StartReal.cs](Assets/Scripts/0c.SourceTest/StartReal.cs)).
- **Pages and randomization.** Conditions are grouped into pages (*Step n/N*). Within each page, the order of the conditions is shuffled for every participant ([MUSHRASet.cs](Assets/Scenes/MUSHRA_TESTs/Lib/MUSHRASet.cs)).
- **Free head movement.** Rendering is updated continuously from the ARKit and AirPods tracking, so listeners can move and turn their head while comparing conditions.

### How stimuli are rated

| Interface | `mushraType` | Rating procedure | Used in |
| --- | --- | --- | --- |
| **D&D-MUSHRA+** | `DragAndDrop` | Each condition is a token that is dragged onto a horizontal scale. The horizontal position is the score: `score = (x − 50) · 100 / (screen width − 100)`, i.e. 0–100 with a 50 px margin. The vertical position is free, so tokens with similar scores do not overlap and remain easy to tap. Tapping a token plays it; dragging it rates it. | `3.2.MUSHRA Forum` (36 conditions, 4 pages × 9) |
| Elimination | `Elimination` | Listeners remove conditions one at a time, starting from the one least similar to the reference. The removal order gives the rank. | `3.3.MUSHRA CHItaly`, `3.3b.MUSHRA CHItaly2` (9 conditions) |
| SAQI | `SAQI` | One slider per condition, with one page per attribute from the Spatial Audio Quality Inventory: *Presence*, *Difference*, *Externalisation*, *Distance* ([SAQIdef.csv](Assets/Resources/SAQIdef.csv)). The layout is mirrored at random on each page. | `3.1.x.MUSHRA Slater …` (A/B comparisons) |
| Classic | `Classic` | Standard MUSHRA: a play button and a vertical slider (0–100) per condition. | — |

All interfaces work the same way otherwise:
- The participant cannot move on to the next page until every condition on the current page has been rated. Until then, an error panel is shown.
- On the last page, a confirmation panel asks the participant to confirm before the scene moves on.
- Every rating is appended immediately to `Log_<user>_<scene>.csv` (page, condition, x, y, score). If the app is restarted, the test resumes with the tokens in their previous positions.
- After confirmation, the final scores are written to `Results_<user>_<scene>_<date>.csv`.

### Defining a test

A test is a CSV file in [Assets/Resources](Assets/Resources) with one row per condition. For example, from [Forum.csv](Assets/Resources/Forum.csv):

```csv
page,condition,name,HRTF,Reverb,SampleVolume,HeadPhoneEQ,EarTrack,HeadTrack,SamplePos,SourceType,Real
1,1,A,personal,calibrated,calibrated,equalized,ear,head,0,noise,virtual
1,2,B,none,calibrated,calibrated,none,ear,head,0,noise,virtual
```

| Column | Values | Meaning |
| --- | --- | --- |
| `page`, `condition`, `name` | integers, label | Page, slot on the page, and condition ID used in the logs |
| `HRTF` | `personal`, `kemar`, `none` | HRTF set selected for the participant, CIPIC KEMAR (subject 165), or no HRTF |
| `Reverb` | `calibrated`, `large`, `none` | SDN room matched to the real room, a larger room, or no reverberation |
| `SampleVolume` | `calibrated`, `+5` | Loudness-matched level, or +5 dB |
| `HeadPhoneEQ` | `equalized`, `none` | AirPods Pro equalization on or off |
| `EarTrack` | `ear`, `ipadhead`, `ipad` | Orientation source: AirPods, tablet-estimated head, or tablet |
| `HeadTrack` | `head`, `head+height`, `ipad` | Position source for the listener in the AR scene |
| `SamplePos` | `0`, `-45` | Source elevation: ear height or floor level |
| `SourceType` | `noise`, `voice`, `ecological`, … | Stimulus |
| `Real` | `real`, `virtual` | Physical loudspeaker or virtual rendering |

To create a new test, duplicate a MUSHRA scene. In its `MUSHRAConfig` component, set the following fields:
- `conditionFilename` (the CSV file)
- `mushraType`
- `instructionMessage` (the question shown to the participant)
- `nextSceneName`

([MUSHRAConfig.cs](Assets/Scenes/MUSHRA_TESTs/MUSHRAConfig.cs))

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
| `3.1.x.MUSHRA Slater …` | SAQI ratings (source at 0° and −45°; noise, speech, steps) |
| [ForumAcousticumCHITALY](Assets/Scenes/ForumAcousticumCHITALY) `3.1a`–`3.2` | Training and **D&D-MUSHRA+** (similarity to the real reference) |
| [ForumAcousticumCHITALY](Assets/Scenes/ForumAcousticumCHITALY) `3.2c`–`3.3b` | Training and elimination test (perceived position compared with the reference) |

## Collected data

Responses are written as CSV files to the app's persistent data folder (`Application.persistentDataPath`) on the device. See [How stimuli are rated](#how-stimuli-are-rated) for the file format. Participants are identified only by a random ID (`U####`, see [WriteLogs.cs](Assets/Scripts/WriteLogs.cs)). To retrieve the files, enable *File Sharing* in the Xcode project or download the app container from Xcode.

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
  title   = {Screening Multiple Interacting Factors with {D\&D-MUSHRA+}: a Pilot Study on Plausibility in Augmented Reality involving Hear-Through Technology},
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
