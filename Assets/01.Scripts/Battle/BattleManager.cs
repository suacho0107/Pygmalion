using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    Player player;
    Enemy enemy;
    Part part;

    public NPCData npcData = new NPCData();

    public Text contentText;
    public Text partText;
    public GameObject buttons;
    public GameObject hpBoxes;
    List<GameObject> hpBoxesList = new List<GameObject>();
    public Sprite hpBoxFull;
    public Sprite hpBoxEmpty;

    public string currentPart; //Select �� partText

    public bool isWin;
    string filePath;

    public State state;

    public bool isEnemyTurnStarted = false;

    public enum State
    {
        PLAYERTURN_START,
        PLAYERTURN_ATTACK,
        //PLAYERTURN_INVENTORY, //���� �̻��, Inventory �߰� �� ��� ����
        PLAYERTURN_RUN,
        ENEMYTURN,
        WIN,
        LOSE,
    }

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        enemy = FindObjectOfType<Enemy>();
        part = FindObjectOfType<Part>();

        filePath = Application.persistentDataPath + "/stage1_statue 3_data.json";
        LoadFightData();
    }

    // Start is called before the first frame update
    void Start()
    {
        //���� ���� �� Setting
        player.SetPlayerHp();
        enemy.StartSetEnemy();
        SetHpBoxes();

        //HpBar
        player.UpdatePlayerHp();
        enemy.UpdateEnemyHp();

        state = State.PLAYERTURN_START;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            //PLAYERTURN
            case State.PLAYERTURN_START:
                player.PlayerTurnStart();
                break;

            case State.PLAYERTURN_ATTACK:
                player.SelectAttackPart();
                break;

            case State.PLAYERTURN_RUN:
                player.Run();
                break;

            //ENEMYTURN
            case State.ENEMYTURN:
                if (!isEnemyTurnStarted)
                {
                    enemy.EnemyTurnStart();
                    isEnemyTurnStarted = true;
                }
                break;

            //END
            case State.WIN:
                PlayerWin();
                break;

            case State.LOSE:
                PlayerLose();
                break;
        }
    }

    void SetHpBoxes()
    {
        for (int i = 0; i < hpBoxes.transform.childCount; i++)
        {
            hpBoxesList.Add(hpBoxes.transform.GetChild(i).gameObject);
            Debug.Log($"hpBoxesList�� �߰�: ${hpBoxes.transform.GetChild(i)}");
            hpBoxes.transform.GetChild(i).gameObject.SetActive(false); //��Ȱ��ȭ�� �ʱ�ȭ
        }
    }

    public void UpdateHpBoxes()
    {
        Debug.Log("UpdateHpBoxes() ����");
        hpBoxes.SetActive(true);

        // �ʿ��� hpBoxes�� Ȱ��ȭ
        for (int i = 0; i < hpBoxesList.Count; i++)
        {
            if (i < enemy.partComponents[FindListIndex(enemy.parts, enemy.currentPart)].partMaxHp)
            {
                hpBoxesList[i].SetActive(true); // partMaxHp��ŭ�� Ȱ��ȭ
                hpBoxesList[i].GetComponent<Image>().sprite = (i < enemy.partComponents[FindListIndex(enemy.parts, enemy.currentPart)].partHp) ? hpBoxFull : hpBoxEmpty;
            }
            else
            {
                hpBoxesList[i].SetActive(false); // �������� ��Ȱ��ȭ
            }
        }
    }

    public int FindListIndex(List<string> list, string element)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == element)
            {
                return i;
            }
        }
        return -1;
    }

    void PlayerWin()
    {
        Debug.Log("PlayerWin() ����");
        //��������

        isWin = true;
        SaveFightData();

        PlayerPrefs.SetInt("PlayerWin", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Museum_ExhibitionRoom2");
    }

    void PlayerLose()
    {
        Debug.Log("PlayerLose() ����");
        //��������

        isWin = false;
        SaveFightData();
        SceneManager.LoadScene("Museum_Lobby");
    }

    public void SaveFightData()
    {
        npcData.isFin = isWin;

        string json = JsonUtility.ToJson(npcData);
        File.WriteAllText(filePath, json);
        Debug.Log("������ ����");
    }

    public void LoadFightData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            npcData = JsonUtility.FromJson<NPCData>(json);
            Debug.Log("������ �ε�");
        }

        isWin = npcData.isFin;
    }
}
