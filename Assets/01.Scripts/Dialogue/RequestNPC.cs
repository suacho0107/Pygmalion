using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestNPC : NPC
{
    [Header("업무 의뢰서")]
    [SerializeField] private PlayerDesk playerDesk;
    [SerializeField] private GameObject replyChat;
    [SerializeField] private GameObject targetUI;
    [SerializeField] private Button replyButton;
    [SerializeField] private Text interactText;
    [SerializeField] private Text profileText;

    [SerializeField] private Text locationText;

    public CompanyOfficeCSV csv2;

    private bool isStartRequest = true;
    private bool isAcceptRequest = false;
    private bool isRoutineStarted = false;
    private bool canSend = false;

    private void Start()
    {
        replyChat.SetActive(false);
    }

    void LateUpdate()
    {
        if (csv2 != null)
        {
            // 의뢰 접수
            if (isStartRequest)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartDialogue();
                    isStartRequest = false;
                }
            }

            // 설명문 대화 출력 후
            if (isInteract)
            {
                // 텍스트 필드의 값을 "네 알겠습니다."로 변경
                if (explainNum == "1")
                {
                    interactText.text = "네 알겠습니다.";
                    profileText.text = "네 알겠습니다.";

                    interactText.color = new Color(0f, 0f, 0f);
                }
                canSend = true;

                // CSV 파일의 대화 파일 변경
                // csv2.npcs[0].ChangeDialogueFile();
            }

            // 의뢰 수락
            if (isAcceptRequest)
            {
                interactText.text = "메세지 입력";
                interactText.color = new Color(103f / 255f, 102f / 255f, 102f / 255f);

                // 다이얼로그 출력 후 패널 닫기
                explainNum = "2";
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartDialogue();
                    isAcceptRequest = false;
                }

                // 일과 시작 - UI 출력
                // - 고민..
                isRoutineStarted = true;
            }
        }
    }

    public void SendButton()
    {
        if (canSend)
        {
            // 스크롤뷰에 답장 오브젝트 추가
            replyChat.SetActive(true);
            canSend = false;
            isAcceptRequest = true;
            replyButton.interactable = false;

            // UI 상태 전환 (Ready -> Start)
            UIManager.u_instance.SetUIState(Define.UI.UIState.Start);

            UIManager.u_instance.UpdateUIText(locationText);
            Debug.Log("RequestNPC는 정상");
        }
    }

}