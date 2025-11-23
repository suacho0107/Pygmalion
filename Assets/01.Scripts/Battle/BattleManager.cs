using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

//https://docs.google.com/spreadsheets/d/1WOir0B9rY5R9YenzHMaeoO7qPDyfnq_Oio72uTV6gRk/edit?gid=1948295636#gid=1948295636

public class BattleManager : MonoBehaviour
{

    #region References
    BattleUI battleUI;
    Player player;
    Enemy enemy;
    Part part;

    public NPCData npcData = new NPCData();
    #endregion

    #region Variables

    public GameObject Aphrodite;
    public GameObject ReadingChild;
    public GameObject Melpomene;

    public AudioSource battleAudioSource;
    public AudioClip battleStartSFX;
    public AudioClip playerAttackSFX;
    public AudioClip enemyAttackSFX;
    public AudioClip playerWinSFX;
    public AudioClip playerLoseSFX;
    public AudioClip playerRunSFX;

    public string currentPart; //Select 시 partText

    public bool isWin;
    bool _enterFight;
    string filePath;


    public bool isPlayerTurnStarted = false;
    public bool isPlayerAttacking = false;
    public bool isPartSelecting = false; //PLAYERTURN_ATTACK에서 공격할 Part 선택 시
    public bool isPlayerRunning = false;
    public bool isEnemyTurnStarted = false;
    private bool isBattleEnd = false;

    private bool isCoroutineRunning = false; //Coroutine Control
    private bool isSFXPlaying = false;
    #endregion

    #region State
    public State state;

    public enum State
    {
        PLAYERTURN_START,
        PLAYERTURN_ATTACK,
        //PLAYERTURN_INVENTORY, //미사용, Inventory 추가 시 사용 예정
        PLAYERTURN_RUN,
        ENEMYTURN,
        WIN,
        LOSE,
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        battleUI = FindObjectOfType<BattleUI>();
        player = FindObjectOfType<Player>();
        part = FindObjectOfType<Part>();

        filePath = Application.persistentDataPath + "/stage1_statue 3_data.json";
        LoadFightData();

        //if문 돌려서 알맞은 적 SetActive(true);
        //Aphrodite.SetActive(true);
        //enemy = Aphrodite.GetComponent<Enemy>();
        //ReadingChild.SetActive(true);
        //enemy = ReadingChild.GetComponent<Enemy>();
        Melpomene.SetActive(true);
        enemy = Melpomene.GetComponent<Enemy>();
        Debug.Log($"Enemy set to: {enemy}"); //Delete
    }

    // Start is called before the first frame update
    void Start()
    {
        //전투 진입 시 Setting
        player.SetPlayerHp();
        enemy.SetEnemy();
        battleUI.SetDialogueButtons();
        battleUI.SetPartButtons();

        battleUI.blackBoard.SetActive(false);

        //HpBar
        player.UpdatePlayerHp();
        enemy.UpdateEnemyHp();

        state = State.PLAYERTURN_START;
        PlaySFX(battleStartSFX);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
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
                if (!isPlayerAttacking)
                {
                    player.SelectAttackPart();
                }
                break;

            case State.PLAYERTURN_RUN:
                if (!isPlayerRunning)
                {
                    PlayerRun();
                    player.Run();
                }
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
                }
                break;

            case State.LOSE:
                if (!isBattleEnd)
                {
                    PlayerLose();
                    PlaySFX(playerLoseSFX);
                }
                break;
        }
    }
    #endregion

    public void Damage(string _object, int _attackpower, Part part = null)
    {
        if(player.isConfused)
        {
            if(_object == "enemy")
            {
                _object = "player";
            }
        }

        if (_object == "player")
        {
            //if (part == null) //처음부터 대상이 player
            //{
            player.playerHp -= _attackpower;
            player.UpdatePlayerHp();
            //}
            //else //isConfused로 대상이 player로 변경된 경우
            //{
            //    player.playerHp -= _attackpower;
            //    player.UpdatePlayerHp();
            //}
        }
        else if (_object == "enemy" && part != null)
        {
            float attackpower = (float)_attackpower * enemy.increaseAttackPower;

            if (player.isConfused)
            {
                attackpower *= 10;
            }

            _attackpower = (int)attackpower;
            part.partHp -= _attackpower;
            enemy.UpdateEnemyHp();
        }
    }

    void PlayerWin()
    {
        Debug.Log("PlayerWin() 실행");
        //구현예정

        isWin = true;
        _enterFight = true;
        SaveFightData();

        PlayerPrefs.SetInt("PlayerWin", 1);
        PlayerPrefs.Save();

        Invoke("ExitBattleScene", 1);
    }

    void PlayerLose()
    {
        Debug.Log("PlayerLose() 실행");

        battleUI.blackBoard.SetActive(true);
        StartCoroutine(ContentTextWriter("눈앞이 흐려진다..."));
        //구현예정

        isWin = false;
        _enterFight = false;
        SaveFightData();

        PlayerPrefs.SetInt("PlayerLose", 1);
        PlayerPrefs.Save();

        Invoke("ExitBattleScene", 2);
    }

    void PlayerRun() // 추가 코드
    {
        isWin = false;
        _enterFight = false;
        SaveFightData();
        
        PlayerPrefs.SetInt("PlayerRun", 1);
        PlayerPrefs.Save();
    }

    #region SFX
    public void PlaySFX(AudioClip audioClip)
    {
        Debug.Log("PlayerSFX 실행");

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
    #endregion

    public void ExitBattleScene()
    {
        if (state == State.WIN)
        {
            SceneManager.LoadScene("Museum_ExhibitionRoom2");
        }
        else if (state == State.LOSE || state == State.PLAYERTURN_RUN)
        {
            SceneManager.LoadScene("Monologue_defeat");
        }
        else
        {
            return;
        }
    }

    public IEnumerator ContentTextWriter(string origintext)
    {
        // 이미 코루틴이 실행 중이라면 중복 실행 방지
        if (isCoroutineRunning)
        {
            yield break;
        }

        isCoroutineRunning = true;
        battleUI.contentText.text = "";

        for (int i = 0; i < origintext.Length; i++)
        {
            battleUI.contentText.text += origintext[i];
            yield return new WaitForSeconds(0.03f);
        }

        //이거 yield break로도 가능한가?
        isCoroutineRunning = false;
    }

    #region Save/Load Data
    public void SaveFightData()
    {
        npcData.isFin = isWin;
        npcData.enterFight = _enterFight;

        string json = JsonUtility.ToJson(npcData);
        File.WriteAllText(filePath, json);
        Debug.Log("데이터 저장");
    }

    public void LoadFightData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            npcData = JsonUtility.FromJson<NPCData>(json);
            Debug.Log("데이터 로드");
        }

        isWin = npcData.isFin;
        _enterFight = npcData.enterFight;
    }
    #endregion
}
