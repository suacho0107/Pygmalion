using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class LibraryLocker : NPC
{
    [SerializeField] private string lockerId;

    public GameObject defaultImage;
    public GameObject interactionImage;

    public bool unlock = false;
    bool saved;

    Collider2D col;

    protected override void Awake()
    {
        base.Awake();
        LoadData();
        saved = false;
        dialogueUI = FindObjectOfType<DialogueUI>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
        if(interactionImage != null && defaultImage != null)
        {
            if(unlock == true)
            {
                interactionImage.SetActive(true);
                defaultImage.SetActive(false);
            }
            else
            {
                defaultImage.SetActive(true);
                interactionImage.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (lockerId == "kiosk") // 키오스크: A열 306번, 2580
        {
            if (dialogueManager.CurrentNPC == this && dialogueUI.lineCount == 3 && !unlock) // kiosk CSV 파일 대사 ID 3 출력 시 A 306 열림
            {
                interactionImage.SetActive(true);
                defaultImage.SetActive(false);
                unlock = true;

                PlayerPrefs.SetInt("Kiosk", 1);
                PlayerPrefs.Save();

                SaveData();
                saved = true;
            }
            if (unlock && isInteract) // 완료 시 상호작용 불가
            {
                if (col != null) col.enabled = false;
            }
        }

        if (lockerId == "A") // 사물함 A열: 키오스크로 잠금 해제 후 최초 상호작용 시 '회의실A 열쇠' 획득
        {
            if(!unlock && PlayerPrefs.GetInt("Kiosk", 0) == 1) // kiosk unlock 시 상호작용 가능
            {
                unlock = true;
                SaveData();
                saved = true;
                dialogueFileName = "Stage2_B1F-A_dialogue";

                ColliderControl();
            }

            if (dialogueManager.CurrentNPC == this && isInteract == true) // 잠금 해제 후 최초 상호작용 시 '회의실A 열쇠' 획득
            {
                InventoryUI.instance.GetAnItem(20102);
                InteractEnd();
            }
        }
        else if (lockerId == "B") // 사물함 B열: 최초 상호작용 시 '수상한 액체가 든 병' 획득
        {
            if (dialogueManager.CurrentNPC == this && isInteract == true)
            {
                InventoryUI.instance.GetAnItem(20103);
                InteractEnd();
            }
        }
    }

    void InteractEnd() // 최초 상호작용 후 collider 비활성화
    {
        if (col != null) col.enabled = false;

        if (!saved)
        {
            SaveData();
            saved = true;
        }
    }

    void ColliderControl()
    {
        if (col == null) return;

        if (isInteract)
        {
            col.enabled = false;
            return;
        }

        col.enabled = unlock;
    }

    void SaveData()
    {
        npcData.isInteract = isInteract;
        npcData.unlock = unlock;

        string json = JsonUtility.ToJson(npcData);
        File.WriteAllText(filePath, json);
        Debug.Log(gameObject.name + " / 데이터 저장");
    }

    void LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            npcData = JsonUtility.FromJson<NPCData>(json);
            Debug.Log(gameObject.name + " / 데이터 로드");

            isInteract = npcData.isInteract;
            unlock = npcData.unlock;
        }
    }
}
