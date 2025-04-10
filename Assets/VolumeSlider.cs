using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private string prefName = "MasterVolume";
    [SerializeField] private Slider slider;
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        if (prefName == "MasterVolume")
        {
            slider.value = SoundSettings.Instance.masterVolume;
        } else if (prefName == "MusicVolume")
        {
            slider.value = SoundSettings.Instance.musicVolume;
        }
        ApplyToMixer(slider.value);
    }

    public void OnSliderValueChanged(float value)
    {
        SoundSettings.Instance.SetVolume(prefName, value);
        ApplyToMixer(value);
    }

    private void ApplyToMixer(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        audioMixer.SetFloat(prefName, dB);
    }
}
