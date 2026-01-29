using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<AudioClip> audioClips;
    public AudioClip bossCaraAudioClip;
    public AudioClip bossCruzAudioClip;
    public AudioClip bossCantoAudioClip;
    public AudioClip selectedAudioClip;
    public AudioSource audioSource;
    public float level;

    void Start()
    {
        int randomClip = Random.Range(0, audioClips.Count);
        selectedAudioClip = audioClips[randomClip];
        PlayMusic();
    }

    public void PlayBossMusic()
    {
        audioSource.Stop();
        if (level == 1)
        {
            selectedAudioClip = bossCaraAudioClip;
        }
        else if (level == 2)
        {
            selectedAudioClip = bossCruzAudioClip;
        }
        else
        {
            selectedAudioClip = bossCantoAudioClip;
        }
        PlayMusic();
    }

    public void PlayMusic()
    {
        audioSource.PlayOneShot(selectedAudioClip);
    }
}
