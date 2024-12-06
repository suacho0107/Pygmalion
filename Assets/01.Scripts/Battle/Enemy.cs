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

    public List<string> parts = new List<string>(); //partComponents�� ������ Dictionary�� ��ȯ�ϱ�?
    public List<Part> partComponents = new List<Part>(); //UpdatehpBox ��� ���
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

    //public void StartSetEnemy() //���� ���� ���� �ÿ��� ����
    //{
    //    //SetPart(), SetHp() ��ħ
    //    Debug.Log("StartSetEnemy() ����");
    //    // �ʱ�ȭ
    //    parts.Clear();
    //    partComponents.Clear();
    //    isDestroyed.Clear();
    //    enemyMaxHp = 0;

    //    for (int i = 0; i < transform.childCount - 1; i++)
    //    {
    //        parts.Add(transform.GetChild(i).gameObject.name); //List parts�� Object�� �̸� �߰�
    //        Debug.Log($"parts.Add(${parts[i]})");

    //        partComponents.Add(transform.GetChild(i).GetComponent<Part>());
    //        partComponents[i].SetPartHp(); //partHP �ʱ�ȭ

    //        enemyMaxHp += partComponents[i].partMaxHp; //partMaxHp �ջ�

    //        isDestroyed.Add(false); //parts ���̸�ŭ isDestroyed false�� �ʱ�ȭ
    //    }
    //    enemyHp = enemyMaxHp;
    //    Debug.Log($"enemyMaxHp = ${enemyMaxHp}\nenemyHp = ${enemyHp}");
    //}

    public void StartSetEnemy() // ���� ���� ���� �ÿ��� ����
    {
        Debug.Log("StartSetEnemy() ����");

        // �ʱ�ȭ
        parts.Clear();
        partComponents.Clear();
        isDestroyed.Clear();
        enemyMaxHp = 0;

        // ��� �ڽ��� Part ������Ʈ�� ����Ʈ�� �߰�
        List<Part> sortedParts = new List<Part>();
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Part partComponent = transform.GetChild(i).GetComponent<Part>();
            if (partComponent != null)
            {
                sortedParts.Add(partComponent);
            }
        }

        // partSort�� �������� ����
        sortedParts.Sort((a, b) => a.partSort.CompareTo(b.partSort));

        // ���ĵ� ������� parts, partComponents, isDestroyed ����Ʈ ä���
        foreach (Part part in sortedParts)
        {
            parts.Add(part.gameObject.name); // Object �̸� �߰�
            Debug.Log($"parts.Add({part.gameObject.name})");

            partComponents.Add(part); // Part ������Ʈ �߰�
            part.SetPartHp(); // partHP �ʱ�ȭ

            enemyMaxHp += part.partMaxHp; // partMaxHp �ջ�
            isDestroyed.Add(false); // isDestroyed �ʱ�ȭ
        }

        // Enemy ��ü HP �ʱ�ȭ
        enemyHp = enemyMaxHp;
        Debug.Log($"enemyMaxHp = {enemyMaxHp}\nenemyHp = {enemyHp}");
    }

    public void UpdateEnemyHp() //�� �ϸ��� ����
    {
        Debug.Log("UpdateEnemyHp() ����");
        enemyHp = 0; //�ջ��� ���� ���� 0���� �ʱ�ȭ

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
            part = "�Ӹ�";
        }
        else if (_part == "Body")
        {
            part = "����";
        }
        else if (_part == "LArm")
        {
            part = "����";
        }
        else if (_part == "RArm")
        {
            part = "������";
        }
        else if (_part == "LLeg")
        {
            part = "�޴ٸ�";
        }
        else if (_part == "RLeg")
        {
            part = "�����ٸ�";
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

        //�ٸ� Enemy ���� �� if������ this.name �˻��ؼ� Add���� ������
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
            Debug.Log($"{partName} ���� ����: {skill.Method.Name} ��ų �߰�");
            skills.Add(skill);
        }
    }


    private void Aphrodite_Charm()
    {
        Debug.Log("Aphrodite_Charm()");
        battleManager.contentText.text = "�������� ��Ȥ���� ������ ���� ����� ������ �ŷ��ŵ�ϴ�.";
        player.isCharmed = true;
    }

    private void Aphrodite_Dance()
    {
        Debug.Log("Aphrodite_Dance()");
        battleManager.contentText.text = "�������� ȲȦ�� ���� �� ����� ũ�� �ŷ��ŵ�ϴ�.\n������ �����մϴ�.";
    }

    private void Aphrodite_Throw()
    {
        Debug.Log("Aphrodite_Throw()");
        battleManager.contentText.text = "�������� Ȳ�� ����� ���� ����� �����մϴ�.";

        player.playerHp -= 15;
        player.UpdatePlayerHp();
    }

    private void EnemyTurnEnd()
    {
        Debug.Log("EnemyTurnEnd()");
        battleManager.contentText.text = "";
        //battleManager.ClearContentText();
        battleManager.isEnemyTurnStarted = false;

        //���� ���� �ٽ� ����
        //isCharmed ���� ��
        if (player.playerHp > 0) //Player ����
        {
            if (this.name == "Aphrodite" && isDestroyed[battleManager.FindListIndex(parts, "Body")]) //�����ε��� && ���� �ı�
            {
                battleManager.state = BattleManager.State.WIN;
            }
            else if (player.isCharmed)
            {
                player.isCharmed = false;

                if (this.name == "Aphrodite" && isDestroyed[battleManager.FindListIndex(parts, "Body")]) //�����ε��� && ���� �ı�
                {
                    battleManager.state = BattleManager.State.WIN;
                }
                else //���� �ı�X
                {
                    EnemyTurnStart(); //EnemyTurn �����
                }
            }
            else
            {
                battleManager.state = BattleManager.State.PLAYERTURN_START;
            }
            
        }
        else //Player ���
        {            
            battleManager.state = BattleManager.State.LOSE;
        }
    }
}
