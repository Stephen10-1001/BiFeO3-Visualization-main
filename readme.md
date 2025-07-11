# Unity BiFeO₃ Crystal Structure Visualizer

![Project Screenshot](https://raw.githubusercontent.com/[Your-Username]/[Your-Repo-Name]/main/Docs/screenshot.png)


A scientific visualization project built in Unity to procedurally generate and interact with the crystal structure of Bismuth Ferrite (BiFeO₃). This tool reads atomic coordinate data from a `.csv` file and renders a fully interactive 3D "ball-and-stick" model, showcasing its rhombohedrally distorted perovskite structure.

---

## Features

*   **Data-Driven Generation:** Procedurally generates the crystal model by reading atom types and 3D coordinates from a simple `.csv` file.
*   **Ball-and-Stick Model:** Renders atoms as spheres and chemical bonds as cylinders for an intuitive representation of the spatial structure.
*   **Element-Specific Properties:** Assigns unique colors and relative sizes to different elements (Bi, Fe, O) for easy identification.
*   **Dynamic Bonding Rules:** Dynamically generates different types of chemical bonds (Fe-O, O-O, Bi-O) based on adjustable, element-specific distance thresholds.
*   **Interactive Information Query:** Left-click any atom or bond to display its detailed information (element type, ID, 3D coordinates, bond length, etc.) on a UI panel.
*   **Highlighting System:** The currently selected object (atom or bond) is highlighted with a distinct material for clear visual focus.
*   **Visibility Toggling:** Use UI checkboxes to toggle the visibility of different bond types in real-time, allowing for focused analysis of specific structural features like the Fe-O octahedra.

---

## Tech Stack

*   **Engine:** Unity 2021.3 LTS (or newer)
*   **Language:** C#
*   **UI:** Unity UI + TextMeshPro

---

## Getting Started

### Prerequisites

*   You must have [Unity Hub](https://unity.com/download) installed.
*   Install **Unity Editor 2021.3 LTS** or a newer version via Unity Hub.

### Installation & Setup

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/[Your-Username]/[Your-Repo-Name].git
    ```
2.  **Open the project in Unity:**
    *   Launch Unity Hub.
    *   Click the `Open` button.
    *   Navigate to and select the cloned project folder.
    *   Unity will import the project, which may take a few moments.

3.  **Run the scene:**
    *   In the `Project` window, navigate to `Assets/Scenes/` and open the main scene file (e.g., `MainScene.unity`).
    *   In the `Hierarchy` window, select the `_CrystalGenerator` GameObject.
    *   In the `Inspector` window, verify that all public fields in the `Crystal Generator (Script)` component are correctly assigned:
        *   Ensure `Atom Prefab` and `Bond Prefab` are assigned from the `Assets/Prefabs/` folder.
        *   Ensure the `Data File` is assigned with `Assets/Data/BiFeO3_supercell.csv`.
        *   Ensure the `Parent` transforms are linked to their corresponding GameObjects in the `Hierarchy` (e.g., `Atoms`, `Fe-O-Bonds`).
    *   Similarly, check the `_InteractionManager` GameObject to ensure its UI elements and highlight material are linked.
    *   Press the **Play (▶)** button at the top of the editor to run the scene.

---

## Project Structure

```
/
├── Assets/
│   ├── Data/
│   │   └── BiFeO3_supercell.csv    # Atomic coordinate data source
│   ├── Prefabs/
│   │   ├── Atom.prefab             # Prefab for all atoms
│   │   └── Bond.prefab             # Prefab for all chemical bonds
│   ├── Scenes/
│   │   └── MainScene.unity         # The main Unity scene
│   └── Scripts/
│       ├── CrystalGenerator.cs     # Core script: reads data and builds the model
│       ├── AtomInfo.cs             # Component attached to each atom to store its data
│       ├── BondInfo.cs             # Component attached to each bond to store its data
│       └── InteractionManager.cs   # Handles all user input, highlighting, and UI display
├── Docs/
│   └── screenshot.png              # Project screenshots
└── README.md                       # This README file
```

---

## Roadmap

*   [ ] Implement an advanced orbit camera for smoother panning, zooming, and rotation.
*   [ ] Add UI sliders to adjust bond distance thresholds in real-time.
*   [ ] Develop a measurement tool to calculate distances between two atoms or angles between three atoms.
*   [ ] Add support for importing other standard crystal file formats (e.g., `.cif`).
*   [ ] Implement a feature to export the current model geometry (e.g., as an `.obj` file).

---

## License

This project is licensed under the [MIT License](LICENSE.md).
