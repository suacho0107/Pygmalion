using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dolphin : Enemy //돌고래
{
    protected override void Awake()
    {
        base.Awake();
        mainPart = GetPart(PartType.FirstFin);
    }

    protected override IEnumerator EnemySkill()
    {
        ResetSkill();

        //AddSkill(20f, Stare, !IsPartDestroyed(PartType.Eye));
        //AddSkill(20f, Curse, !IsPartDestroyed(PartType.Mouth));

        //if (IsPartDestroyed(PartType.Head) && !IsPartDestroyed(PartType.Eye))
        //{
        //    Glare();
        //}
        //else if (skills.Count > 0)
        //{
        //    RandomSkill();
        //}
        //else if (!IsPartDestroyed(PartType.Hand))
        //{
        //    Parry();
        //}

        yield return null;
    }

    #region Dolphin_Skills
    public void WaterJet(PartType _part) //물줄기 발사
    {
        StartCoroutine(battleUI.TypeWriter("돌고래가 당신에게 강한 물줄기를 뿜어냅니다."));

        int damage;
        if (_part == PartType.ThirdHead)
        {
            damage = 15;
        }
        else if (_part == PartType.SecondHead)
        {
            damage = 20;
                }
        else //if (_part == PartType.FirstHead)
        {
            damage = 25;
        }
        Deal(damage);
    }

    public void WaterBarrage(PartType _part) //물줄기 폭격
    {
        StartCoroutine(battleUI.TypeWriter("돌고래가 당신에게 해로운 물줄기를 뿜어냅니다.\n모든 일에는 순서가 있습니다."));

        int damage;
        if (_part == PartType.ThirdHead)
        {
            damage = 35;
        }
        else //if (_part == PartType.SecondHead)
        {
            damage = 40;
        }
        Deal(damage);
    }

    public void RagingWave(PartType _part) //요란한 파도
    {
        StartCoroutine(battleUI.TypeWriter("돌고래가 지느러미를 찰박이며 강한 파도를 일으킵니다.\n당신은 몸이 무거워집니다."));

        int damage;
        if (_part == PartType.ThirdFin)
        {
            damage = 10;
        }
        else if (_part == PartType.SecondFin)
        {
            damage = 15;
        }
        else //if (_part == PartType.FirstFin)
        {
            damage = 20;
        }
        Deal(damage);
    }

    public void Splash(PartType _part) //찰박이기
    {
        StartCoroutine(battleUI.TypeWriter("돌고래가 꼬리를 귀엽게 찰박입니다.\n당신은 자신도 모르게 무장해제됩니다."));

        int damage;
        if (_part == PartType.ThirdTail)
        {
            damage = 5;
        }
        else if (_part == PartType.SecondTail)
        {
            damage = 10;
        }
        else //if (_part == PartType.FirstTail)
        {
            damage = 15;
        }
        Deal(damage);
    }
    #endregion
}
