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

    public string enemyName;
    public string mainPart;

    public bool firstEnemyTurn1;
    public bool firstEnemyTurn2;

    public float increaseAttackPower;

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
    public void StartSetEnemy() // 최초 전투 진입 시에만 실행
    {
        Debug.Log("StartSetEnemy() 실행");

        // 초기화
        enemyName = this.name;
        parts.Clear();
        partComponents.Clear();
        isDestroyed.Clear();
        enemyMaxHp = 0;
        firstEnemyTurn1 = true;
        firstEnemyTurn2 = true;
        increaseAttackPower = 1.0f;

        // `Part` 컴포넌트 가져오기
        List<Part> tempParts = new List<Part>();
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            tempParts.Add(transform.GetChild(i).GetComponent<Part>());
        }

        // `partSort` 기준으로 정렬
        tempParts.Sort((a, b) => a.partSort.CompareTo(b.partSort));

        foreach (Part part in tempParts)
        {
            parts.Add(part.gameObject.name); // List parts에 Object 이름 추가
            Debug.Log($"parts.Add({part.gameObject.name})");

            partComponents.Add(part);
            part.SetPartHp(); // partHp 초기화

            enemyMaxHp += part.partMaxHp; // partMaxHp 합산
            isDestroyed.Add(false); // parts 길이만큼 isDestroyed false로 초기화
        }

        enemyHp = enemyMaxHp;
        Debug.Log($"enemyMaxHp = {enemyMaxHp}\nenemyHp = {enemyHp}");

        //공략 부위 설정
        if (enemyName == "Aphrodite")
        {
            mainPart = "Body";
        }
        else if (enemyName == "ReadingChild")
        {
            mainPart = "RLeg";
        }
        else if (enemyName == "Melpomene")
        {
            mainPart = "LArm";
        }
        else
        {
            mainPart = null;
        }
    }

    public void UpdateEnemyHp() // 매 턴마다 실행
    {
        Debug.Log("UpdateEnemyHp() 실행");
        enemyHp = 0; // 합산을 위해 먼저 0으로 초기화

        for (int i = 0; i < partComponents.Count; i++)
        {
            if (partComponents[i].partHp <= 0) // isDestroyed true
            {
                isDestroyed[i] = true;
                partComponents[i].gameObject.SetActive(false); // 정렬된 순서대로 비활성화
            }
            Debug.Log($"partComponents[{i}] = {partComponents[i].partHp}");
            enemyHp += partComponents[i].partHp;
        }

        enemyHpBar.value = (float)enemyHp / enemyMaxHp;
        Debug.Log($"enemyHpBar.value: enemyMaxHp = {enemyMaxHp}\nenemyHp = {enemyHp}");
    }


    public string ReplacePartText(string _part)
    {
        string part;

        if (_part == "Head")
        {
            part = "머리";
        }
        else if (_part == "Mask")
        {
            part = "가면";
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

        battleManager.contentText.text = "";
        battleManager.partText.text = "";
        battleManager.hpBoxes.gameObject.SetActive(false);

        if (enemyName == "Aphrodite") //아프로디테
        {
            List<Action> Aphrodite_skills = new List<Action>();
            AddSkill(Aphrodite_skills, "Head", isDestroyed[battleManager.FindListIndex(parts, "Head")], 0.2f, Aphrodite_Charm);
            AddSkill(Aphrodite_skills, "Body", isDestroyed[battleManager.FindListIndex(parts, "Body")], 0.2f, Aphrodite_Dance);

            if (Aphrodite_skills.Count > 0)
            {
                int index = Random.Range(0, Aphrodite_skills.Count);
                Aphrodite_skills[index]();
            }
            else if (!isDestroyed[battleManager.FindListIndex(parts, "LArm")])
            {
                Aphrodite_Throw();
            }
            Invoke("EnemyTurnEnd", 2);
        }
        else if (enemyName == "ReadingChild") //책 읽는 아이
        {
            if (isDestroyed[battleManager.FindListIndex(parts, "RArm")])
            {
                if (isDestroyed[battleManager.FindListIndex(parts, "LArm")])
                {
                    if (isDestroyed[battleManager.FindListIndex(parts, "Head")])
                    {
                        ReadingChild_Kick();
                    }
                    else
                    {
                        ReadingChild_Stroyteller();
                    }
                }
                else
                {
                    ReadingChild_BookShelf(15);
                }
            }
            else
            {
                ReadingChild_BookShelf(20);
            }

            //혼란 시스템 추가 예정
            Invoke("EnemyTurnEnd", 2);
        }
        else if (enemyName == "Melpomene") //멜포메네
        {
            List<Action> Melpomene_skills = new List<Action>();

            if (firstEnemyTurn1 && Random.value < 0.4f)
            {
                Melpomene_Narrative(15);
                firstEnemyTurn1 = false;
            }

            if (isDestroyed[battleManager.FindListIndex(parts, "Mask")] && isDestroyed[battleManager.FindListIndex(parts, "Head")] && firstEnemyTurn2 && Random.value < 0.4f)
            {
                Melpomene_Narrative(10);
                firstEnemyTurn2 = false;
            }

            AddSkill(Melpomene_skills, "Mask", isDestroyed[battleManager.FindListIndex(parts, "Mask")], 1.0f, Melpomene_Shout);
            AddSkill(Melpomene_skills, "Mask", isDestroyed[battleManager.FindListIndex(parts, "RArm")], 1.0f, Melpomene_Slap);

            if(Melpomene_skills.Count >0)
            {
                int index = Random.Range(0, Melpomene_skills.Count);
                Melpomene_skills[index]();
            }
            else
            {
                Melpomene_Bat();
            }
            Invoke("EnemyTurnEnd", 2);
        }
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
        StartCoroutine(battleManager.ContentTextWriter(" 조각상이 매혹적인 눈빛을 보내 당신을 완전히 매료시킵니다."));

        player.isCharmed = true;
    }
    private void Aphrodite_Dance()
    {
        Debug.Log("Aphrodite_Dance()");
        StartCoroutine(battleManager.ContentTextWriter(" 조각상이 황홀한 춤을 춰 당신을 크게 매료시킵니다.\n방어력이 감소합니다."));

        if (increaseAttackPower != 1.2f)
        {
            increaseAttackPower = 1.2f;
        }
    }
    private void Aphrodite_Throw()
    {
        Debug.Log("Aphrodite_Throw()");

        StartCoroutine(battleManager.ContentTextWriter(" 조각상이 황금 사과를 던져 당신을 공격합니다."));

        battleManager.battleAudioSource.Stop();
        battleManager.battleAudioSource.clip = battleManager.enemyAttackSFX;
        battleManager.battleAudioSource.time = 0;
        battleManager.battleAudioSource.Play();

        battleManager.Damage("player", 15);
    }

    private void ReadingChild_Stroyteller()
    {
        Debug.Log("ReadingChild_Stroyteller()");
        StartCoroutine(battleManager.ContentTextWriter(" 타고난 이야기꾼인 조각상은 흥미로운 이야기를 들려줍니다.\n당신은 환상에 휘말립니다."));

        player.isConfused = true;
    }
    private void ReadingChild_BookShelf(int _damage)
    {
        Debug.Log($"ReadingChild_BookShelf({_damage})");
        StartCoroutine(battleManager.ContentTextWriter(" 조각상이 책에서 페이지를 뽑아 날카로운 종이의 칼날을 휘두릅니다."));

        battleManager.Damage("player", _damage); //LArm, RArm 같은 스킬, 데미지 차이
    }
    private void ReadingChild_Kick()
    {
        Debug.Log("ReadingChild_Kick()");
        StartCoroutine(battleManager.ContentTextWriter(" 아무것도 남지 않은 조각상이 당신을 힘껏 걷어찹니다."));

        battleManager.Damage("player", 20);
    }

    private void Melpomene_Shout()
    {
        Debug.Log("Melpomene_Shout()");
        StartCoroutine(battleManager.ContentTextWriter(" 조각상이 비극을 외쳐, 그 울림이 당신에게 강력한 정신적 충격을 줍니다.\n방어력이 감소합니다."));

        battleManager.Damage("player", 30);
    }

    private void Melpomene_Narrative(int _damage)
    {
        Debug.Log("Melpomene_Narrative()");
        StartCoroutine(battleManager.ContentTextWriter(" 조각상이 당신의 비극적인 운명을 노래합니다.\n운명의 저주가 당신을 천천히 갉아먹습니다."));

        battleManager.Damage("player", _damage);
    }
    public void Melpomene_Redemption()
    {
        Debug.Log("Melpomene_Redemption()");
        StartCoroutine(battleManager.ContentTextWriter(" 조각상이 알 수 없는 힘으로 당신을 구속합니다.")); //첫 글자 누락, 일단 공백으로 임시 해결

        battleManager.Damage("player", 5);
    }

    private void Melpomene_Bat() //Player가 Run 선택 시 발동
    {
        Debug.Log("Melpomene_Bat()");
        StartCoroutine(battleManager.ContentTextWriter(" 조각상이 손에 든 커다란 방망이를 휘두릅니다."));

        battleManager.Damage("player", 15);
    }

    private void Melpomene_Slap()
    {
        Debug.Log("Melpomene_Slap()");
        StartCoroutine(battleManager.ContentTextWriter(" 조각상이 당신의 뺨을 후려칩니다.\n그다지 타격은 없으나 비극적인 기분이 느껴집니다."));

        battleManager.Damage("player", 5);
    }

    private void EnemyTurnEnd()
    {
        Debug.Log("EnemyTurnEnd()");
        battleManager.contentText.text = "";
        battleManager.isEnemyTurnStarted = false;
        battleManager.isPlayerRun = false;

        //여기 로직 다시 보기
        if (player.playerHp > 0) //Player 생존
        {
            if (isDestroyed[battleManager.FindListIndex(parts, mainPart)]) //공략 부위 파괴 시
            {
                battleManager.state = BattleManager.State.WIN;
            }  
            else if (player.isCharmed) //매혹
            {
                player.isCharmed = false;

                EnemyTurnStart(); //EnemyTurn 재시작
            }
            else
            {
                Debug.Log("Change State to PLAYERTURN_START");
                battleManager.state = BattleManager.State.PLAYERTURN_START;
                Debug.Log($"CurrentScene is {battleManager.state}");
            }
        }
        else //Player 사망
        {
            battleManager.state = BattleManager.State.LOSE;
        }
    }
}
