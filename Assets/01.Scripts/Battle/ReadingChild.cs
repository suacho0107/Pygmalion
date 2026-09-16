using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadingChild : Enemy //책을 읽는 아이
{
    protected override void Awake()
    {
        base.Awake();
        mainPart = GetPart(PartType.RLeg);
    }

    protected override IEnumerator EnemySkill()
    {
        ResetSkill();

        AddSkill(null, Storyteller, !IsPartDestroyed(PartType.Head));
        AddSkill(null, Kick, !IsPartDestroyed(PartType.RLeg));

        if (!IsPartDestroyed(PartType.RArm))
        {
            BookShelf(PartType.RArm);
        }
        else if (!IsPartDestroyed(PartType.LArm))
        {
            BookShelf(PartType.LArm);
        }
        else //if (skills.Count > 0)
        {
            RandomSkill();
        }

        yield return null;
    }

    #region ReadingChild_Skills
    private void Storyteller() //타고난 이야기꾼
    {
        StartCoroutine(battleUI.TypeWriter("타고난 이야기꾼인 조각상은 흥미로운 이야기를 들려줍니다.\n당신은 환상에 휘말립니다."));

        player.isConfused = true;
    }
    private void BookShelf(PartType _part) //날카로운 책장
    {
        StartCoroutine(battleUI.TypeWriter("조각상이 책에서 페이지를 뽑아 날카로운 종이의 칼날을 휘두릅니다."));

        int damage;

        if (_part == PartType.RArm)
        {
            damage = 20;
        }
        else //if (_part == PartType.LArm)
        {
            damage = 15;
        }

        Deal(damage); //LArm, RArm 같은 스킬, 데미지 차이
    }
    private void Kick() //걷어차기
    {
        StartCoroutine(battleUI.TypeWriter("아무것도 남지 않은 조각상이 당신을 힘껏 걷어찹니다."));

        Deal(20);
    }
    #endregion
}
