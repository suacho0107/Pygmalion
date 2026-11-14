using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LyingFace : Enemy //누워 있는 얼굴
{
    protected override void Awake()
    {
        base.Awake();
        mainPart = GetPart(PartType.Hand);
    }

    protected override IEnumerator EnemySkill()
    {
        ResetSkill();

        AddSkill(20f, Stare, !IsPartDestroyed(PartType.Eye));
        AddSkill(20f, Curse, !IsPartDestroyed(PartType.Mouth));

        if (IsPartDestroyed(PartType.Head) && !IsPartDestroyed(PartType.Eye))
        {
            Glare();
        }
        else if (skills.Count > 0)
        {
            RandomSkill();
        }
        else if (!IsPartDestroyed(PartType.Hand))
        {
            Parry();
        }

        yield return null;
    }

    #region LyingFace_Skills

    private void Stare() //가만히 노려보기
    {
        StartCoroutine(battleUI.TypeWriter("조각상이 당신을 가만히 노려봅니다.\n당신의 마음속에 불쾌한 감정이 싹틉니다."));

        //정신력 소폭 하락
    }

    private void Glare() //무섭게 노려보기
    {
        StartCoroutine(battleUI.TypeWriter("당신의 무자비한 행동에 조각상이 당신을 무섭게 노려봅니다.\n당신의 마음속에 더욱 불쾌한 감정이 싹틉니다."));

        //정신력 하락
    }

    private void Curse() //무언의 저주
    {
        StartCoroutine(battleUI.TypeWriter("조각상이 가만히 당신을 저주합니다.\n방어력이 감소합니다."));

        Deal(10);

        increaseAttackPower = 1.3f;
    }

    private void Parry() //튕겨내기
    {
        StartCoroutine(battleUI.TypeWriter("조각상이 강한 손가락 힘으로 당신을 튕겨냅니다."));

        Deal(25);
    }

    #endregion

}
