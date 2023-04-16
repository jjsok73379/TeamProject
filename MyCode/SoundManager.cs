using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource ClickSound;
    public AudioSource MoneySound;
    public AudioSource BGMSound;
    public AudioSource StartSound;
    public AudioSource NoticeSound;
    public AudioSource NoTouchSound;
    public AudioSource TypingSound;
    public AudioSource SoundList;
    public AudioSource OneDayEndSound;
    public AudioSource[] Eff;


    private void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Tutorial1")
        {
            SoundList.volume = PlayerPrefs.GetFloat("volume");
        }
        else
        {
            BGMSound.volume = PlayerPrefs.GetFloat("volume");
        }
        foreach (AudioSource source in Eff)
        {
            source.volume = PlayerPrefs.GetFloat("eff");
        }
    }
}
