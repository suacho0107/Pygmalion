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

        // 업무지시서의 장소 텍스트를 받아 UI의 텍스트 변경: 업무지시서 -> 확인 후 패널 닫기 시 변경
        // UIManager의 텍스트 변경 함수 호출(해당 함수의 매개변수 전달 방식에 대한 고민 필요)
        // 아냐 그 장소 지정을 sendButton에서 해야될 것 같아. 아닌가. 일단 여긴 아님. 왜냐면 location이 소속된 오브젝트에서 실행하는게 좋겠어.
        //UIManager.u_instance.UpdateUIText(locationText);
        //Debug.Log($"장소 텍스트 변경: {locationText}");
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
