using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Define;

public class PlayerDesk : MonoBehaviour
{
    [SerializeField] GameObject requestPanel;
    // [SerializeField] private Text locationText;
    RequestNPC requestNPC;

    private bool playerInRange = false;
    private bool IsActive = false;
    public bool startON = false;

    private void Awake()
    {
        requestNPC = requestPanel.GetComponent<RequestNPC>();
    }

    private void Update()
    {
        //if (playerInRange && !IsActive && Input.GetKeyDown(KeyCode.F))
        //{
        //    requestPanel.SetActive(true);
        //    IsActive = true;
        //}
        //else if (startON && IsActive && Input.GetKeyDown(KeyCode.F))
        //{
        //    Close();
        //}
    }

    public void Close()
    {
        requestPanel.SetActive(false);
        IsActive = false;

        UIManager.u_instance.Set_UIState(Define.UI.UIState.Start);
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
