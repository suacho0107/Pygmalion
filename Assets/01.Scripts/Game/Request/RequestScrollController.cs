using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestScrollController : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;

    private float previousHeight;

    void Update()
    {
        // Content 높이가 변했으면
        if (content.rect.height != previousHeight)
        {
            previousHeight = content.rect.height;

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
