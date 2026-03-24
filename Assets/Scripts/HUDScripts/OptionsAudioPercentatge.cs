using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsAudioPercentatge : MonoBehaviour
{
    [Header("UI References")]
    public Slider audioMusicSlider;
    public TextMeshProUGUI audioMusicPercentatgeTxt;

    public Slider audioSFXSlider;
    public TextMeshProUGUI audioSFXPercentatgeTxt;

    [Header("Audio References")]
    public AudioSource musicAudioSource;

    void Start()
    {
        if (audioMusicSlider != null)
        {
            audioMusicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
            UpdateMusicText(audioMusicSlider.value);
            UpdateMusicVolume(audioMusicSlider.value);
        }

        if (audioSFXSlider != null)
        {
            audioSFXSlider.onValueChanged.AddListener(OnSFXSliderChanged);
            UpdateSFXText(audioSFXSlider.value);
            UpdateAllSFXVolume(audioSFXSlider.value);
        }
    }

    private void OnMusicSliderChanged(float value)
    {
        UpdateMusicText(value);
        UpdateMusicVolume(value);
    }

    private void OnSFXSliderChanged(float value)
    {
        UpdateSFXText(value);
        UpdateAllSFXVolume(value);
    }

    private void UpdateMusicText(float value)
    {
        int percentage = Mathf.RoundToInt(value * 100);
        if (audioMusicPercentatgeTxt != null)
            audioMusicPercentatgeTxt.text = percentage + "%";
    }

    private void UpdateMusicVolume(float value)
    {
        if (musicAudioSource != null)
            musicAudioSource.volume = value;
    }

    private void UpdateSFXText(float value)
    {
        int percentage = Mathf.RoundToInt(value * 100);
        if (audioSFXPercentatgeTxt != null)
            audioSFXPercentatgeTxt.text = percentage + "%";
    }

    private void UpdateAllSFXVolume(float value)
    {
        // Buscar todos los AudioSources activos que no sean la música
        AudioSource[] sources = GameObject.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var audio in sources)
        {
            if (audio != null && audio != musicAudioSource)
                audio.volume = value;
        }
    }
}
