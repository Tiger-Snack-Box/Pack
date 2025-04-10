using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SoundSettings : MonoBehaviour
{
    public static SoundSettings Instance;
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetVolume(string prefName, float volume)
    {
        if (prefName == "MasterVolume") 
        {
            masterVolume = volume;
        } if (prefName == "MusicVolume")
        {
            musicVolume = volume;
        }
        
        PlayerPrefs.SetFloat(prefName, volume);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

    }
}
