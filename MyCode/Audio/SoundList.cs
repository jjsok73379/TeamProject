using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundList : MonoBehaviour
{
    public static SoundList Inst = null;
    public AudioClip[] SoundClips;

    private void Awake()
    {
        Inst = this;
    }
    public void PlaySound(int ClipNum)
    {
        SoundManager.Instance.SoundList.clip = SoundClips[ClipNum];
        switch (ClipNum)
        {
            case 0: // Ç²½ºÅÇ
                SoundManager.Instance.SoundList.volume = 0.2f;
                SoundManager.Instance.SoundList.pitch = 0.55f;
                break;
            case 1:
                SoundManager.Instance.SoundList.loop = false;
                SoundManager.Instance.SoundList.pitch = 1f;
                break;
        }
        SoundManager.Instance.SoundList.Play();
    }
}
