using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] DoorState doorState;

    Animator animator;

    private void Awake()
    {
        if (gameObject != null)
        {
            animator = GetComponent<Animator>();

            if (doorState.isDoorDestroyed)
            {
                Destroy(gameObject);
            }
        }
        else
            return;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Open");
        }
    }

    public void DoorOff()
    {
        doorState.isDoorDestroyed = true;
        gameObject.SetActive(false);
    }
}
