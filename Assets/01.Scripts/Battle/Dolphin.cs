using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dolphin : Enemy //µ¹°í·¡
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

    #region LyingFace_Skills
    #endregion
}
