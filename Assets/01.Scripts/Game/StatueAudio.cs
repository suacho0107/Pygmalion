using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class StatueAudio : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup SFXGroup;

    public AudioClip pencil;
    public AudioClip enterFight;
    //public AudioClip retry;
    public AudioClip destroyed;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.outputAudioMixerGroup = SFXGroup;
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
        //Debug.Log("enterFight play");
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
