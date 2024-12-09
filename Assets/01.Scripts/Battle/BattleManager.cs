using System.Collections;
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
    public GameObject blackBoard;

    public AudioSource battleAudioSource;
    public AudioClip battleStartSFX;
    public AudioClip playerAttackSFX;
    public AudioClip enemyAttackSFX;
    public AudioClip playerWinSFX;
    public AudioClip playerLoseSFX;
    public AudioClip playerRunSFX;

    public string currentPart; //Select �� partText

    public bool isWin;
    string filePath;

    public State state;

    public bool isPlayerTurnStarted = false;
    public bool isEnemyTurnStarted = false;
    bool isBattleEnd = false;
    private bool isCoroutineRunning = false;
    private bool isSFXPlaying = false;

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

        blackBoard.SetActive(false);

        //HpBar
        player.UpdatePlayerHp();
        enemy.UpdateEnemyHp();

        state = State.PLAYERTURN_START;
        PlaySFX(battleStartSFX);


    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            //PLAYERTURN
            case State.PLAYERTURN_START:
                if (!isPlayerTurnStarted)
                {
                    player.PlayerTurnStart();
                    isPlayerTurnStarted = true;
                }
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
                if (!isBattleEnd)
                {
                    PlayerWin();
                    PlaySFX(playerWinSFX);
                    isBattleEnd = true;
                    //battleAudioSource.Stop();
                    //battleAudioSource.clip = playerWinSFX;
                    //battleAudioSource.time = 0;
                    //battleAudioSource.Play();
                }
                break;

            case State.LOSE:
                if (!isBattleEnd)
                {
                    PlayerLose();
                    PlaySFX(playerLoseSFX);
                    isBattleEnd = true;
                    //battleAudioSource.Stop();
                    //battleAudioSource.clip = playerLoseSFX;
                    //battleAudioSource.time = 0;
                    //battleAudioSource.Play();
                }
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

        partText.text = "";
        hpBoxes.SetActive(false);

        StartCoroutine(ContentTextWriter("...�������� �� �� �Ѱ�?"));
        //��������

        isWin = true;
        SaveFightData();

        PlayerPrefs.SetInt("PlayerWin", 1);
        PlayerPrefs.Save();

        //SceneManager.LoadScene("Museum_ExhibitionRoom2");
        Invoke("ExitBattleScene", 1);
    }

    void PlayerLose()
    {
        Debug.Log("PlayerLose() ����");

        blackBoard.SetActive(true);
        StartCoroutine(ContentTextWriter("������ �������..."));
        //��������

        isWin = false;
        SaveFightData();

        PlayerPrefs.SetInt("PlayerLose", 1);
        PlayerPrefs.Save();

        //SceneManager.LoadScene("Museum_Lobby");
        Invoke("ExitBattleScene", 2);
    }

    public void PlaySFX(AudioClip audioClip)
    {
        Debug.Log("PlayerSFX ����");

        if (isSFXPlaying)
        {
            return;
        }

        isSFXPlaying = true;

        battleAudioSource.Stop();
        battleAudioSource.clip = audioClip;
        //audioSource.loop = false;
        battleAudioSource.time = 0;
        battleAudioSource.Play();

        Invoke("ResetPlaySFX", 3);
    }

    private void ResetPlaySFX()
    {
        isSFXPlaying = false;
    }

    public void ExitBattleScene()
    {
        if (state == State.WIN)
        {
            SceneManager.LoadScene("Museum_ExhibitionRoom2");
        }
        else if (state == State.LOSE || state == State.PLAYERTURN_RUN)
        {
            SceneManager.LoadScene("Museum_Lobby");
        }
        else
        {
            return;
        }
    }

    public IEnumerator ContentTextWriter(string origintext)
    {
        // �̹� �ڷ�ƾ�� ���� ���̶�� �ߺ� ���� ����
        if (isCoroutineRunning)
        {
            yield break;
        }

        isCoroutineRunning = true;
        contentText.text = "";

        for (int i = 0; i < origintext.Length; i++)
        {
            contentText.text += origintext[i];
            yield return new WaitForSeconds(0.03f);
        }

        //�̰� yield break�ε� �����Ѱ�?
        isCoroutineRunning = false;
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
