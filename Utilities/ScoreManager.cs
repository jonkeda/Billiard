using System;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Utilities
{
    class Record
    {
        public string Name { get; set; }
        public int Points { get; set; }
        public DateTime Date { get; set; }
    }
    class ScoreManager
    {
        public static ObservableCollection<Record> Scores { get; private set; } = new ObservableCollection<Record>();

        private static readonly string path = "scores.sav";

        static ScoreManager()
        {
            string input = "[]";
            try
            {
                input = System.IO.File.ReadAllText(path);
            }
            catch { }

            foreach (Record record in JsonSerializer.Deserialize<ObservableCollection<Record>>(input))
            {
                Scores.Add(record);
            }
        }

        public static void SaveScore(string name, int score)
        {
            Scores.Add(new Record() { Name = name, Date = DateTime.Now, Points = score });

            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(Scores));
        }
    }
}
