using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMessageUI : MonoBehaviour
{
    [SerializeField] Animator animator;

    private void Start()
    {
        if (UIManager.u_instance != null && 
            UIManager.u_instance.canRequestSaveMessage())
        {
            animator.SetTrigger("Show");
        }    
    }
}
