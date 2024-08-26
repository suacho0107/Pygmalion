using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerPosition playerPos;

    [SerializeField] Transform transitionCheck;
    [SerializeField] float checkRadius = 0.5f;

    private bool isChecked;

    void Update()
    {
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
