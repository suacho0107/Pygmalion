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

    #region Variables
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

    public Dictionary<EnemyType, int> runHpDict = new Dictionary<EnemyType, int>();

    #region Battle Flags
    private bool isWin;
    private bool _enter1st;
    private bool _enterFight;
    #endregion
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
            //Aphrodite.SetActive(true);
            //enemy = Aphrodite.GetComponent<Enemy>();
            //ReadingChild.SetActive(true);
            //enemy = ReadingChild.GetComponent<Enemy>();
            Melpomene.SetActive(true);
            enemy = Melpomene.GetComponent<Enemy>();

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
            battleUI.ResetUI();

            Melpomene melpomene = FindObjectOfType<Melpomene>();
            melpomene.Redemption();

            yield return new WaitForSeconds(1.5f);
            ChangeState(BattleManager.State.ENEMYTURN);
        }
        else
        {
            //Data
            isWin = false;
            _enter1st = true;
            _enterFight = true;
            runHpDict[enemy.enemyType] = player.hp;
            SaveFightData(true);

            PlayerPrefs.SetInt("PlayerLose", 1);
            PlayerPrefs.Save();

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
    public void SaveFightData(bool isRun = false)
    {
        //string json = JsonUtility.ToJson(npcData);
        //File.WriteAllText(filePath, json);
        //Debug.Log("데이터 저장");

        string path = SceneTransport.previousStatue;

        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            string json = File.ReadAllText(path);
            NPCData data = JsonUtility.FromJson<NPCData>(json);

            if (isRun == false)
            {
                data.isFin = isWin;
                data.enter1st = _enter1st;
                data.enterFight = _enterFight;
            }

            //Run 이후 재진입, Dictionary → List 변환
            data.runHpList.Clear();
            foreach (var kvp in runHpDict)
            {
                data.runHpList.Add(new RunHpData { enemyType = kvp.Key, playerHp = kvp.Value });
            }

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

            //Run 이후 재진입, List → Dictionary 변환
            runHpDict.Clear();
            foreach (var hpData in data.runHpList)
            {
                runHpDict[hpData.enemyType] = hpData.playerHp;
            }
        }
    }
    #endregion

    #region Item Effect
    public IEnumerator ItemEffect(int _itemID)
    {
        battleUI.ResetUI();

        //if (_itemID == 10402 || _itemID == 10403)
        //{
        //    if (player.isMaxhp)
        //    {
        //        yield return StartCoroutine(CannotUse());
        //        yield break;
        //    }
        //}

        switch (_itemID)
        {
            case 10402:
                Debug.Log("비타5000 효과");
                Vita5000();
                break;
            case 10403:
                Debug.Log("포도주 효과");
                Wine();
                break;
            case 20103:
                Debug.Log("수상한 액체가 든 병 효과");
                SuspiciousPotion();
                break;
        }
        yield return new WaitUntil(() => battleUI.isTyping);
        yield return new WaitUntil(() => !battleUI.isTyping);

        yield return new WaitForSeconds(1f);

        ChangeState(State.ENEMYTURN);
    }

    public bool CanUseItem(int _itemID)
    {
        switch (_itemID)
        {
            case 10402: //비타5000
            case 10403: //포도주
                return !player.isMaxhp;

            default: //조건 없는 item
                return true;
        }
    }

    public IEnumerator CannotUseItem()
    {        
        yield return StartCoroutine(battleUI.TypeWriter("지금은 사용할 수 없다."));

        yield return new WaitForSeconds(1f);

        ChangeState(BattleManager.State.PLAYERTURN_START);
    }

    public void Vita5000() //비타5000
    {
        StartCoroutine(battleUI.TypeWriter("[비타 5000]을 꿀꺽꿀꺽 마셨다.\n기운이 넘친다!"));
        player.Heal((int)(player.maxHp * 0.5));
    }

    public void Wine() //포도주
    {
        StartCoroutine(battleUI.TypeWriter("[포도주]를 꿀꺽꿀꺽 마셨다.\n생명력이 느껴진다!"));
        player.Heal((int)(player.maxHp * 0.99));
    }

    public void SuspiciousPotion() //수상한 액체가 든 병
    {
        battleUI.ResetUI();

        StartCoroutine(battleUI.TypeWriter("[수상한 액체가 든 병]을 조각상에 던진다."));

        foreach (Part part in enemy.parts)
        {
            if (!part.IsDestroyed)
            {
                part.Damaged(1);
            }
        }
    }
    #endregion
}
