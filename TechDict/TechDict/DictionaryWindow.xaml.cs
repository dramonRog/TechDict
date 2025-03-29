using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using Microsoft.Win32;

namespace TechDict
{
    public partial class DictionaryWindow : Window
    {
        public DictionaryWindow()
        {
            InitializeComponent();
            LoadWords(); // wypisz slowa z bazy danych
        }


        //---------------------------------------CLICK ADD WORD BUTTON-----------------------------------------------
        private void AddWord_Click(object sender, RoutedEventArgs e)
        {
            AddWordWindow addWindow = new AddWordWindow();
            addWindow.Owner = this;

            if (addWindow.ShowDialog() == true)
            {
                LoadWords(); // odswiez liste
                MessageBox.Show("Word added!");
            }
        }
        //-----------------------------------------------------------------------------------------------------------

        //--------------------------------------------CLICK SAVE DICTIONARY------------------------------------------
        private void SaveDictionary_Click(object sender, RoutedEventArgs e)
        {
            List<WordItem> items = WordsListView.SelectedItems.Cast<WordItem>().ToList();

            if (items.Count == 0)
            {
                MessageBox.Show("Select at least one item to save.");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName = "SelectedWords",
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllLines(dialog.FileName,
                    items.Select(i => $"{i.Word} = {i.Translation}"));
                MessageBox.Show("Selected words saved.");
            }
        }
        //-----------------------------------------------------------------------------------------------------------

        //---------------------------------------------CLICK DELETE--------------------------------------------------
        private void DeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            if (WordsListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one item to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete the selected word(s)?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                List<WordItem> itemsToDelete = WordsListView.SelectedItems.Cast<WordItem>().ToList();

                foreach (WordItem item in itemsToDelete)
                {
                    DatabaseHelper.DeleteWord(item.Word, item.Translation);
                }

                LoadWords(); // odswiez liste
            }
        }
        //-----------------------------------------------------------------------------------------------------------

        //-----------------------------------------HELPER METHODS----------------------------------------------------
        private void LoadWords()
        {
            List<(string Word, string Translation)> words = DatabaseHelper.GetAllWords();
            List<WordItem> items = new List<WordItem>();

            foreach (var w in words)
            {
                items.Add(new WordItem { Word = w.Word, Translation = w.Translation });
            }

            WordsListView.ItemsSource = items;
        }
        //-----------------------------------------------------------------------------------------------------------


        public class WordItem
        {
            public string Word { get; set; }
            public string Translation { get; set; }
        }
    }
}
