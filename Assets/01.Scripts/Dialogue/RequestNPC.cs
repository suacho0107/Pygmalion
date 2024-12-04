using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestNPC : NPC
{
    [Header("���� �Ƿڼ�")]
    [SerializeField] private PlayerDesk playerDesk;
    [SerializeField] private GameObject replyChat;
    [SerializeField] private GameObject targetUI;
    [SerializeField] private Button replyButton;
    [SerializeField] private Text interactText;
    [SerializeField] private Text profileText;

    [SerializeField] private Text locationText;

    public CompanyOfficeCSV csv2;

    private bool isStartRequest = true;
    public  bool isAcceptRequest = false;
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
            // �Ƿ� ����
            if (isStartRequest)
            {
                isInteract = false;
                replyButton.interactable = false;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartDialogue();
                    isStartRequest = false;
                }
            }

            // ���� ��ȭ ��� ��
            if (isInteract)
            {
                // �ؽ�Ʈ �ʵ��� ���� "�� �˰ڽ��ϴ�."�� ����
                if (explainNum == "1")
                {
                    replyButton.interactable = true;

                    interactText.text = "�� �˰ڽ��ϴ�.";
                    profileText.text = "�� �˰ڽ��ϴ�.";

                    interactText.color = new Color(0f, 0f, 0f);
                }
                canSend = true;
            }

            // �Ƿ� ����
            if (isAcceptRequest)
            {
                interactText.text = "�޼��� �Է�";
                interactText.color = new Color(103f / 255f, 102f / 255f, 102f / 255f);
                replyButton.interactable = false;

                // ���̾�α� ��� �� �г� �ݱ�
                explainNum = "2";
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartDialogue();
                    isAcceptRequest = false;
                }

                // �ϰ� ���� - UI ���
                // - ���..
                isRoutineStarted = true;
            }
        }
    }

    public void SendButton()
    {
        if (canSend)
        {
            replyChat.SetActive(true);
            canSend = false;
            isAcceptRequest = true;
            replyButton.interactable = false;

            playerDesk.startON = true;

            UIManager.u_instance.UpdateStartUI();
            Debug.Log($"RequestNPC: {locationText}");
            // UIManager.u_instance.UpdateUIText(locationText);
            // Debug.Log($"��� �ؽ�Ʈ ����: {locationText}");
        }
    }

}