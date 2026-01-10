using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBGM : MonoBehaviour
{
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayBGM();
        StartCoroutine(FadeInOutBGM(true, 2f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayBGM()
    {
        audioSource.Stop();
        audioSource.loop = true;
        //audioSource.volume = 0.4f;
        audioSource.volume = 0f; //FadeIn 할 거라 0으로 Play
        audioSource.time = 0;
        audioSource.Play();
    }

    public IEnumerator FadeInOutBGM(bool _isFadeIn, float _duration)
    {
        float time = 0f;

        float startVolume = _isFadeIn ? 0f : 0.4f;
        float endVolume = _isFadeIn ? 0.4f : 0f;

        audioSource.volume = startVolume;

        while (time < _duration)
        {
            float t = time / _duration;

            float volume = Mathf.Lerp(startVolume, endVolume, t);
            audioSource.volume = volume;

            time += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = endVolume;
    }
}
