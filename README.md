


# Unity Grid Placement 3D - Plcement, Roate, Delete, Visualize

## Overview
This project is a comprehensive implementation of a 3D grid placement system. It provides dynamic and precise placement of objects within a 3D space, leveraging a grid-based system to ensure accuracy and alignment. This tool can be particularly useful in game development, architectural visualization, and any application requiring spatial organization.

### Toggle off Cell Visualizing (Preview)
https://github.com/Litt1eStar/Grid-Placement-3d-Completed-Version/assets/90139527/21060a78-17d7-43a3-8548-92d5c7084549

## Features
- **3D Grid Visualization**: A clear and adjustable 3D grid that provides a reference for object placement.
- **Object Placement, Rotation, and Deletion**: Manage grid objects through placement, directional rotation, and deletion.
- **Visual Feedback**: Provides visual feedback to indicate if an object can be placed.

## Installation
1. **Clone the repository**:
    ```sh
    git clone https://github.com/Litt1eStar/Grid-Placement-3d-Completed-Version.git
    ```
2. **Open the project in Unity**: Ensure you are using Unity version 2020.3 or later for compatibility.
3. **Load the main scene**: Open the `Playground.unity` file to get started.

## Usage
1. **Grid Initialization**: The grid is initialized automatically when the scene loads. Configure grid size and spacing through the `GameManager` script.
2. **Placing Objects**: Select an object from the object palette and click on the grid to place it. The object will snap to the grid position.
3. **Object Manipulation**: Use on-screen controls or keyboard shortcuts for:
   - **Placement**: Place an object on the grid.
   - **Rotate**: Rotate the direction of the object before placement.
   - **Delete**: Delete an object from the grid if it exists.
4. **Grid Visualizer**: Visualize feedback for object manipulation and toggle grid cell rendering as needed.

## Main Systems

### Grid System
The **Grid System** is the backbone of the project, responsible for:
- **Grid Creation**: Generates a 3D grid based on specified dimensions and spacing.
- **Grid Visualization**: Renders the grid lines within the 3D space to aid in object placement.
- **Grid Configuration**: Allows customization of grid parameters.

### Object Manipulation System
The **Object Manipulation System** handles the following:
- **Object Placement**: Users can place objects on the grid.
- **Object Rotation**: Users can rotate objects before placing them on the grid.
- **Object Deletion**: Users can delete objects from the grid.
- **Placement Validation**: Checks if a position is valid (e.g., not occupied) before placing an object.

### Placement Validation System
The **Placement Validation System** ensures objects do not overlap by:
- **Grid Data Checks**: Performing real-time checks during object placement and movement by check data of grid.
- **Feedback**: Providing visual feedback if an object cannot be placed.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE.txt) file for details.

## Authors
- **Litt1eStar**

## Asset Source
- **Road Model**: https://assetstore.unity.com/packages/3d/environments/roadways/modular-lowpoly-track-roads-free-205188
- **Building Model**: https://assetstore.unity.com/packages/3d/props/exterior/low-poly-houses-free-pack-243926
## Acknowledgements
This project has been a part of my learning and practice in game system creation. Feel free to ask anything you want to know.

