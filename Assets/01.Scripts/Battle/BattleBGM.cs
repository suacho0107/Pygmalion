using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBGM : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        Play();
    }

    void Play()
    {
        audioSource.Stop();
        audioSource.loop = true;
        audioSource.volume = 0f;
        audioSource.time = 0;
        audioSource.Play();
        FadeIn(2f);
    }

    #region Fade
    private void FadeIn(float duration)
    {
        StartCoroutine(Fade(true, duration));
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(Fade(false, duration));
    }

    private IEnumerator Fade(bool isFadeIn, float duration)
    {
        float start = isFadeIn ? 0f : 0.4f;
        float end = isFadeIn ? 0.4f : 0f;
        float time = 0f;

        audioSource.volume = start;

        while (time < duration)
        {
            float t = time / duration;
            audioSource.volume = Mathf.Lerp(start, end, t);

            time += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = end;
    }
    #endregion
}
