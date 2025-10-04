using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters;
using static UnityEditor.Progress;

public class NPC : MonoBehaviour
{
    #region Set Values
    public DialogueManager dialogueManager;
    public DialogueUI dialogueUI;
    public InteractionEvent interactionEvent; // 이 NPC와 연결된 InteractionEvent
    public MuseumLobbyCSV csv;
    public StatueScore statueScore;
    public NPCData npcData = new NPCData();
    
    public bool isInteract = false;
    [SerializeField] public bool isObject = false;

    public bool isOfficeTuto = false;
    public bool isDialogueChanged = false;

    public string filePath;

    //public int FILEINDEX;
    #endregion

    [SerializeField] public string dialogueFileName;
    [SerializeField] public string selectFileName;
    [SerializeField] public string explainNum;
    [SerializeField] public string[] dialogueFiles; // 파일 변경 배열 추가
    [SerializeField] public string[] selectFiles;
    public int currentIndex = 0;

    protected virtual void Awake()
    {
        filePath = Application.persistentDataPath + "/" + gameObject.name + "_data.json";
        //LoadNPCData(); // 각 스크립트에서 나눠서 실행, NPC에서는 직접적으로 호출X
        //ResetNPCData(); // NPC 초기화 코드
    }

    private void Start()
    {
        //ResetNPCData(); // NPC 초기화 코드
        //FieldItemManager.Instance.ResetFieldItems(); // 필드아이템 초기화 코드
    }

    public void StartDialogue()
    {
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();

        if (dialogueManager != null)
        {
            dialogueManager.SetNPC(this);
        }
        else //null 처리
        {
            Debug.LogError("DialogueManager is null.");
        }

        InteractionEvent interactionEvent = GetComponent<InteractionEvent>();
        if (interactionEvent != null)
        {
            if (!string.IsNullOrEmpty(explainNum)) //explainNum 있으면 전달
            {
                interactionEvent.LoadDialogue(dialogueFileName, explainNum);
            }
            else //explainNum 없으면 그냥
            {
                interactionEvent.LoadDialogue(dialogueFileName);
            }
        }
    }
   

    public void ChangeDialogueFileName(string _dialogueFileName)
    {
        dialogueFileName = _dialogueFileName;
        //currentName = dialogueFileName;
        //Debug.Log(dialogueFileName);
    }

    public void ChangeExplainNum(string _explainNum)
    {
        explainNum = _explainNum;
    }

    public void ChangeDialogueFile(int _currentIndex)
    {
        currentIndex = _currentIndex;
        dialogueFileName = dialogueFiles[currentIndex];
        selectFileName = selectFiles[currentIndex];
        //Debug.Log("대화: " + dialogueFileName + ", 선지: " + selectFileName);
    }
    public void SaveNPCData()
    {
        if (!isObject)
        {
            npcData.isDialogueChanged = isDialogueChanged;
            npcData.currentIndex = currentIndex;
            npcData.dialogueFileName = dialogueFileName;
            npcData.selectFileName = selectFileName;
            npcData.isInteract = isInteract;

            string json = JsonUtility.ToJson(npcData);
            File.WriteAllText(filePath, json);
            Debug.Log(gameObject.name + " / NPC 데이터 저장");
        }
    }

    public void LoadNPCData()
    {
        if(!isObject)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                npcData = JsonUtility.FromJson<NPCData>(json);
                Debug.Log(gameObject.name + " / NPC 데이터 로드");

                isDialogueChanged = npcData.isDialogueChanged;
                currentIndex = npcData.currentIndex;
                dialogueFileName = npcData.dialogueFileName;
                selectFileName = npcData.selectFileName;
                isInteract = npcData.isInteract;
            }
        }
    }

    public void ResetNPCData()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("NPC 데이터 초기화 : " + filePath);
        }
        npcData = new NPCData();
    }
}
