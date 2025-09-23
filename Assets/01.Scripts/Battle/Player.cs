using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region References
    BattleUI battleUI;
    BattleManager battleManager;
    Enemy enemy;
    #endregion

    #region Variables
    public int playerHp;
    private int playerMaxHp = 100; //임의 설정
    //private int playerMaxHp = 30; //Test용 임의 설정

    private int attackDamage = 1; //피 1칸씩 깔 거임

    public Slider playerHpBar;

    public bool isCharmed; //매혹
    public bool isConfused; //혼란
    #endregion

    #region Unity Methods
    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
        battleUI = FindObjectOfType<BattleUI>();
        enemy = FindObjectOfType<Enemy>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    public void SetPlayerHp()
    {
        playerHp = playerMaxHp; // Hp 초기화; 재진입 구현 시 수정
    }

    public void UpdatePlayerHp() //매 턴마다 실행
    {
        playerHpBar.value = (float)playerHp / playerMaxHp;
    }

    public void PlayerTurnStart()
    {
        Debug.Log("PlayerTurnStart() 실행");
        StartCoroutine(battleManager.ContentTextWriter("어떤 행동을 할까?"));
        battleUI.dialogueButtons.gameObject.SetActive(true);
    }

    public void AttackButton()
    {
        //Debugging, 삭졔
        if (battleManager == null)
        {
            Debug.LogError("BattleManager is not assigned.");
            return;
        }

        if (enemy == null)
        {
            Debug.LogError("Enemy is not assigned.");
            return;
        }

        if (enemy.parts == null || enemy.parts.Count == 0)
        {
            Debug.LogError("Enemy parts list is not initialized or empty.");
            return;
        }

        if (enemy.isDestroyed == null || enemy.isDestroyed.Count != enemy.parts.Count)
        {
            Debug.LogError("Enemy isDestroyed list is not initialized or mismatched with parts.");
            return;
        }


        battleUI.dialogueButtons.gameObject.SetActive(false);

        battleManager.state = BattleManager.State.PLAYERTURN_ATTACK;
    }

    public void SelectAttackPart()
    {
        battleManager.isPlayerAttacking = true;

        battleUI.contentText.text = "공격 부위 선택";

        //battleUI.currentPartButtonIndex = 0; //초기화
        battleUI.UpdatePartButtons();
        battleUI.partButtons.SetActive(true);

        battleManager.isPartSelecting = true;
    }

    public void InventoryButton()
    {
        //battleManager.state = BattleManager.State.PLAYERTURN_INVENTORY;

        //구현예정
    }

    public void RunButton()
    {
        battleManager.state = BattleManager.State.PLAYERTURN_RUN;
    }
    
    public void Run()
    {
        battleManager.isPlayerRunning = true;

        if (enemy.enemyName == "Melpomene" && !enemy.isDestroyed[battleUI.FindListIndex(enemy.parts, "Body")]) //멜포메네
        {
            enemy.Melpomene_Redemption();

            Invoke("ToStateEnemyTurn", 2);

            battleManager.isPlayerTurnStarted = false;
        }
        else
        {
            //여기서 bool로 도망 여부 저장해서 재진입 시 Setting 변경하기?
            battleUI.contentText.text = "";

            battleManager.PlaySFX(battleManager.playerRunSFX);
            ////이것만 소리 안 나서 그냥 냅다 실행하기
            //battleManager.battleAudioSource.Stop();
            //battleManager.battleAudioSource.clip = battleManager.playerRunSFX;
            //battleManager.battleAudioSource.time = 0;
            //battleManager.battleAudioSource.Play();

            battleManager.Invoke("ExitBattleScene", 3);
        }
    }

    public void ToStateEnemyTurn()
    {
        battleManager.state = BattleManager.State.ENEMYTURN;
    }


    public void PlayerAttack(Part part)
    {
        Debug.Log("PlayerAttack(enemy, part) 실행");

        battleUI.partButtons.SetActive(false);
        battleUI.contentText.text = $"{enemy.ReplacePartText(part.name)}을/를 공격했다.";

        battleManager.battleAudioSource.Stop();
        battleManager.battleAudioSource.clip = battleManager.playerAttackSFX;
        battleManager.battleAudioSource.time = 0;
        battleManager.battleAudioSource.Play();

        battleManager.Damage("enemy", attackDamage, part); //attackDamage만큼 partHp 차감

        //PlayerTurnEnd();
        Invoke("PlayerTurnEnd", 1);
    }

    void PlayerTurnEnd()
    {
        Debug.Log("PlayerTurnEnd() 실행");

        isConfused = false;

        if (enemy.isDestroyed[battleUI.FindListIndex(enemy.parts, enemy.mainPart)]) //공략 부위 파괴
        {
            battleManager.state = BattleManager.State.WIN;
        }
        else
        {
            battleManager.state = BattleManager.State.ENEMYTURN;
        }

        battleManager.isPlayerTurnStarted = false;
        battleManager.isPlayerAttacking = false;
    }
}