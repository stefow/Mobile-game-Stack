using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
public class SaveSystem : MonoBehaviour
{
    public static void SaveSettings(Settings settings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/settings.save";

        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, settings);
        stream.Close();
    }
    public static Settings LoadSettings()
    {
        string path = Application.persistentDataPath + "/settings.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

           Settings data = formatter.Deserialize(stream) as Settings;
           stream.Close();

            return data;
        }
        else
        {
            return new Settings(100,100);
        }
    }

    public static void SaveScore(HighScore score)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Score.save";

        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, score);
        stream.Close();
    }
    public static HighScore LoadHighScore()
    {
        string path = Application.persistentDataPath + "/Score.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            HighScore data = formatter.Deserialize(stream) as HighScore;
            stream.Close();

            return data;
        }
        else
        {
            return new HighScore(0);
        }
    }
}
