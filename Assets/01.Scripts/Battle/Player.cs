using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region References
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private BattleUI battleUI;
    [SerializeField] private BattleSFX battleSFX;
    private Enemy enemy;
    #endregion

    #region Player Info
    [Header("Player Info")]
    public int hp;
    public int maxHp = 100;
    public bool isMaxhp => hp >= maxHp;

    private int attackPower = 1; //피 1칸씩 깔 거임

    //[SerializeField] private Image hpBar;
    [SerializeField] private HpBarUI hpBar;
    #endregion

    #region status
    [Header("Status")]
    public bool isCharmed; //매혹
    public bool isConfused; //혼란
    public bool isAngry; //분노
    public bool isNegated; //무효
    #endregion

    #region Unity Methods
    private void Awake()
    {
        enemy = FindObjectOfType<Enemy>();
    }
    #endregion

    #region Initialization
    public void SetPlayer() //사실상 SetPlayerHP임
    {
        //hp = maxHp; //재진입 구현 시 수정
        hp = (battleManager.runHpDict.TryGetValue(enemy.enemyType, out int savedHp)) ? savedHp : maxHp;

        UpdatePlayer();
    }

    public void UpdatePlayer() //UpdatePlayerHp(), 매 턴마다 실행
    {
        StartCoroutine(battleUI.UpdateHpBar(hpBar, hp, maxHp));
    }
    #endregion

    #region Turn Flow
    public void StartTurn()
    {
        battleUI.Playerturn_Start();
    }

    public void AttackTurn()
    {
        battleUI.Playerturn_Attack();
    }

    public IEnumerator Attack(Part part)
    {
        battleUI.ResetUI();

        battleSFX.Play(battleSFX.playerAttack);

        yield return null;

        if (isConfused) // 혼란 상태: 자기 자신 공격
        {
            StartCoroutine(battleUI.TypeWriter(" 당신은 순간 혼란에 빠져 스스로를 공격합니다."));

            Damaged(attackPower);
        }
        if (isAngry) //분노 상태: 1턴 동안 공격력 2배
        {
            //대사 추가시 수정
            StartCoroutine(battleUI.TypeWriter($" {battleUI.TranslateEnemy(enemy)}의 {battleUI.TranslatePart(part)}{battleUI.KorParticle(battleUI.TranslatePart(part), "을", "를")} 공격합니다."));
            
            part.Damaged(attackPower * 2);
        }
        if (isNegated) //무효
        {
            StartCoroutine(battleUI.TypeWriter(" 공격 무효"));
        }
        else // 일반 공격
        {
            StartCoroutine(battleUI.TypeWriter($" {battleUI.TranslateEnemy(enemy)}의 {battleUI.TranslatePart(part)}{battleUI.KorParticle(battleUI.TranslatePart(part), "을", "를")} 공격합니다."));

            part.Damaged(attackPower);
        }

        yield return new WaitForSeconds(1.5f);

        EndTurn();
    }

    public void InventoryTurn()
    {
        battleUI.PlayerTurn_Inventory();
    }

    private void EndTurn()
    {
        // 상태 초기화
        isConfused = false;
        isAngry = false;
        isNegated = false;

        //Inventory UI
        if (InventoryUI.instance.activeInventory)
        {
            InventoryUI.instance.activeInventory = false;
        }

        // 패배 조건
        if (hp <= 0)
        {
            battleManager.ChangeState(BattleManager.State.LOSE);
        }
        // 승리 조건
        else if (enemy.IsMainPartDestroyed())
        {
            battleManager.ChangeState(BattleManager.State.WIN);
        }
        // 다음 턴
        else
        {
            battleManager.ChangeState(BattleManager.State.ENEMYTURN);
        }
    }
    #endregion

    #region Damage & Heal
    public void Damaged(int damage)
    {
        if (isConfused)
        {
            damage *= 10;
        }

        StartCoroutine(battleUI.Shake(gameObject.transform, 0.2f, 10f));

        hp -= damage;

        if (hp <= 0)
        {
            hp = 0;
        }

        UpdatePlayer();
    }

    public void Heal(int heal)
    {
        hp += heal;

        if (hp > maxHp)
        {
            hp = maxHp;
        }

        UpdatePlayer();
    }
    #endregion
}