using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StatueAudio : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip pencil;
    public AudioClip enterFight;
    //public AudioClip retry;
    public AudioClip destroyed;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    //public void SoundButton()
    //{
    //    PlaySound(enterFight);
    //}

    private void PlaySound(AudioClip clip)
    {
        if(clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    void VolumeDown()
    {
        audioSource.volume = 0.5f;
    }

    public void PlayPencil()
    {
        VolumeDown();
        PlaySound(pencil);
    }

    public void PlayEnterFight()
    {
        PlaySound(enterFight);
        Debug.Log("enterFight play");
    }

    //public void PlayRetry()
    //{
    //    PlaySound(retry);
    //}

    public void PlayDestroyed()
    {
        VolumeDown();
        PlaySound(destroyed);
    }

    public void StopPlay()
    {
        audioSource.Stop();
    }
}
