using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Aphrodite : Enemy //아프로디테
{
    protected override void Awake()
    {
        base.Awake();
        mainPart = GetPart(PartType.LArm);
    }

    protected override IEnumerator EnemySkill()
    {
        ResetSkill();

        AddSkill(0.2f, Charm, !IsPartDestroyed(PartType.Head));
        AddSkill(0.2f, Dance, !IsPartDestroyed(PartType.Body));

        if (skills.Count > 0)
        {
            RandomSkill();
        }
        else if (!IsPartDestroyed(PartType.LArm))
        {
            ThrowApple();
        }

        yield return null;
    }

    #region Aphrodite Skills
    private void Charm() //매혹의 눈빛
    {
        StartCoroutine(battleUI.TypeWriter("조각상이 매혹적인 눈빛을 보내 당신을 완전히 매료시킵니다."));

        player.isCharmed = true;
    }
    private void Dance() //황홀한 춤
    {
        StartCoroutine(battleUI.TypeWriter("조각상이 황홀한 춤을 춰 당신을 크게 매료시킵니다.\n당신은 무방비 상태가 됩니다."));

        increaseAttackPower = 1.1f;
    }
    private void ThrowApple() //황금 사과 투척
    {
        StartCoroutine(battleUI.TypeWriter("조각상이 황금 사과를 던져 당신을 공격합니다."));

        Deal(15);
    }
    #endregion
}
