using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDesk : MonoBehaviour
{
    [SerializeField] GameObject requestPanel;

    private bool playerInRange = false;
    private bool IsActive = false;

    private void Update()
    {
        if (playerInRange && !IsActive && Input.GetKeyDown(KeyCode.F))
        {
            requestPanel.SetActive(true);
            IsActive = true;
        }
        else if (IsActive && Input.GetKeyDown(KeyCode.F))
        {
            requestPanel.SetActive(false);
            IsActive = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
