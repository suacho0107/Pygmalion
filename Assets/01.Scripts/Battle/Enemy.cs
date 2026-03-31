using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class Enemy : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] private BattleManager battleManager;
    [SerializeField] protected BattleUI battleUI;
    [SerializeField] protected BattleSFX battleSFX;
    [SerializeField] protected Player player;
    #endregion

    #region Variables
    [Header("Enemy Info")]
    public EnemyType enemyType;

    public int hp;
    private int maxHp;
    [SerializeField] protected Image hpBar;

    public List<Part> parts = new();
    protected Part mainPart;

    protected List<Action> skills = new();

    protected float increaseAttackPower = 1.0f;
    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        // 하위 파츠 자동 수집
        parts.AddRange(GetComponentsInChildren<Part>());
    }
    #endregion

    #region Initialization
    public void SetEnemy() // 전투 시작 시 1회 호출
    {
        increaseAttackPower = 1.0f;

        // 파츠 정렬
        parts.Sort((a, b) => a.sort.CompareTo(b.sort));

        maxHp = 0;

        foreach (Part part in parts)
        {
            part.SetPart();
            maxHp += part.maxHp;
        }

        hp = maxHp;
    }

    public void UpdateEnemy() // 매 턴마다 호출 (HP 갱신)
    {
        hp = 0;

        foreach (Part part in parts)
        {
            if (!part.IsDestroyed)
                hp += part.hp;
        }

        StartCoroutine(battleUI.UpdateHpBar(hpBar, hp, maxHp));
    }
    #endregion

    #region Part
    protected Part GetPart(PartType type)
    {
        return parts.Find(p => p.partType == type);
    }

    public bool IsPartDestroyed(PartType type)
    {
        Part part = GetPart(type);
        return part == null || part.IsDestroyed;
    }

    public bool IsMainPartDestroyed()
    {
        return mainPart == null || mainPart.IsDestroyed;
    }
    #endregion

    #region Turn Flow
    public IEnumerator StartTurn()
    {
        yield return new WaitForSeconds(0.3f);

        yield return EnemySkill();

        yield return new WaitUntil(() => !battleUI.isTyping);
        yield return new WaitForSeconds(1.5f);

        EndTurn();
    }

    private void EndTurn()
    {
        // 패배 조건
        if (player.hp <= 0)
        {
            battleManager.ChangeState(BattleManager.State.LOSE);
            return;
        }

        // 승리 조건
        if (IsMainPartDestroyed())
        {
            battleManager.ChangeState(BattleManager.State.WIN);
            return;
        }

        // 상태 처리
        if (player.isCharmed)
        {
            player.isCharmed = false;
            battleManager.ChangeState(BattleManager.State.ENEMYTURN);
        }
        else
        {
            battleManager.ChangeState(BattleManager.State.PLAYERTURN_START);
        }

    }
    #endregion

    #region Skill
    protected abstract IEnumerator EnemySkill();

    protected void AddSkill(float? probability, Action skill, bool partAlive)
    {
        if (!partAlive)
        {
            return;
        }

        if (probability == null)
        {
            skills.Add(skill);
        }
        else if (Random.value <= probability)
        {
            skills.Add(skill);
        }
    }

    protected void RandomSkill()
    {
        if (skills.Count <= 0)
        {
            return;
        }

        skills[Random.Range(0, skills.Count)]();
    }

    protected void ResetSkill()
    {
        skills.Clear();
    }
    #endregion

    #region Damage
    protected void Deal(int baseDamage)
    {
        battleSFX.Play(battleSFX.enemyAttack);

        int Damage = Mathf.RoundToInt(baseDamage * increaseAttackPower);
        player.Damaged(Damage);
    }
    #endregion
}
