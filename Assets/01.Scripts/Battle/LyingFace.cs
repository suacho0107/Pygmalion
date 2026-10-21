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

        /*
        AddSkill(20f, Charm, !IsPartDestroyed(PartType.Head));
        AddSkill(20f, Dance, !IsPartDestroyed(PartType.Body));

        if (skills.Count > 0)
        {
            RandomSkill();
        }
        else if (!IsPartDestroyed(PartType.LArm))
        {
            ThrowApple();
        }
        */

        yield return null;
    }

    
}
