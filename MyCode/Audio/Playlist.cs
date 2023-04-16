using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Playlist : MonoBehaviour
{
    public AudioClip[] clips;
    public AudioSource audiosource;
    int currentTrack;
    float v;
    // Start is called before the first frame update
    void Start()
    {
        PlayMusic();
    }
    public void PlayMusic()
    {
        if (audiosource.isPlaying)
        {
            return;
        }
        currentTrack--;
        if (currentTrack < 0)
        {
            currentTrack = clips.Length - 1;
        }
        StartCoroutine("WaitForMusicEnd");
    }
    IEnumerator WaitForMusicEnd()
    {
        while (audiosource.isPlaying)
        {
            yield return null;
        }
        NextTitle();
    }
    public void NextTitle()
    {
        audiosource.Stop();
        currentTrack++;
        if (currentTrack > clips.Length - 1)
        {
            currentTrack = 0;
        }
        audiosource.clip = clips[currentTrack];
        audiosource.Play();

        StartCoroutine("WaitForMusicEnd");
    }
}
