using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Settings
{
    private float soundLevel;
    private float musicLevel;
    public Settings(float soundLevel,float musicLevel)
    {
        this.soundLevel = soundLevel;
        this.musicLevel = musicLevel;
    }
    public float getSoundLevel()
    {
        return soundLevel;
    }
    public float getmusicLevel()
    {
        return musicLevel;
    }
}
[System.Serializable]
public class HighScore
{
    private int score;
    public HighScore(int score)
    {
        this.score = score;
    }
    public int getScore()
    {
        return score;
    }
    
}