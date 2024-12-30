using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerPosition playerPos;

    [SerializeField] Transform transitionCheck;
    [SerializeField] float checkRadius = 0.5f;

    private bool isChecked;

    private void Awake()
    {
        Debug.Log($"현재 위치: {transform.position}");
    }

    void Update()
    {
        if (UIManager.u_instance.isTutorialRian1)
        {
            transform.position = new Vector3(28f, 5.2f, 0f);
            UIManager.u_instance.isTutorialRian1 = false;
        }
        if (UIManager.u_instance.isTutorialRian2)
        {
            transform.position = new Vector3(-1f, -0.5f, 0f);
            UIManager.u_instance.isTutorialRian2 = false;
            UIManager.u_instance.isTutorialEnd = true;
        }

        playerPos.currentPosition = transform.position;

        Checking();
    }

    void Checking()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transitionCheck.position, checkRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Transition"))
            {
                isChecked = true;
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transitionCheck.position, checkRadius);
    }
}
