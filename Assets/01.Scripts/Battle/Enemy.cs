using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    #region References
    BattleUI battleUI;
    BattleManager battleManager;
    Player player;
    #endregion

    #region Variables
    public int enemyHp;
    private int enemyMaxHp;

    public Slider enemyHpBar;

    public List<string> parts = new List<string>(); //partComponents도 포함한 Dictionary로 변환하기?
    public List<Part> partComponents = new List<Part>(); //UpdatehpBox 등에서 사용
    public string currentPart;
    public List<bool> isDestroyed = new List<bool>();

    public string enemyName;
    public string mainPart;

    public bool isMasked; //Melpomene: Mask 파괴 시 Head 공격 가능

    // Melpomene_Narrative() 발동 조건으로 사용
    public bool firstEnemyTurn1;
    public bool firstEnemyTurn2;

    public float ConfusionRate; //Melpomene: Confusion 발동 확률
    public bool isConfusion1;
    public bool isConfusion2;

    public float increaseAttackPower;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
        battleUI = FindObjectOfType<BattleUI>();
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

    #endregion


    public void SetEnemy() // 최초 전투 진입 시에만 실행
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
        ConfusionRate = 0.0f;
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
        battleUI.contentText.text = "";

        //아프로디테
        if (enemyName == "Aphrodite")
        {
            Aphrodite_Skill();

            Invoke("EnemyTurnEnd", 2);
        }

        //책 읽는 아이
        else if (enemyName == "ReadingChild")
        {
            ReadingChild_Skill();

            Invoke("EnemyTurnEnd", 2);
        }

        //멜포메네
        else if (enemyName == "Melpomene")
        {
            Melpomene_Skill();

            Invoke("EnemyTurnEnd", 2);
        }
    }

    #region Enemy_Skill Methods
    private void Aphrodite_Skill()
    {
        List<Action> Aphrodite_skills = new List<Action>();
        AddSkill(Aphrodite_skills, "Head", isDestroyed[battleUI.FindListIndex(parts, "Head")], 0.2f, Aphrodite_Charm);
        AddSkill(Aphrodite_skills, "Body", isDestroyed[battleUI.FindListIndex(parts, "Body")], 0.2f, Aphrodite_Dance);

        if (Aphrodite_skills.Count > 0)
        {
            int index = Random.Range(0, Aphrodite_skills.Count);
            Aphrodite_skills[index]();
        }
        else if (!isDestroyed[battleUI.FindListIndex(parts, "LArm")])
        {
            Aphrodite_Throw();
        }
    }

    private void ReadingChild_Skill()
    {
        List<Action> ReadingChild_skills = new List<Action>();

        if (!isDestroyed[battleUI.FindListIndex(parts, "RArm")])
        {
            ReadingChild_BookShelf(20);
        }
        else if (!isDestroyed[battleUI.FindListIndex(parts, "LArm")])
        {
            ReadingChild_BookShelf(15);
        }
        else
        {
            AddSkill(ReadingChild_skills, "Head", isDestroyed[battleUI.FindListIndex(parts, "Head")], 0.5f, ReadingChild_Stroyteller);
            AddSkill(ReadingChild_skills, "RLeg", isDestroyed[battleUI.FindListIndex(parts, "RLeg")], 0.5f, ReadingChild_Kick);

            if (ReadingChild_skills.Count > 0)
            {
                int index = Random.Range(0, ReadingChild_skills.Count);
                ReadingChild_skills[index]();
            }
        }
    }

    private void Melpomene_Skill()
    {
        List<Action> Melpomene_skills = new List<Action>();

        if (isConfusion1 || isConfusion2)
        {
            ConfusionRate += 0.05f;
        }
        else if (isConfusion1 && isConfusion2)
        {
            ConfusionRate += 0.1f;
        }

        if (firstEnemyTurn1 && Random.value < 0.4f)
        {
            Melpomene_Narrative(15);
            firstEnemyTurn1 = false;
            isConfusion1 = true;
        }

        if (isDestroyed[battleUI.FindListIndex(parts, "Mask")] && isDestroyed[battleUI.FindListIndex(parts, "Head")] && firstEnemyTurn2 && Random.value < 0.4f)
        {
            Melpomene_Narrative(10);
            firstEnemyTurn2 = false;
            isConfusion2 = true;
        }

        AddSkill(Melpomene_skills, "Mask", isDestroyed[battleUI.FindListIndex(parts, "Mask")], 1.0f, Melpomene_Shout); //partName 삭제 예정
        AddSkill(Melpomene_skills, "RArm", isDestroyed[battleUI.FindListIndex(parts, "RArm")], 1.0f, Melpomene_Slap); //partName 삭제 예정

        if (Melpomene_skills.Count > 0)
        {
            int index = Random.Range(0, Melpomene_skills.Count);
            Melpomene_skills[index]();
        }
        else if (!isDestroyed[battleUI.FindListIndex(parts, "RArm")])
        {
            Melpomene_Bat();
        }

        if (Random.value < ConfusionRate)
        {
            Melpomene_Confusion();
        }
        Debug.Log($"ConfusionRate: {ConfusionRate}");
    }

    private void AddSkill(List<Action> skills, string partName, bool isDestroyed, float skillprobability, Action skill) //partName 삭제 예정
    {
        if (!isDestroyed && Random.value <= skillprobability)
        {
            Debug.Log($"{partName} 조건 만족: {skill.Method.Name} 스킬 추가");
            skills.Add(skill);
        }
    }
    #endregion

    #region Aphrodite Skills
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
    #endregion

    #region ReadingChild_Skills
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
    #endregion

    #region Melpomene_Skills
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
        ConfusionRate += 0.05f;
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

    private void Melpomene_Confusion()
    {
        Debug.Log("This is not a Skill, just Confusion");
        StartCoroutine(battleManager.ContentTextWriter("혼란 발동됨.")); //대사가 없어서 일단 임시 대사
    }
    #endregion

    private void EnemyTurnEnd()
    {
        Debug.Log("EnemyTurnEnd()");
        battleUI.contentText.text = "";
        battleManager.isEnemyTurnStarted = false;
        battleManager.isPlayerRunning = false;

        //여기 로직 다시 보기
        if (player.playerHp > 0) //Player 생존
        {
            if (isDestroyed[battleUI.FindListIndex(parts, mainPart)]) //공략 부위 파괴 시
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
