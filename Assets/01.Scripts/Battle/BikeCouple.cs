using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeCouple : Enemy //자전거 타는 연인
{
    protected override void Awake()
    {
        base.Awake();
        mainPart = GetPart(PartType.WLeg);
    }

    protected override IEnumerator EnemySkill()
    {
        ResetSkill();

        AddSkill(0.2f, Whisper, !IsPartDestroyed(PartType.MHead));
        AddSkill(0.2f, Whisper, !IsPartDestroyed(PartType.WHead));
        AddSkill(0.15f, Connect, !IsPartDestroyed(PartType.MArm));
        AddSkill(0.15f, Connect, !IsPartDestroyed(PartType.WArm));
        AddSkill(0.15f, Rush, !IsPartDestroyed(PartType.MLeg));
        AddSkill(0.15f, Rush, !IsPartDestroyed(PartType.WLeg));

        RandomSkill();        

        yield return null;
    }

    private void Whisper() //사랑의 속삭임
    {
        StartCoroutine(battleUI.TypeWriter("연인이 서로 조용히 속삭이며 당신을 희롱합니다.\n당신은 순간 분노에 휩싸입니다. 공격이 크게 강화되고 방어력이 감소합니다."));

        Deal(10);
        player.isAngry = true;
        increaseAttackPower *= 1.2f;
        Debug.Log($"increaseAttackPower = {increaseAttackPower}");
    }
    private void Connect() //사랑의 연결
    {
        StartCoroutine(battleUI.TypeWriter("연인이 서로의 손을 맞잡고 서로의 몸을 보호합니다."));

        player.isNegated = true;
    }
    private void Rush() //연인의 질주
    {
        StartCoroutine(battleUI.TypeWriter("자전거가 빠르게 달려와 당신과 충돌합니다.\n당신은 큰 충격에 주저앉습니다."));

        Deal(30);
        player.isNegated = true;
    }
}
