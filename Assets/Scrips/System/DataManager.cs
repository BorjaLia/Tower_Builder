using UnityEngine;

// Data manager is Persistent
public class DataManager : PersistentSingleton<DataManager>
{
    // PlayerPrefs keys
    private const string HIGH_SCORE_KEY = "HighScore";
    private const string MASTER_VOL_KEY = "MasterVolume";
    private const string MUSIC_VOL_KEY = "MusicVolume";
    private const string SFX_VOL_KEY = "SFXVolume";

    // publig get, private set
    public int highScore { get; private set; }
    public float masterVolume { get; private set; }
    public float musicVolume { get; private set; }
    public float sfxVolume { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        LoadData();
    }

    // Load the saved data (default if no data exists)
    private void LoadData()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);

        masterVolume = PlayerPrefs.GetFloat(MASTER_VOL_KEY, 1.0f);
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 1.0f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOL_KEY, 1.0f);

        print("====================");
        print("Data loaded!");
        print($"Highscore: {highScore}" );
        print($"Master Volume: {masterVolume}" );
        print($"Music Volume: {musicVolume}" );
        print($"SFX Volume: {sfxVolume}" );
        print("====================");
    }

    public void SaveHighScore(int score)
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
            print($"New highscore! : {highScore}");
        }
    }

    public void SaveVolumes(float master, float music, float sfx)
    {
        masterVolume = master;
        musicVolume = music;
        sfxVolume = sfx;

        PlayerPrefs.SetFloat(MASTER_VOL_KEY, masterVolume);
        PlayerPrefs.SetFloat(MUSIC_VOL_KEY, musicVolume);
        PlayerPrefs.SetFloat(SFX_VOL_KEY, sfxVolume);
        PlayerPrefs.Save();
        print("====================");
        print("New volume data:");
        print($"Master Volume: {masterVolume}");
        print($"Music Volume: {musicVolume}");
        print($"SFX Volume: {sfxVolume}");
        print("====================");
    }
}