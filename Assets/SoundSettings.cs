using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private AudioMixer masterMixer;

    public static SoundSettings instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //broken code, finds slider but value doesn't change at all
    //private void OnEnable()
    //{
    //    SceneManager.sceneLoaded += refindSlider;
    //}
    //private void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= refindSlider;
    //}
    private void Start()
    {
        if (PlayerPrefs.HasKey("savedMusicVolume"))
        {
            loadMusicVolume();
        }
        else
            setMusicFromSlider();
        if (PlayerPrefs.HasKey("savedSoundVolume"))
        {
            loadSoundVolume();
        }
        else
            setVolumeFromSlider();
    }
    private void Update()
    {
        refindSlider();
    }
    public void setVolumeFromSlider()
    {
        float volume = soundSlider.value;
        masterMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("savedSoundVolume", volume);
    }
    private void loadSoundVolume()
    {
        soundSlider.value = PlayerPrefs.GetFloat("savedSoundVolume");
        setVolumeFromSlider();
    }
    public void setMusicFromSlider()
    {
        float volume = musicSlider.value;
        masterMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("savedMusicVolume", volume);
    }
    private void loadMusicVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("savedMusicVolume");
        setMusicFromSlider();
    }
    private void refindSlider()
    {
            musicSlider = GameObject.Find("Music Slider")?.GetComponent<Slider>();
            soundSlider = GameObject.Find("Audio Slider")?.GetComponent<Slider>();
    }
}
