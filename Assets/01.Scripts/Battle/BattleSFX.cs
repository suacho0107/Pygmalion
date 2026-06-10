using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSFX : MonoBehaviour
{
    #region Variables
    [Header("Audios & SFXs")]
    [SerializeField] private AudioSource audioSource;

    public AudioClip playerAttack;
    public AudioClip enemyAttack;

    public AudioClip win;
    public AudioClip lose;
    public AudioClip run;

    private bool isPlaying = false;
    #endregion

    public void Play(AudioClip clip)
    {
        //Debug.Log($"Play({clip})");
        if (isPlaying)
        {
            ResetPlay();
        }

        isPlaying = true;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.time = 0;
        audioSource.Play();

        Invoke(nameof(ResetPlay), 2f);
    }

    private void ResetPlay()
    {
        isPlaying = false;
    }
}
