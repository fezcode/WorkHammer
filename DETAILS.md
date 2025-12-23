# WorkHammer Project Details

Welcome! This document explains how **WorkHammer** is built. Even if you don't know anything about programming or software design, this guide will help you understand how the "engine" under the hood works.

---

## 1. What is WorkHammer?
WorkHammer is a desktop application designed to help developers track their job applications. It allows you to save where you applied, what the status is (Interviewing, Offered, etc.), track the different **Interview Stages** (HR, Technical, System Design), and record the **Final Result** (including feedback received).

## 2. The Language: C# (C-Sharp)
The app is written in **C#**. 
- Think of C# as the "instructions" given to the computer. 
- It is a powerful, modern language created by Microsoft that is used for everything from mobile apps to giant banking systems.
- It is "type-safe," meaning the computer double-checks the code to make sure we aren't trying to put a "round peg in a square hole" (like trying to treat a piece of text as a math number).

## 3. The Visuals: Avalonia UI
To make the app look like a modern Windows app (with transparency and pretty icons), we use a framework called **Avalonia UI**.
- It uses **XAML** (pronounced "Zammel") to describe the interface.
- XAML is like a blueprint for a house: it says "put a button here" and "make the background blurry here."

---

## 4. The Brains: The MVVM Pattern
We use a professional design pattern called **MVVM** (Model-View-ViewModel). To understand this, imagine a **Restaurant**:

### The Model (The Ingredients)
- **Location:** `Models/` folder.
- **Role:** These are the raw facts. A "Job Application" model just knows it has a Company Name and a Date. It doesn't know how to "look pretty" on screen; it's just data.

### The View (The Plate/Table)
- **Location:** `Views/` folder.
- **Role:** This is what you see. The buttons, the colors, and the lists. The View is "dumb"—it doesn't know how to calculate stats; it just shows what it's told to show.

### The ViewModel (The Waiter/Chef)
- **Location:** `ViewModels/` folder.
- **Role:** This is the most important part. The ViewModel takes the raw data (Models) and prepares it for the screen (View). 
- If you click "Delete," the View tells the ViewModel, and the ViewModel decides which file to delete.
- It acts as the "middleman" so the data and the visuals never have to talk to each other directly.

---

## 5. How Data is Saved
WorkHammer doesn't use a complex database. Instead, it uses **JSON files**.
- **JSON** is just a way of writing data in a text file that both humans and computers can read.
- **The Folders:**
    - Everything is kept in your user folder under a directory called `wh`.
    - `settings.json`: Remembers if you liked transparency or where your jobs are stored.
    - `jobs/`: Every job application you create gets its own little `.json` file here. This makes it very easy to backup or move your data.

## 6. Key Components
- **Services:** These are "specialists." We have a `JobDataService` that only knows how to read/write files to the hard drive, and a `SettingsService` that only cares about your preferences.
- **Converters:** These are "translators." For example, the computer sees a status like "Rejected." The **StatusToColorConverter** translates that word into the color **Red** so the View can display it.

## 7. Folder Map
- `Assets/`: Images and icons.
- `Models/`: The "Ingredients" (Data structures).
- `Services/`: The "Specialists" (Saving/Loading logic).
- `ViewModels/`: The "Logic" (Connecting data to visuals).
- `Views/`: The "Blueprints" (User Interface).

---

By separating the app this way, it makes it much easier to fix bugs. If a button looks wrong, we check the **View**. If a calculation is wrong, we check the **ViewModel**. If a file won't save, we check the **Service**.
