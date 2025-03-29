using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TechDict
{
    public partial class MainWindow : Window
    {
        private bool isEnglishToPolish = true; // Language direction
        private bool suppressSuggestions = false; // czy bylo klikniete slowo z listy mozliwych slow?
        // HISTORY BOX:
        private List<(string Word, string Translation)> historyEntries = new List<(string Word, string Translation)>(); // lista która składa się z elementów zawierających dwa pola(krotka)

        public MainWindow()
        {
            InitializeComponent();
        }


        #region MainWIndowContent
        //-------------------------------------TRANSLATE CLICK------------------------------------------------------
        private void Translate_Click(object sender, RoutedEventArgs e)
        {
            string input = InputTextBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(input))
            {
                OutputTextBox.Text = "Please enter a word.";
                return;
            }

            var words = DatabaseHelper.GetAllWords();
            (string Word, string Translation)? match = null; // krotka
            // Krotka ma Value property when the krotka is nullable, inside Value we have Word and Translation
            if (isEnglishToPolish)
            {
                match = words.FirstOrDefault(w => w.Word.ToLower() == input);
            }
            else
            {
                match = words.FirstOrDefault(w => w.Translation.ToLower() == input);
            }
            
            if (match != null && !string.IsNullOrEmpty(match.Value.Word) && !string.IsNullOrEmpty(match.Value.Translation))
            {
                string output = isEnglishToPolish ? match.Value.Translation : match.Value.Word;
                OutputTextBox.Text = output;

                // Store in history if not duplicate
                if (!historyEntries.Any(h =>
                    h.Word == match.Value.Word &&
                    h.Translation == match.Value.Translation))
                { 
                    historyEntries.Insert(0, match.Value);
                    RedrawHistory();
                }
            }
            else
            {
                OutputTextBox.Text = "Translation not found.";
            }
        }
        //-----------------------------------------------------------------------------------------------------------

        //---------------------------------------------------CLEAR INPUT CLICK---------------------------------------
        private void ClearInput_Click(object sender, RoutedEventArgs e)
        {
            InputTextBox.Clear();
            OutputTextBox.Clear();
        }
        //-----------------------------------------------------------------------------------------------------------

        //--------------------------------------------------SWAP LANGUAGE CLICK--------------------------------------
        private void SwapLanguages_Click(object sender, RoutedEventArgs e)
        {
            isEnglishToPolish = !isEnglishToPolish;

            InputLabel.Content = isEnglishToPolish ? "English" : "Polish";
            OutputLabel.Content = isEnglishToPolish ? "Polish" : "English";

            string temp = InputTextBox.Text;
            InputTextBox.Text = OutputTextBox.Text;
            OutputTextBox.Text = temp;

            RedrawHistory();
        }
        //-----------------------------------------------------------------------------------------------------------

        //----------------------------------THE ITEM FROM HISTORY BOX WAS SELECTED(CLICKED ON)-----------------------
        private void HistoryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HistoryListBox.SelectedItem is string selected) // jezeli selecteditem jest stringem, to otworz zmienna selected z wartoscia wybranego elementu
            {
                string[] pair = selected.Split('=');
                InputTextBox.Text = pair[0].Trim();
                OutputTextBox.Text = pair[1].Trim();
                suppressSuggestions = true;
            }
        }
        //-----------------------------------------------------------------------------------------------------------

        //------------------------------------------------WRITE TEXT BOX CONTENT WAS CHANGED-------------------------
        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (suppressSuggestions)
            {
                suppressSuggestions = false;
                return;
            }

            string input = InputTextBox.Text.Trim();

            PlaceholderText.Visibility = string.IsNullOrWhiteSpace(input)
                ? Visibility.Visible
                : Visibility.Collapsed; // Nie jest widoczny oraz nie rezerwuje miejsca w input bloku

            if (string.IsNullOrWhiteSpace(input))
            {
                SuggestionsListBox.Items.Clear(); // Wyczysc liste z slowami ktore pasuje do slowa ktore uzytkownik wpisuje
                SuggestionsPopup.IsOpen = false; // Zamknij male wyskakujace okienko ktore zawiera SuggestionsListBox
                return;
            }

            List<(string Word, string Translation)> words = DatabaseHelper.GetAllWords();

            List<(string Word, string Translation)> matches = isEnglishToPolish
                ? words.Where(w => w.Word.StartsWith(input)).ToList()
                : words.Where(w => w.Translation.StartsWith(input)).ToList();

            SuggestionsListBox.Items.Clear();

            if (matches.Any())
            {
                foreach (var match in matches)
                {
                    SuggestionsListBox.Items.Add(isEnglishToPolish ? match.Word : match.Translation);
                }

                if (!SuggestionsPopup.IsOpen)
                    SuggestionsPopup.IsOpen = true;
            }
            else
            {
                SuggestionsPopup.IsOpen = false;
            }
        }
        //-----------------------------------------------------------------------------------------------------------

        //-------------------------------------------------Add Click-------------------------------------------------
        private void AddWord_Click(object sender, RoutedEventArgs e)
        {
            AddWordWindow addWindow = new AddWordWindow();
            addWindow.Owner = this; // glowne okno

            if (addWindow.ShowDialog() == true) // jezeli uzytkownik zatwierdzil dodawanie slowa, 
            {
                MessageBox.Show("Word added!");
            }
        }
        //-----------------------------------------------------------------------------------------------------------

        //--------------------------------CLICK ON THE WORD FROM THE TOGGLE LIST------------------------------------
        private void SuggestionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SuggestionsListBox.SelectedItem != null)
            {
                string selectedWord = SuggestionsListBox.SelectedItem.ToString();

                suppressSuggestions = true;                // slowo bylo klikniete
                SuggestionsPopup.IsOpen = false;           

                InputTextBox.Text = selectedWord;
                InputTextBox.Focus();
                InputTextBox.CaretIndex = selectedWord.Length; // wyznacz pozycje kursora

                Translate_Click(null, null);

                SuggestionsListBox.SelectedItem = null; // wyczysc to zaznacenie ktora wybral uzytkownik
            }
        }
        //-----------------------------------------------------------------------------------------------------------

        //-------------------------------------------------HELPER METHODS--------------------------------------------
        private void RedrawHistory()
        {
            HistoryListBox.Items.Clear(); // history box jest pusty, ale mamy liste ktora zachowuje elementy tego bloku

            foreach (var entry in historyEntries)
            {
                string display = isEnglishToPolish
                    ? $"{entry.Word} = {entry.Translation}"
                    : $"{entry.Translation} = {entry.Word}";

                HistoryListBox.Items.Add(display);
            }
        }
        //-----------------------------------------------------------------------------------------------------------
        #endregion

        #region FileMenuItem
        //------------------------------------------------EXPORT HISTORY CLICK---------------------------------------
        private void ExportHistory_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "history_export",
                DefaultExt = ".txt",
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Export History"
            };

            if (dialog.ShowDialog() == true)
            {
                List<string> lines = HistoryListBox.Items
                    .Cast<string>() // Metoda LINQ: ktora sie zmiennia wszystkie elementy kolekcji na string
                    .ToList();

                System.IO.File.WriteAllLines(dialog.FileName, lines); // Otworz file with FileName -> Zapisz wszystkie wierszy tekstu lines

                MessageBox.Show("History exported successfully.");
            }
        }
        //-----------------------------------------------------------------------------------------------------------

        //------------------------------------------------IMPORT HISTORY CLICK---------------------------------------
        private void ImportHistory_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt",
                Title = "Import History from File"
            };

            if (dialog.ShowDialog() == true)
            {
                string[] lines = System.IO.File.ReadAllLines(dialog.FileName);

                historyEntries.Clear(); 
                HistoryListBox.Items.Clear();

                foreach (string line in lines)
                {
                    string[] pair = line.Split('=');
                    if (pair.Length == 2)
                    {
                        string word = pair[0].Trim();
                        string translation = pair[1].Trim();

                        historyEntries.Insert(0, (word, translation));
                    }
                }

                RedrawHistory();
                MessageBox.Show("History imported successfully.");
            }
        }
        //-----------------------------------------------------------------------------------------------------------

        //-------------------------------------------------CLEAR HISTORY CLICK---------------------------------------
        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            historyEntries.Clear();
            HistoryListBox.Items.Clear();
        }
        //-----------------------------------------------------------------------------------------------------------

        //-------------------------------------------------ABOUT CLICK-----------------------------------------------
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TechDict\nVersion 1.0\nCreated by Roman Buchynskyi!");
        }
        //-----------------------------------------------------------------------------------------------------------

        //-------------------------------------------------EXIT CLICK------------------------------------------------
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //-----------------------------------------------------------------------------------------------------------
        #endregion

        #region DictionaryMenuItem
        //--------------------------------------------------OPEN CLICK-----------------------------------------------
        private void OpenDictionary_Click(object sender, RoutedEventArgs e)
        {
            DictionaryWindow dictWindow = new DictionaryWindow();
            dictWindow.Owner = this;
            dictWindow.ShowDialog();
        }
        //-----------------------------------------------------------------------------------------------------------

        //-------------------------------------------------SAVE TO TXT CLICK-----------------------------------------
        private void SaveDictionary_Click(object sender, RoutedEventArgs e)
        {
            List<(string Word, string Translation)> words = DatabaseHelper.GetAllWords();

            if (words.Count == 0)
            {
                MessageBox.Show("No words to save.");
                return;
            }

            SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Dictionary",
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllLines(dialog.FileName,
                    words.Select(w => $"{w.Word} = {w.Translation}"));
                MessageBox.Show("Dictionary saved successfully.");
            }
        }
        //-----------------------------------------------------------------------------------------------------------
        #endregion

        #region FlashCardsMenuItem
        //-------------------------------------------FLASHCARDS CLICK------------------------------------------------
        private void OpenFlashCards_Click(object sender, RoutedEventArgs e)
        {
            FlashCardsWindow flashWindow = new FlashCardsWindow();
            flashWindow.Owner = this;
            flashWindow.ShowDialog();
        }
        //-----------------------------------------------------------------------------------------------------------
        #endregion
    }
}


