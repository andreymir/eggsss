using System;
using System.IO;
using System.Xml.Serialization;

namespace eggsss
{
    [Serializable]
    public class HighScoreData
    {
        public int Score = 0;
        private const string HIGH_SCORES_FILENAME = "highscores.lst";

        private static void SaveHighScores(HighScoreData data)
        {
            FileStream stream = File.Open(HIGH_SCORES_FILENAME, FileMode.OpenOrCreate);
            try
            {
                var serializer = new XmlSerializer(typeof (HighScoreData));
                serializer.Serialize(stream, data);
            }
            finally
            {
                stream.Close();
            }
        }

        private static HighScoreData LoadHighScores()
        {
            HighScoreData data;
            FileStream stream = File.Open(HIGH_SCORES_FILENAME, FileMode.OpenOrCreate,
                                          FileAccess.Read);
            try
            {
                var serializer = new XmlSerializer(typeof (HighScoreData));
                data = (HighScoreData) serializer.Deserialize(stream);
            }
            finally
            {
                stream.Close();
            }

            return (data);
        }

        public void Initialize()
        {
            if (!File.Exists(HIGH_SCORES_FILENAME))
            {
                var data = new HighScoreData {Score = 50};
                SaveHighScores(data);
            }
            else
            {
                HighScoreData data = LoadHighScores();
                Score = data.Score;
            }
        }


        public void SaveHighScore()
        {
            HighScoreData data = LoadHighScores();

            if (Score > data.Score)
            {
                data.Score = Score;
                SaveHighScores(data);
            }
        }
    }
}

