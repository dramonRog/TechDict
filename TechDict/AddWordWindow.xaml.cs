using System.Windows;

namespace TechDict
{
    public partial class AddWordWindow : Window
    {
        public AddWordWindow()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string word = WordBox.Text.Trim();
            string translation = TranslationBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(word) || string.IsNullOrWhiteSpace(translation))
            {
                MessageBox.Show("Both fields are required.");
                return;
            }

            DatabaseHelper.AddWord(word, translation);
            DialogResult = true; // pole okienka, ktore okresla jak bylo zamkniete
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
