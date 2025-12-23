# 🛠️ WorkHammer

A high-fidelity, professional job application tracker designed for developers. Built with **C#**, **Avalonia UI**, and **FluentAvalonia**, providing a native Windows 11 experience with advanced data management.

![Workhammer Banner](./workhammer.webp)

## ✨ Features

### 🎨 Premium Design
- **Mica & Acrylic Effects:** Full support for Windows 11 translucency.
- **Fluent UI Controls:** Pixel-perfect WinUI 3 styling for inputs, buttons, and dialogs.
- **Editor-Style Sidebar:** A compact, resizable sidebar with modern pill-shaped highlights and professional outline icons.
- **Custom Title Bar:** Immersive, draggable title bar with a dedicated WorkHammer branding.

### 📊 Powerful Tracking
- **Smart Sorting:** Sort applications by Name, Status, Date Applied, or Last Update with instant Ascending/Descending toggling.
- **Advanced Filtering:** Live search and status-based filtering to manage long lists effortlessly.
- **Tech Stack Tagging:** Add and remove technology badges (e.g., React, Go, C#) for every job.
- **Live Statistics:** A detailed status bar showing a real-time breakdown of your application pipeline (Applied, Interviewing, Offers, etc.).

### 🛡️ Safety & Reliability
- **Unsaved Changes Protection:** Native Fluent dialogs warn you before switching views if you have unsaved progress.
- **Safe Delete:** Red-button confirmation for critical actions.
- **Disk-Based Storage:** Data is stored as clean, portable JSON files. Unsaved changes can be discarded by reloading the original state from disk.
- **Middle-Path Truncation:** Long directory paths are elegantly shortened in the sidebar for better readability.

## 🛠️ Tech Stack
- **Framework:** Avalonia UI (Cross-platform .NET)
- **Design System:** FluentAvalonia (WinUI 3 port for Avalonia)
- **Pattern:** Reactive MVVM using CommunityToolkit.Mvvm
- **Language:** C# / .NET 10 (Preview)
- **Data:** JSON Serialization

## 🚀 Getting Started

### Prerequisites
- **.NET 10 SDK** (or latest .NET SDK)
- Windows 11 (Recommended for best Mica effect)

### How to Run
1. Open the project folder in a terminal.
2. Run the following command:
   ```bash
   dotnet run
   ```
3. Use the **Select Folder** icon in the sidebar to choose where your job JSON files are stored.

### 📦 Publishing & Building

#### 1. Windows Standalone (Fastest)
Generates a single `WorkHammer.exe` for Windows x64.
- **Script:** Run `./build_windows.ps1`
- **Output:** `./publish_single/WorkHammer.exe`

#### 2. Cross-Platform Build (All OS)
Generates standalone executables for **Windows**, **Linux**, **macOS (Apple Silicon)**, and **macOS (Intel)**.
- **Script:** Run `./build_all.ps1`
- **Output:** `./publish_dist/`

#### 3. Folder Publish (Legacy)
Generates a folder containing the executable and its dependencies as separate files.
- **Script:** Run `./publish_folder.ps1`
- **Manual Command:**
  ```bash
  dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o ./publish
  ```

---
**Author:** Fezcode (A. Samil Bulbul)  
**Homepage:** [https://fezcode.com](https://fezcode.com)