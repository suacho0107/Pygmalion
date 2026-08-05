using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Melpomene : Enemy //멜포메네
{
    #region Variables
    public Dictionary<PartType, bool> canNarrative = new();
    private Dictionary<PartType, bool> narrativeActivated = new();

    public float confusionRate;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        mainPart = GetPart(PartType.LArm);

        //Narrative 발동 가능 여부
        canNarrative[PartType.Mask] = true;
        canNarrative[PartType.LArm] = false; //IsPartDestroyed(PartType.Head);

        //Narrative 발동 여부
        narrativeActivated[PartType.Mask] = false;
        narrativeActivated[PartType.LArm] = false;
    }

    protected override IEnumerator EnemySkill()
    {
        //LArm Narrative Check
        if (IsPartDestroyed(PartType.Mask) && IsPartDestroyed(PartType.Head))
        {
            canNarrative[PartType.LArm] = true;
        }

        //Narrative 실행
        foreach (var part in new List<PartType>(canNarrative.Keys))
        {
            // 최초 1회만
            if (canNarrative[part] && !narrativeActivated[part] && !IsPartDestroyed(part))
            {
                if(Random.value < 0.4f)
                {
                    yield return StartCoroutine(Narrative(part));
                }
            }
        }

        //Narrative 지속 효과
        foreach (var pair in narrativeActivated)
        {
            if (pair.Value)
            {
                confusionRate += 0.05f;
            }
        }
        Debug.Log($"confusionRate = {confusionRate}");

        //Finale
        if (!IsPartDestroyed(PartType.Body) && confusionRate >= 1f)
        {
            Finale();
            yield break;
        }

        ResetSkill();

        AddSkill(null, Shout, IsPartDestroyed(PartType.Mask)&&!IsPartDestroyed(PartType.Head));
        AddSkill(null, Slap, IsPartDestroyed(PartType.RArm)&&!IsPartDestroyed(PartType.LArm));

        if (!IsPartDestroyed(PartType.RArm))
        {
            Bat();
        }
        else if (skills.Count > 0)
        {
            RandomSkill();
        }

        //isConfused
        if (Random.value < confusionRate)
        {
            player.isConfused = true;
        }

        yield return null;

    }

    #region Melpomene_Skills
    private void Shout() //비극의 외침
    {
        StartCoroutine(battleUI.TypeWriter("조각상이 비극을 외쳐, 그 울림이 당신에게 강력한 정신적 충격을 줍니다.\n방어력이 감소합니다."));

        Deal(30);
    }

    private IEnumerator Narrative(PartType part) //운명의 서사
    {
        Debug.Log($"Narrative({part});");

        StartCoroutine(battleUI.TypeWriter("조각상이 당신의 비극적인 운명을 노래합니다.\n운명의 저주가 당신을 천천히 갉아먹습니다."));

        canNarrative[part] = false;

        int damage;

        if (part == PartType.Mask)
        {
            damage = 15;
        }
        else //if (_part == PartType.RArm)
        {
            damage = 10;
        }
        narrativeActivated[part] = true;

        yield return new WaitForSeconds(2f);

        Deal(damage); //Mask, LArm 같은 Skill, 데미지 차이
    }

    public void Redemption() //구속; Player가 Run 선택 시 발동
    {
        StartCoroutine(battleUI.TypeWriter("조각상이 알 수 없는 힘으로 당신을 구속합니다."));

        Deal(5);
    }

    private void Finale() //종막
    {
        StartCoroutine(battleUI.TypeWriter("조각상이 찢어진 ◼◼%▒̵▓̸?▓̸▓̸∅으로 비극을 노래합니다. 당신은 무대에서 영원히 A̴͠A̴͠H̸͠▒̵▒̵▓̸▓̸▓̸"));

        Deal(100);
    }

    private void Bat()//휘두르는 방망이
    {
        StartCoroutine(battleUI.TypeWriter("조각상이 손에 든 커다란 방망이를 휘두릅니다."));

        Deal(15);
    }

    private void Slap() //뺨 후려치기
    {
        StartCoroutine(battleUI.TypeWriter("조각상이 당신의 뺨을 후려칩니다.\n그다지 타격은 없으나 비극적인 기분이 느껴집니다."));

        Deal(5);
    }
    #endregion
}