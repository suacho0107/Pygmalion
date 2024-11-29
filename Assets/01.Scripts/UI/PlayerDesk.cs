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
        if (playerInRange && !IsActive && Input.GetKeyDown(KeyCode.F))
        {
            requestPanel.SetActive(true);
            IsActive = true;
        }
        else if (startON && IsActive && Input.GetKeyDown(KeyCode.F))
        {
            Close();
        }
    }

    public void Close()
    {
        requestPanel.SetActive(false);
        IsActive = false;

        UIManager.u_instance.SetUIState(Define.UI.UIState.Start);

        // �������ü��� ��� �ؽ�Ʈ�� �޾� UI�� �ؽ�Ʈ ����: �������ü� -> Ȯ�� �� �г� �ݱ� �� ����
        // UIManager�� �ؽ�Ʈ ���� �Լ� ȣ��(�ش� �Լ��� �Ű����� ���� ��Ŀ� ���� ��� �ʿ�)
        // �Ƴ� �� ��� ������ sendButton���� �ؾߵ� �� ����. �ƴѰ�. �ϴ� ���� �ƴ�. �ֳĸ� location�� �Ҽӵ� ������Ʈ���� �����ϴ°� ���ھ�.
        //UIManager.u_instance.UpdateUIText(locationText);
        //Debug.Log($"��� �ؽ�Ʈ ����: {locationText}");
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
