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
    public InteractionEvent interactionEvent; // �� NPC�� ����� InteractionEvent
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
    [SerializeField] public string[] dialogueFiles; // ���� ���� �迭 �߰�
    [SerializeField] public string[] selectFiles;
    public int currentIndex = 0;

    protected virtual void Awake()
    {
        filePath = Application.persistentDataPath + "/" + gameObject.name + "_data.json";
        ResetNPCData();
        //LoadNPCData(); // �� ��ũ��Ʈ���� ������ ����, NPC������ ���������� ȣ��X
        //ResetNPCData(); // NPC �ʱ�ȭ �ڵ�
    }

    private void Start()
    {
        //ResetNPCData(); // NPC �ʱ�ȭ �ڵ�
        FieldItemManager.Instance.ResetFieldItems(); // �ʵ������ �ʱ�ȭ �ڵ�
    }

    public void StartDialogue()
    {
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();

        if (dialogueManager != null)
        {
            dialogueManager.SetNPC(this);
        }
        else //null ó��
        {
            Debug.LogError("DialogueManager is null.");
        }

        InteractionEvent interactionEvent = GetComponent<InteractionEvent>();
        if (interactionEvent != null)
        {
            if (!string.IsNullOrEmpty(explainNum)) //explainNum ������ ����
            {
                interactionEvent.LoadDialogue(dialogueFileName, explainNum);
            }
            else //explainNum ������ �׳�
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
        //Debug.Log("��ȭ: " + dialogueFileName + ", ����: " + selectFileName);
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
            Debug.Log(gameObject.name + " / NPC ������ ����");
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
                Debug.Log(gameObject.name + " / NPC ������ �ε�");

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
            Debug.Log("NPC ������ �ʱ�ȭ : " + filePath);
        }
        npcData = new NPCData();
    }
}
