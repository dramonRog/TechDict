using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TechDict
{
    public partial class FlashCardsWindow : Window
    {
        private List<(string Word, string Translation)> cards = new List<(string Word, string Translation)>();
        private int currentIndex = 0;
        private bool showTranslation = false;
        private bool isEnglishToPolish = true;

        public FlashCardsWindow()
        {
            InitializeComponent();
            PreviousButton.Visibility = Visibility.Hidden;
            NextButton.Visibility = Visibility.Hidden;
        }

        //------------------------------------------START BUTTON CLICK-----------------------------------------------
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(CountTextBox.Text, out int count) || count <= 0)
            {
                MessageBox.Show("Please enter a valid number of cards.");
                return;
            }

            isEnglishToPolish = DirectionComboBox.SelectedIndex == 0;
            List<(string Word, string Translation)> allWords = DatabaseHelper.GetAllWords();

            Random rnd = new Random();
            cards = allWords.OrderBy(x => rnd.Next()).Take(count).ToList(); // dla kazdego x przypisz mu losowa wartosc rnd.Next()
            currentIndex = 0;
            showTranslation = false;
            ShowCard();
            PreviousButton.Visibility = Visibility.Visible;
            NextButton.Visibility = Visibility.Visible;
        }
        //-----------------------------------------------------------------------------------------------------------

        //--------------------------------------------CLICK ON CARD--------------------------------------------------
        private void CardBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            showTranslation = !showTranslation;
            ShowCard();
        }
        //-----------------------------------------------------------------------------------------------------------

        //--------------------------------------------CLICK PREV BUTTON----------------------------------------------
        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                showTranslation = false;
                ShowCard();
            }
            PreviousButton.Visibility = currentIndex < 1 ? Visibility.Hidden : Visibility.Visible;
            
            if (NextButton.Visibility == Visibility.Hidden)
            {
                NextButton.Visibility = Visibility.Visible;
            }
        }
        //-----------------------------------------------------------------------------------------------------------

        //---------------------------------------------CLICK NEXT BUTTON---------------------------------------------
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex < cards.Count - 1)
            {
                currentIndex++;
                showTranslation = false;
                ShowCard();
            }
            NextButton.Visibility = currentIndex >= cards.Count - 1 ? Visibility.Hidden : Visibility.Visible;

            if (PreviousButton.Visibility == Visibility.Hidden)
            {
                PreviousButton.Visibility = Visibility.Visible;
            }
        }
        //-----------------------------------------------------------------------------------------------------------

        //---------------------------------------------------HELPER METHODS------------------------------------------
        private void ShowCard()
        {
            if (cards.Count == 0 || currentIndex < 0 || currentIndex >= cards.Count)
            {
                FlashCardText.Text = "No cards.";
                return;
            }

            var pair = cards[currentIndex];
            FlashCardText.Text = showTranslation
                ? (isEnglishToPolish ? pair.Translation : pair.Word)
                : (isEnglishToPolish ? pair.Word : pair.Translation);
        }
        //-----------------------------------------------------------------------------------------------------------

    }
}
