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
    UIManager uiManager;

    public NPCData npcData = new NPCData();
    #endregion

    #region Variables
    [Header("Enemys")]
    public GameObject Aphrodite;
    public GameObject ReadingChild;
    public GameObject Melpomene;

    [Header("Audios & SFXs")]
    public AudioSource battleAudioSource;
    public AudioClip battleStartSFX;
    public AudioClip playerAttackSFX;
    public AudioClip enemyAttackSFX;
    public AudioClip playerWinSFX;
    public AudioClip playerLoseSFX;
    public AudioClip playerRunSFX;

    public string currentPart; //Select 시 partText

    public bool isWin;
    private bool _enterFight;
    private string filePath;

    [Header("Controls")]
    private bool isBattleMode = false;

    public bool isStatePLAYERTURN = false; //State PLAYERTURN동안 true
    public bool isStatePLAYERTURN_ATTACK = false; //State PLAYERTURN_ATTACK동안 true
    public bool isStatePLAYERTURN_ATTACK_PartSelecting = false; //PLAYERTURN_ATTACK에서 공격할 Part 선택 시
    public bool isStatePLAYERTURN_RUN = false; //State PLAYERTURN_RUN동안 true
    public bool isStateENEMYTURN = false; //State ENEMYTURM동안 true
    private bool isStateEND = false; //State WIN, LOSE시 true

    public bool isContentTextWriting = false; //ContentTextWrite 중복 실행 방지
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
        enemy = FindObjectOfType<Enemy>();
        part = FindObjectOfType<Part>();
        uiManager = FindObjectOfType<UIManager>();
        
        //Enemy setting
        //if문 돌려서 알맞은 적 SetActive(true);
        if (uiManager != null)
        {
            int stage = UIManager.u_instance.stageIndex;
            Debug.Log($"현재 진행 스테이지: {stage}");

            switch (stage) //임시입니다. 한 스테이지에 2개 이상 Enemy의 경우 if문을 한 번 더 돌리든가 해야지
            {
                case 0:
                    {
                        Debug.Log("stage 인식: Aphrodite로 Battle 실행");
                        Aphrodite.SetActive(true);
                        enemy = Aphrodite.GetComponent<Enemy>();
                        break;
                    }
                case 1:
                    {
                        Debug.Log("stage 인식: 도셔관");

                        if (SceneTransport.previousScene == "Library_B1F")
                        {
                            Debug.Log("stage: 도서관, ReadingChild");
                            ReadingChild.SetActive(true);
                            enemy = ReadingChild.GetComponent<Enemy>();
                            break;
                        }
                        else if (SceneTransport.previousScene == "Library_2F")
                        {
                            Debug.Log("stage: 도서관, Melpomene");
                            Melpomene.SetActive(true);
                            enemy = Melpomene.GetComponent<Enemy>();
                            break;
                        }
                        break;
                    }                    
            }
        }
        else
        {
            isBattleMode = true;

            //임시 Random 구현
            int r = Random.Range(0, 3);
            if (r == 0)
            {
                Aphrodite.SetActive(true);
                enemy = Aphrodite.GetComponent<Enemy>();
            }
            else if (r == 1)
            {
                ReadingChild.SetActive(true);
                enemy = ReadingChild.GetComponent<Enemy>();
            }
            else //(r == 2)
            {
                Melpomene.SetActive(true);
                enemy = Melpomene.GetComponent<Enemy>();
            }
        }
        Debug.Log($"Enemy set to: {enemy}"); //Delete

        filePath = Application.persistentDataPath + "/stage1_statue 3_data.json";
        LoadFightData();
    }
    
    private void Start()
    {
        //Set, 최초 1회만 실행
        player.SetPlayer();
        enemy.SetEnemy();

        //Update, 매 턴마다 실행
        player.UpdatePlayer();
        enemy.UpdateEnemy();

        //UI, Buttons
        battleUI.SetDialogueButtons();
        battleUI.SetPartButtons();

        //Ui, BlackBoard
        battleUI.blackBoard.SetActive(true);
        StartCoroutine(battleUI.FadeInOut(true, 1f));

        state = State.PLAYERTURN_START;
        PlaySFX(battleStartSFX);
    }

    private void Update()
    {
        switch (state)
        {
            //PLAYERTURN
            case State.PLAYERTURN_START:
                if (!isStatePLAYERTURN)
                {
                    player.PlayerTurnStart();
                    //isStatePLAYERTURN = true; //player.PlayerTurnStart() 내부로 이동, 문제 발생 시 롤백
                }
                break;

            case State.PLAYERTURN_ATTACK:
                if (!isStatePLAYERTURN_ATTACK)
                {
                    player.PlayerTurnAttack();
                }
                break;

            case State.PLAYERTURN_RUN:
                if (!isStatePLAYERTURN_RUN)
                {
                    PlayerRun(); //SaveRun으로 함수명 변경해서 player.Run 안에 넣을 수 있나요?
                    player.Run();
                    StartCoroutine(player.Run());
                }
                break;

            //ENEMYTURN
            case State.ENEMYTURN:
                if (!isStateENEMYTURN)
                {
                    StartCoroutine(enemy.EnemyTurnStart()); //Coroutine화 했음
                    //isStateENEMYTURN = true;
                }
                break;

            //END
            case State.WIN:
                if (!isStateEND)
                {
                    isStateEND = true;
                    //Win();
                    StartCoroutine(Win());
                    //PlaySFX(playerWinSFX);
                }
                break;

            case State.LOSE:
                if (!isStateEND)
                {
                    isStateEND = true;
                    //Lose();
                    StartCoroutine(Lose());
                    //PlaySFX(playerLoseSFX);
                }
                break;
        }
    }
    #endregion

    public void Damage(string _object, int _attackpower, Part _part = null)
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
            player.playerHp -= _attackpower;
            player.UpdatePlayer();
        }
        else if (_object == "enemy" && _part != null)
        {
            float attackpower = (float)_attackpower * enemy.increaseAttackPower;

            if (player.isConfused)
            {
                attackpower *= 10;
            }

            _attackpower = (int)attackpower;
            _part.partHp -= _attackpower;
            enemy.UpdateEnemy();
        }
    }

    private void PlayerRun() // 추가 코드
    {
        if (!isBattleMode)
        {
            isWin = false;
            _enterFight = false;
            SaveFightData();

            PlayerPrefs.SetInt("PlayerRun", 1);
            PlayerPrefs.Save();
        }
    }

    #region Battle End
    private IEnumerator Win()
    {
        Debug.Log("PlayerWin() 실행");

        PlaySFX(playerWinSFX);

        //UI
        StartCoroutine(battleUI.FadeInOut(false, 2f));

        if (!isBattleMode)
        {
            //Data
            isWin = true;
            _enterFight = true;
            SaveFightData();

            PlayerPrefs.SetInt("PlayerWin", 1);
            PlayerPrefs.Save();
        }

        yield return new WaitForSeconds(playerWinSFX.length);
        //Invoke("ExitBattleScene", 1); //이거 기다리지 말아야 할 것 같은데?
        ExitBattleScene(); //문제 생기면 롤백
    }

    private IEnumerator Lose()
    {
        Debug.Log("PlayerLose() 실행");

        PlaySFX(playerLoseSFX);

        //UI
        StartCoroutine(ContentTextWriter("눈앞이 흐려진다..."));
        StartCoroutine(battleUI.FadeInOut(false, 2f));
        //구현예정

        if (!isBattleMode)
        {
            isWin = false;
            _enterFight = false;
            SaveFightData();

            PlayerPrefs.SetInt("PlayerLose", 1);
            PlayerPrefs.Save();
        }

        yield return new WaitForSeconds(playerWinSFX.length);
        ExitBattleScene();
        //Invoke("ExitBattleScene", 2); //함수 Lose() Coroutine화 예정
    }

    public void ExitBattleScene()
    {

        if (isBattleMode)
        {
            SceneManager.LoadScene("Start");
        }
        else
        {
            if (state == State.WIN) //승리 시
            {
                if (enemy == Aphrodite.GetComponent<Enemy>())
                {
                    SceneManager.LoadScene("Museum_ExhibitionRoom2");
                }
                else if (enemy == ReadingChild.GetComponent<Enemy>())
                {
                    SceneManager.LoadScene("Library_B1F");
                }
                else if (enemy == Melpomene.GetComponent<Enemy>())
                {
                    SceneManager.LoadScene("Library_2F");
                }
                
            }
            else if (state == State.LOSE || state == State.PLAYERTURN_RUN) //패배||도망 시
            {
                if (enemy == Aphrodite.GetComponent<Enemy>())
                {
                    SceneManager.LoadScene("Museum_Lobby");
                }
                else if (enemy == ReadingChild.GetComponent<Enemy>())
                {
                    SceneManager.LoadScene("Library_B1F");
                }
                else if (enemy == Melpomene.GetComponent<Enemy>())
                {
                    SceneManager.LoadScene("Library_2F");
                }
            }
            else
            {
                return;
            }
        }
    }
    #endregion

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

    public IEnumerator ContentTextWriter(string _text)
    {
        // 이미 코루틴이 실행 중이라면 중복 실행 방지
        if (isContentTextWriting)
        {
            yield break;
        }

        isContentTextWriting = true;
        battleUI.contentText.text = "";

        for (int i = 0; i < _text.Length; i++)
        {
            battleUI.contentText.text += _text[i];
            yield return new WaitForSeconds(0.03f);
        }

        isContentTextWriting = false;
    }
}
