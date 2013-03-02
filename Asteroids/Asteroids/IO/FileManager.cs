using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Asteroids.IO
{
    class FileManager
    {
        public string scorePath = "";
        public static List<HighScore> highscores = new List<HighScore>();

        XmlSerializer serializer;

        public FileManager()
        {
            string myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string myGames = Path.Combine(myDocs, "My Games\\Asteroids");
            scorePath = Path.Combine(myGames, "scores.xml");

            if (!Directory.Exists(myGames))
                Directory.CreateDirectory(myGames);

            // Create the stream if it doesn't exist . . . Just a test
            FileStream stream = File.Open(scorePath, FileMode.OpenOrCreate);
            stream.Close();

            serializer = new XmlSerializer(typeof(List<HighScore>));

            if (new FileInfo(scorePath).Length == 0)
                ResetScores();

            LoadData();
        }

        private void LoadData()
        {
            Stream dataStream = File.Open(scorePath, FileMode.Open);
            highscores = serializer.Deserialize(dataStream) as List<HighScore>;
            dataStream.Close();
        }

        public void SaveScore(string name, int score)
        {
            if (highscores.Count < 10)
                highscores.Add(new HighScore() { Name = name, Score = score });
            else
            {
                var orderedScores = (List<HighScore>)highscores.OrderByDescending(s => s.Score).ToList();
                orderedScores.Reverse();

                if (score > orderedScores[0].Score)
                    orderedScores[0] = new HighScore() { Name = name, Score = score };
                highscores = orderedScores;
            }

            highscores = (List<HighScore>) highscores.OrderByDescending(s => s.Score).ToList();

            Stream dataStream = File.Open(scorePath, FileMode.OpenOrCreate);
            try
            {
                serializer.Serialize(dataStream, highscores);
            }
            finally
            {
                dataStream.Close();
            }
        }

        public List<HighScore> GetHighScores(int amount = 1)
        {
            List<HighScore> result = new List<HighScore>(amount);
            int index = 0;

            do
            {
                result.Add(highscores[index]);
                index++;
            } while (result.Count < amount);

            return (List<HighScore>)highscores.OrderByDescending(s => s.Score).ToList();
        }

        /// <summary>
        /// Resets the in-game scoreboard to default. **WARNING** This will
        /// erase all your personal scores.
        /// </summary>
        public void ResetScores()
        {
            // Clear the current high scores list
            highscores.Clear();

            List<HighScore> defaultScores = new List<HighScore>();

            // Add 6 entries for default high scores
            for (int i = 1; i < 6; i++)
                defaultScores.Add(new HighScore() { Name = "Default " + i.ToString(), Score = i * 1000 });

            // Save each high score to the xml
            foreach (HighScore score in defaultScores)
                SaveScore(score.Name, score.Score);
        }

    }
}
   