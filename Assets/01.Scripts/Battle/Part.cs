using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    #region References
    private BattleUI battleUI;
    private Enemy enemy;
    #endregion

    #region Variables
    public PartType partType;

    public int hp;
    public int maxHp;

    public int sort;

    public bool IsDestroyed => hp <= 0;
    #endregion

    private void Awake()
    {
        battleUI = FindObjectOfType<BattleUI>();
        enemy = GetComponentInParent<Enemy>();
    }

    public void SetPart()
    {
        hp = maxHp; //Hp 초기화; 재진입 구현 시 수정
        gameObject.SetActive(true);
    }

    #region Damage
    public void Damaged(int damage)
    {
        if (IsDestroyed)
        {
            return;
        }

        StartCoroutine(DamageRoutine(damage));
    }

    private IEnumerator DamageRoutine(int damage)
    {
        yield return StartCoroutine(battleUI.Shake(enemy.gameObject.transform, 0.2f, 10f));

        hp -= damage;

        enemy.UpdateEnemy();

        if (hp <= 0)
        {
            hp = 0;
            Destroy();
        }
    }
    #endregion

    private void Destroy()
    {
        gameObject.SetActive(false);


        if (enemy.enemyType == EnemyType.Melpomene && this.partType == PartType.Head)
        {
            Melpomene melpomene = GetComponentInParent<Melpomene>();
            melpomene.canNarrative[PartType.LArm] = true;
        }
    }

}
