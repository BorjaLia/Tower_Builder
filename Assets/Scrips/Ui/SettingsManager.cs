using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;

    private void Start()
    {
        if (DataManager.s_instance != null)
        {
            masterSlider.value = DataManager.s_instance.masterVolume;
            musicSlider.value = DataManager.s_instance.musicVolume;
            sfxSlider.value = DataManager.s_instance.sfxVolume;
            uiSlider.value = DataManager.s_instance.uiVolume;

            AudioManager.s_instance.SetVolume("MasterVolume", masterSlider.value);
            AudioManager.s_instance.SetVolume("MusicVolume", musicSlider.value);
            AudioManager.s_instance.SetVolume("SFXVolume", sfxSlider.value);
            AudioManager.s_instance.SetVolume("UIVolume", uiSlider.value);
        }

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        uiSlider.onValueChanged.AddListener(SetUIVolume);
    }

    private void SetMasterVolume(float value)
    {
        AudioManager.s_instance.SetVolume("MasterVolume", value);
        SaveAllVolumes();
    }

    private void SetMusicVolume(float value)
    {
        AudioManager.s_instance.SetVolume("MusicVolume", value);
        SaveAllVolumes();
    }

    private void SetSFXVolume(float value)
    {
        AudioManager.s_instance.SetVolume("SFXVolume", value);
        SaveAllVolumes();
    }
    private void SetUIVolume(float value)
    {
        AudioManager.s_instance.SetVolume("UIVolume", value);
        SaveAllVolumes();
    }

    private void SaveAllVolumes()
    {
        if (DataManager.s_instance != null)
        {
            DataManager.s_instance.SaveVolumes(masterSlider.value, musicSlider.value, sfxSlider.value,uiSlider.value);
        }
    }
}