using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    BattleManager battleManager;
    Player player;

    public int enemyHp;
    private int enemyMaxHp;

    public Slider enemyHpBar;

    public List<string> parts = new List<string>(); //partComponents도 포함한 Dictionary로 변환하기?
    public List<Part> partComponents = new List<Part>(); //UpdatehpBox 등에서 사용
    public string currentPart;
    public List<bool> isDestroyed = new List<bool>();

    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
        player = FindObjectOfType<Player>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void StartSetEnemy() //최초 전투 진입 시에만 실행
    //{
    //    //SetPart(), SetHp() 합침
    //    Debug.Log("StartSetEnemy() 실행");
    //    // 초기화
    //    parts.Clear();
    //    partComponents.Clear();
    //    isDestroyed.Clear();
    //    enemyMaxHp = 0;

    //    for (int i = 0; i < transform.childCount - 1; i++)
    //    {
    //        parts.Add(transform.GetChild(i).gameObject.name); //List parts에 Object들 이름 추가
    //        Debug.Log($"parts.Add(${parts[i]})");

    //        partComponents.Add(transform.GetChild(i).GetComponent<Part>());
    //        partComponents[i].SetPartHp(); //partHP 초기화

    //        enemyMaxHp += partComponents[i].partMaxHp; //partMaxHp 합산

    //        isDestroyed.Add(false); //parts 길이만큼 isDestroyed false로 초기화
    //    }
    //    enemyHp = enemyMaxHp;
    //    Debug.Log($"enemyMaxHp = ${enemyMaxHp}\nenemyHp = ${enemyHp}");
    //}

    public void StartSetEnemy() // 최초 전투 진입 시에만 실행
    {
        Debug.Log("StartSetEnemy() 실행");

        // 초기화
        parts.Clear();
        partComponents.Clear();
        isDestroyed.Clear();
        enemyMaxHp = 0;

        // 모든 자식의 Part 컴포넌트를 리스트에 추가
        List<Part> sortedParts = new List<Part>();
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Part partComponent = transform.GetChild(i).GetComponent<Part>();
            if (partComponent != null)
            {
                sortedParts.Add(partComponent);
            }
        }

        // partSort를 기준으로 정렬
        sortedParts.Sort((a, b) => a.partSort.CompareTo(b.partSort));

        // 정렬된 순서대로 parts, partComponents, isDestroyed 리스트 채우기
        foreach (Part part in sortedParts)
        {
            parts.Add(part.gameObject.name); // Object 이름 추가
            Debug.Log($"parts.Add({part.gameObject.name})");

            partComponents.Add(part); // Part 컴포넌트 추가
            part.SetPartHp(); // partHP 초기화

            enemyMaxHp += part.partMaxHp; // partMaxHp 합산
            isDestroyed.Add(false); // isDestroyed 초기화
        }

        // Enemy 전체 HP 초기화
        enemyHp = enemyMaxHp;
        Debug.Log($"enemyMaxHp = {enemyMaxHp}\nenemyHp = {enemyHp}");
    }

    public void UpdateEnemyHp() //매 턴마다 실행
    {
        Debug.Log("UpdateEnemyHp() 실행");
        enemyHp = 0; //합산을 위해 먼저 0으로 초기화

        for (int i = 0; i < parts.Count; i++)
        {
            if(partComponents[i].partHp <= 0) //isDestroyed true
            {
                isDestroyed[i] = true;
                transform.GetChild(i).gameObject.SetActive(false);
            }
            Debug.Log($"partComponents[${i}] = ${partComponents[i].partHp}");
            enemyHp += partComponents[i].partHp;
        }
        //if (enemyHp <= 0)
        //{
        //    battleManager.state = BattleManager.State.WIN;
        //}
        enemyHpBar.value = (float)enemyHp / enemyMaxHp;
        Debug.Log($"enemyHpBar.value: enemyMaxHp = ${enemyMaxHp}\nenemyHp = ${enemyHp}");
    }


    public string ReplacePartText(string _part)
    {
        string part;

        if (_part == "Head")
        {
            part = "머리";
        }
        else if (_part == "Body")
        {
            part = "몸통";
        }
        else if (_part == "LArm")
        {
            part = "왼팔";
        }
        else if (_part == "RArm")
        {
            part = "오른팔";
        }
        else if (_part == "LLeg")
        {
            part = "왼다리";
        }
        else if (_part == "RLeg")
        {
            part = "오른다리";
        }
        else
        {
            part = _part;
        }
        return part;
    }


    public void EnemyTurnStart()
    {
        Debug.Log("EnemyTurnStart()");

        battleManager.partText.text = "";
        battleManager.hpBoxes.gameObject.SetActive(false);
        //battleManager.isEnemyTurnStarted = true;

        //다른 Enemy 구현 시 if문으로 this.name 검사해서 Add문들 돌리기
        List<Action> skills = new List<Action>();
        AddSkill(skills, "Head", isDestroyed[battleManager.FindListIndex(parts, "Head")], 0.2f, Aphrodite_Charm);
        AddSkill(skills, "Body", isDestroyed[battleManager.FindListIndex(parts, "Body")], 0.2f, Aphrodite_Dance);


        if (skills.Count > 0)
        {
            int index = Random.Range(0, skills.Count);
            skills[index]();
        }
        else if (!isDestroyed[battleManager.FindListIndex(parts, "LArm")])
        {
            Aphrodite_Throw();
        }
        Invoke("EnemyTurnEnd", 2);
    }    

    private void AddSkill(List<Action> skills, string partName, bool isDestroyed, float skillprobability, Action skill)
    {
        if (!isDestroyed && Random.value <= skillprobability)
        {
            Debug.Log($"{partName} 조건 만족: {skill.Method.Name} 스킬 추가");
            skills.Add(skill);
        }
    }


    private void Aphrodite_Charm()
    {
        Debug.Log("Aphrodite_Charm()");
        battleManager.contentText.text = "조각상이 매혹적인 눈빛을 보내 당신을 완전히 매료시킵니다.";
        player.isCharmed = true;
    }

    private void Aphrodite_Dance()
    {
        Debug.Log("Aphrodite_Dance()");
        battleManager.contentText.text = "조각상이 황홀한 춤을 춰 당신을 크게 매료시킵니다.\n방어력이 감소합니다.";
    }

    private void Aphrodite_Throw()
    {
        Debug.Log("Aphrodite_Throw()");
        battleManager.contentText.text = "조각상이 황금 사과를 던져 당신을 공격합니다.";

        player.playerHp -= 15;
        player.UpdatePlayerHp();
    }

    private void EnemyTurnEnd()
    {
        Debug.Log("EnemyTurnEnd()");
        battleManager.contentText.text = "";
        //battleManager.ClearContentText();
        battleManager.isEnemyTurnStarted = false;

        //여기 로직 다시 보기
        //isCharmed 정리 좀
        if (player.playerHp > 0) //Player 생존
        {
            if (this.name == "Aphrodite" && isDestroyed[battleManager.FindListIndex(parts, "Body")]) //아프로디테 && 몸통 파괴
            {
                battleManager.state = BattleManager.State.WIN;
            }
            else if (player.isCharmed)
            {
                player.isCharmed = false;

                if (this.name == "Aphrodite" && isDestroyed[battleManager.FindListIndex(parts, "Body")]) //아프로디테 && 몸통 파괴
                {
                    battleManager.state = BattleManager.State.WIN;
                }
                else //몸통 파괴X
                {
                    EnemyTurnStart(); //EnemyTurn 재시작
                }
            }
            else
            {
                battleManager.state = BattleManager.State.PLAYERTURN_START;
            }
            
        }
        else //Player 사망
        {            
            battleManager.state = BattleManager.State.LOSE;
        }
    }
}
