using System.Data.SQLite;

public class DatabaseHelper
{
    private static string dbPath = "Data Source=Data/dictionary.sqlite"; // sciezka do bazy danych

    public static List<(string Word, string Translation)> GetAllWords()
    {
        List<(string, string)> list = new List<(string, string)>();

        using (SQLiteConnection connection = new SQLiteConnection(dbPath))
        // otworz polaczenie z baza danych przy uzyciu sciezki -> uzywamy ta baze danych -> po zakonczeniu polaczenie using automatycznie zamyka polaczenia i zwalnia zasoby
        {
            connection.Open(); // otworz polaczenie z baza danych 
            string query = "SELECT Word, Translation FROM Dictionary"; // zapytanie

            using (SQLiteCommand command = new SQLiteCommand(query, connection)) // stworz polecenie sql ktore wykona zapytanie query na polaczeniu connection
            using (var reader = command.ExecuteReader()) // uruchom zapytania sql z command i zwroc wynik jako tabele
            {
                while (reader.Read())
                {
                    list.Add((reader.GetString(0)/*pierwsza kolumna(Word)*/, reader.GetString(1)/*druga kolumna(Column)*/));
                }
            }
        }

        return list;
    }

    public static void AddWord(string word, string translation)
    {
        using (SQLiteConnection connection = new SQLiteConnection(dbPath))
        {
            connection.Open();
            string query = "INSERT INTO Dictionary (Word, Translation) VALUES (@word, @translation)"; 
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@word", word); // do naszego zapytania wstaw zamiast @word - word
                command.Parameters.AddWithValue("@translation", translation);
                command.ExecuteNonQuery(); // uruchom zapytanie, nic nie oczekujemy(odpowiedzi), zwroci ilosc dodanych do tabeli wierszow
            }
        }
    }

    public static void DeleteWord(string word, string translation)
    {
        using (SQLiteConnection connection = new SQLiteConnection(dbPath))
        {
            connection.Open();
            string query = "DELETE FROM Dictionary WHERE Word = @word AND Translation = @translation";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@word", word);
                command.Parameters.AddWithValue("@translation", translation);
                command.ExecuteNonQuery();
            }
        }
    }
}