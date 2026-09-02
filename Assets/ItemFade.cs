using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemFade : MonoBehaviour
{
    public Image itemImage;

    public IEnumerator ItemFadeIn(float duration)
    {
        float time = 0f;
        Color color = itemImage.color;

        while(time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, time / duration);
            itemImage.color = color;
            yield return null;
        }
    }

    public IEnumerator ItemFadeOut(float duration)
    {
        float time = 0f;
        Color color = itemImage.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, time / duration);
            itemImage.color = color;
            yield return null;
        }
    }
}
