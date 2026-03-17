# TechDict

**TechDict** is a desktop application (WPF) designed for quick translation and interactive vocabulary learning. The application is optimized for translating technical terminology and everyday words between English and Polish, supports local database operations, and features a built-in Flashcards module for knowledge testing.

## ⚙️ Key Features

* **Two-Way Translation:** The program supports translation in both directions: English to Polish and Polish to English.
* **Smart Suggestions (Autocomplete):** As the user types, a drop-down list appears displaying word suggestions that match the entered characters.
* **Search History:** The application automatically saves the search history. Users can export the history to a `.txt` file, import it back, or clear the list entirely.
* **Dictionary Management:** Through a dedicated "Dictionary" window, users can view all existing records, add new words, and delete existing translation pairs. Exporting selected words to a text file is also supported.
* **Flashcards Module:** An interactive learning mode. Users can select the number of random cards and the translation direction. Clicking on a card flips it to reveal the correct translation. Navigation between cards is done using the "Previous" and "Next" buttons.
* **Local Storage:** All vocabulary is stored locally in an SQLite database (`dictionary.sqlite`), ensuring fast performance in offline mode.

## 🛠 Tech Stack

* **Programming Language:** C#
* **Framework:** .NET 8.0, Windows Presentation Foundation (WPF)
* **Database:** SQLite (package `System.Data.SQLite` version 1.0.119)

## 📂 Project Structure

* `MainWindow.xaml.cs` — Main window logic: text input, search, history management, menus, and language switching.
* `DictionaryWindow.xaml.cs` — Dictionary editing window (view, delete, export records).
* `AddWordWindow.xaml.cs` — Form for adding a new word and its translation.
* `FlashCards.xaml.cs` — Logic for the "Flashcards" training module.
* `DataBase.cs` (`DatabaseHelper`) — SQLite connection setup (executes SQL queries: `SELECT`, `INSERT`, `DELETE`).

## 🚀 Installation & Usage

1. Clone the repository to your local machine.
2. Open the solution file (`.sln` or `TechDict.csproj`) using **Visual Studio 2022** (ensure the .NET desktop development workload for WPF is installed).
3. Restore NuGet packages (the IDE will download the `System.Data.SQLite` dependency).
4. Build and run the project. The database file `dictionary.sqlite` will be automatically copied to the output directory thanks to the `CopyToOutputDirectory` setting.

## 👤 Author

Created by: **Roman Buchynskyi**.
