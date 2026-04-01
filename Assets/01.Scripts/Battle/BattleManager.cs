using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    #region References
    [SerializeField] private BattleUI battleUI;
    [SerializeField] private BattleSFX battleSFX;
    [SerializeField] private BattleBGM battleBGM;
    [SerializeField] private Player player;
    private Enemy enemy;

    public NPCData npcData = new NPCData();
    #endregion

    #region Enemy Objects
    [Header("Enemies")]
    public GameObject Aphrodite;
    public GameObject ReadingChild;
    public GameObject Melpomene;
    #endregion

    #region Player Position
    [Header("Player Position")]
    [SerializeField] private PlayerPosition playerPos;
    #endregion

    #region Battle Flags
    private bool isWin;
    private bool _enter1st;
    private bool _enterFight;
    #endregion

    #region State
    public State state;

    public enum State
    {
        START,
        PLAYERTURN_START,
        PLAYERTURN_ATTACK,
        PLAYERTURN_INVENTORY,
        ENEMYTURN,
        WIN,
        LOSE,
        RUN
    }

    public void ChangeState(State newState)
    {
        state = newState;
        Debug.Log($"BattleManager State: {state}");

        switch (state)
        {
            case State.START:
                StartBattle();
                break;

            case State.PLAYERTURN_START:
                player.StartTurn();
                break;

            case State.PLAYERTURN_ATTACK:
                player.AttackTurn();
                break;

            case State.PLAYERTURN_INVENTORY:
                player.InventoryTurn();
                break;

            case State.ENEMYTURN:
                StartCoroutine(enemy.StartTurn());
                break;

            case State.WIN:
                StartCoroutine(Win());
                break;

            case State.LOSE:
                StartCoroutine(Lose());
                break;

            case State.RUN:
                StartCoroutine(Run());
                break;
        }
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        SetEnemyByStage();

        LoadFightData();
    }

    private void Start()
    {
        ChangeState(State.START);
    }
    #endregion

    #region Battle Setup
    private void SetEnemyByStage()
    {
        if (UIManager.u_instance != null)
        {
            int stage = UIManager.u_instance.stageIndex;

            battleUI.SetBackground(stage);

            switch (stage)
            {
                case 0:
                    {
                        if (SceneTransport.previousScene == "Museum_ExhibitionRoom2")
                        {
                            Aphrodite.SetActive(true);
                            enemy = Aphrodite.GetComponent<Enemy>();

                            playerPos.nextPosition = new Vector3(-2.55f, 11.5f, 0f);
                            playerPos.isChecked = true;
                            break;
                        }
                        break;
                    }
                case 1:
                    {
                        if (SceneTransport.previousScene == "Library_B1F")
                        {
                            ReadingChild.SetActive(true);
                            enemy = ReadingChild.GetComponent<Enemy>();

                            playerPos.nextPosition = new Vector3(-0.6f, 7f, 0f);
                            playerPos.isChecked = true;
                        }
                        else if (SceneTransport.previousScene == "Library_2F")
                        {
                            Melpomene.SetActive(true);
                            enemy = Melpomene.GetComponent<Enemy>();

                            playerPos.nextPosition = new Vector3(-9f, -3f, 0f);
                            playerPos.isChecked = true;
                        }
                        break;
                    }
                default:
                    Aphrodite.SetActive(true);
                    enemy = Aphrodite.GetComponent<Enemy>();

                    playerPos.nextPosition = new Vector3(-2.55f, 11.5f, 0f);
                    playerPos.isChecked = true;
                    break;
            }
        }
        else
        {
            Aphrodite.SetActive(true);
            enemy = Aphrodite.GetComponent<Enemy>();
            //Melpomene.SetActive(true);
            //enemy = Melpomene.GetComponent<Enemy>();

            playerPos.nextPosition = new Vector3(-2.55f, 11.5f, 0f);
            playerPos.isChecked = true;
        }
    }

    private void StartBattle()
    {
        player.SetPlayer();
        enemy.SetEnemy();

        ChangeState(State.PLAYERTURN_START);
    }
    #endregion

    #region Player Input
    public void OnSelectAttack()
    {
        ChangeState(State.PLAYERTURN_ATTACK);
    }

    public void OnSelectInventory()
    {
        ChangeState(State.PLAYERTURN_INVENTORY);
    }

    public void OnSelectRun()
    {
        ChangeState(State.RUN);
    }

    public void OnSelectPart(Part part)
    {
        StartCoroutine(player.Attack(part));
    }
    #endregion


    #region Battle End
    private IEnumerator Win()
    {
        battleUI.Win();
        battleSFX.Play(battleSFX.win);
        battleBGM.FadeOut(2f);

        //Data
        isWin = true;
        _enter1st = true;
        _enterFight = true;
        SaveFightData();

        PlayerPrefs.SetInt("PlayerWin", 1);
        PlayerPrefs.Save();

        yield return new WaitForSeconds(battleSFX.win.length);
        ExitBattleScene();
    }

    private IEnumerator Lose()
    {
        battleUI.Lose();
        battleSFX.Play(battleSFX.lose);
        battleBGM.FadeOut(2f);

        //Data
        isWin = false;
        _enter1st = true;
        _enterFight = true;
        SaveFightData();

        PlayerPrefs.SetInt("PlayerLose", 1);
        PlayerPrefs.Save();

        yield return new WaitForSeconds(battleSFX.win.length);
        ExitBattleScene();
    }

    public IEnumerator Run()
    {
        if (enemy.enemyType == EnemyType.Melpomene && !enemy.IsPartDestroyed(PartType.Body)) //멜포메네
        {
            Melpomene melpomene = FindObjectOfType<Melpomene>();
            melpomene.Redemption();

            yield return new WaitForSeconds(1.5f);
            ChangeState(BattleManager.State.ENEMYTURN);
        }
        else
        {
            //여기서 bool로 도망 여부 저장해서 재진입 시 Setting 변경하기?

            battleUI.Run();
            battleSFX.Play(battleSFX.run);
            battleBGM.FadeOut(2f);

            yield return new WaitForSeconds(2f); //SFX가 길어서 그냥 2초 듣고 잘라
            ExitBattleScene();
        }
    }
    #endregion

    #region Scene Control
    public void ExitBattleScene()
    {
        string path = SceneTransport.previousStatue;

        if (state == State.WIN) //승리 시
        {
            if (enemy.enemyType == EnemyType.Aphrodite)
            {
                SceneManager.LoadScene("Museum_ExhibitionRoom2");
            }
            else if (enemy.enemyType == EnemyType.ReadingChild)
            {
                SceneManager.LoadScene("Library_B1F");
            }
            else if (enemy.enemyType == EnemyType.Melpomene)
            {
                SceneManager.LoadScene("Library_2F");
            }
        }
        else if (state == State.LOSE) //패배 시
        {
            SceneManager.LoadScene("Monologue_defeat");
        }
        else if (state == State.RUN) // 도망 시
        {
            if (enemy.enemyType == EnemyType.Aphrodite)
            {
                SceneManager.LoadScene("Monologue_run_mus-1");
            }
            else if (enemy.enemyType == EnemyType.ReadingChild)
            {
                SceneManager.LoadScene("Monologue_run_lib-1");
            }
            else if (enemy.enemyType == EnemyType.Melpomene)
            {
                SceneManager.LoadScene("Monologue_run_lib-2");
            }
        }
    }
    #endregion



    #region Save / Load
    public void SaveFightData()
    {
        //string json = JsonUtility.ToJson(npcData);
        //File.WriteAllText(filePath, json);
        //Debug.Log("데이터 저장");

        string path = SceneTransport.previousStatue;

        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            string json = File.ReadAllText(path);
            NPCData data = JsonUtility.FromJson<NPCData>(json);

            data.isFin = isWin;
            data.enter1st = _enter1st;
            data.enterFight = _enterFight;

            File.WriteAllText(path, JsonUtility.ToJson(data));
            Debug.Log(path + " 전투 데이터 저장");
        }
    }

    public void LoadFightData()
    {
        //if (File.Exists(filePath))
        //{
        //    string json = File.ReadAllText(filePath);
        //    npcData = JsonUtility.FromJson<NPCData>(json);
        //    Debug.Log(filePath + " 전투 데이터 로드");
        //}

        string path = SceneTransport.previousStatue;

        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            string json = File.ReadAllText(path);
            NPCData data = JsonUtility.FromJson<NPCData>(json);
            Debug.Log(path + " 전투 데이터 로드");

            isWin = data.isFin;
            _enter1st = data.enter1st;
            _enterFight = data.enterFight;
        }
    }
    #endregion
}
