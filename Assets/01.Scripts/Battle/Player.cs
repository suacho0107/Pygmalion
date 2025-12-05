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
    #endregion

    #region Set & Update
    public void SetPlayer() //사실상 SetPlayerHP임
    {
        playerHp = playerMaxHp; //Hp 초기화; 재진입 구현 시 수정
    }

    public void UpdatePlayer() //UpdatePlayerHp(), 매 턴마다 실행
    {
        playerHpBar.value = (float)playerHp / playerMaxHp;
    }
    #endregion

    #region PlayerTurn Control
    public void PlayerTurnStart()
    {
        Debug.Log("PlayerTurnStart() 실행");

        //UI
        StartCoroutine(battleManager.ContentTextWriter("어떤 행동을 할까?"));
        battleUI.dialogueButtons.gameObject.SetActive(true);

        //State
        battleManager.isStatePLAYERTURN = true;
    }
    public void PlayerTurnAttack()
    {
        //State
        battleManager.isStatePLAYERTURN_ATTACK = true;

        //UI
        battleUI.contentText.text = "어느 부위를 공격할까?";

        //battleUI.currentPartButtonIndex = 0; //초기화
        battleUI.UpdatePartButtons();
        battleUI.partButtons.SetActive(true);

        //Next State
        battleManager.isStatePLAYERTURN_ATTACK_PartSelecting = true;
    }

    public IEnumerator PlayerAttack(Part _part)
    {
        Debug.Log($"PlayerAttack({_part}) 실행"); //Delete

        //UI
        battleUI.contentText.text = "";
        battleUI.partButtons.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        if (isConfused) //혼란 시 공격 대사
        {
            StartCoroutine(battleManager.ContentTextWriter("당신은 순간 혼란에 빠져 스스로를 공격합니다."));
        }
        else //일반 공격 대사
        {
            StartCoroutine(battleManager.ContentTextWriter($"{enemy.ReplaceEnemyText(enemy.name)}의 {enemy.ReplacePartText(_part.name)}{battleUI.KorParticle(enemy.ReplacePartText(_part.name), "을", "를")} 공격합니다."));
        }

        //SFXs
        battleManager.battleAudioSource.Stop();
        battleManager.battleAudioSource.clip = battleManager.playerAttackSFX;
        battleManager.battleAudioSource.time = 0;
        battleManager.battleAudioSource.Play();

        //Attack
        battleManager.Damage("enemy", attackDamage, _part); //attackDamage만큼 partHp 차감

        yield return new WaitForSeconds(1.5f);
        PlayerTurnEnd();
    }

    private void PlayerTurnEnd()
    {
        Debug.Log("PlayerTurnEnd() 실행");

        //초기화
        isConfused = false;

        if (enemy.isDestroyed[battleUI.FindListIndex(enemy.parts, enemy.mainPart)]) //WIN
        {
            battleManager.state = BattleManager.State.WIN;
        }
        else if (playerHp <= 0) //LOSE
        {
            battleManager.state = BattleManager.State.LOSE;
        }
        else //Next
        {
            battleManager.state = BattleManager.State.ENEMYTURN;
        }

        //초기화
        battleManager.isStatePLAYERTURN = false;
        battleManager.isStatePLAYERTURN_ATTACK = false;
    }
    #endregion

    #region Select Control
    public void SelectAttack()
    {   
        //UI
        battleUI.dialogueButtons.gameObject.SetActive(false);

        //State
        battleManager.state = BattleManager.State.PLAYERTURN_ATTACK;
    }

    public void SelectInventory()
    {
        ////State
        //battleManager.state = BattleManager.State.PLAYERTURN_INVENTORY;

        //구현예정
    }

    public void SelectRun()
    {
        //State
        battleManager.state = BattleManager.State.PLAYERTURN_RUN;
    }

    #endregion

    public IEnumerator Run()
    {
        //State
        battleManager.isStatePLAYERTURN_RUN = true;

        if (enemy.enemyName == "Melpomene" && !enemy.isDestroyed[battleUI.FindListIndex(enemy.parts, "Body")]) //멜포메네
        {
            enemy.Melpomene_Redemption();

            //Next State
            yield return new WaitForSeconds(2f);
            battleManager.state = BattleManager.State.ENEMYTURN;

            battleManager.isStatePLAYERTURN = false;
        }
        else
        {
            //여기서 bool로 도망 여부 저장해서 재진입 시 Setting 변경하기?

            Debug.Log("Run!!!!!!!!!");

            //UI
            battleUI.contentText.text = "";
            battleUI.dialoguePanel.SetActive(false);
            StartCoroutine(battleUI.FadeInOut(false, 2f));

            ////SFX
            //battleManager.PlaySFX(battleManager.playerRunSFX);
            //이것만 소리 안 나서 그냥 냅다 실행하기
            battleManager.battleAudioSource.Stop();
            battleManager.battleAudioSource.clip = battleManager.playerRunSFX;
            battleManager.battleAudioSource.time = 0;
            battleManager.battleAudioSource.Play();

            //Next
            yield return new WaitForSeconds(2f); //SFX가 길어서 그냥 2초 듣고 잘라
            //yield return new WaitForSeconds(battleManager.playerRunSFX.length);
            battleManager.ExitBattleScene();
        }
    }    
}