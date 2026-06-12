using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarUI : MonoBehaviour
{
    [SerializeField] private Image hpImage;

    private Material runtimeMaterial;

    private static readonly int FillAmountID = Shader.PropertyToID("_FillAmount");

    private void Awake()
    {
        runtimeMaterial = Instantiate(hpImage.material);
        hpImage.material = runtimeMaterial;

        Debug.Log(runtimeMaterial.name);
    }

    public float FillAmount
    {
        get => runtimeMaterial.GetFloat(FillAmountID);
        set => runtimeMaterial.SetFloat(FillAmountID, value);
    }
}
