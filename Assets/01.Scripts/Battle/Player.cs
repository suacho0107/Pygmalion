using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    BattleManager battleManager;
    Enemy enemy;
    Part part;

    public int playerHp;
    private int playerMaxHp = 100; //���� ����
    //private int playerMaxHp = 30; //���� ����

    private int attackDamage = 1; //�� 1ĭ�� �� ����

    public Slider playerHpBar;

    public bool isCharmed; //��Ȥ����

    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
        enemy = FindObjectOfType<Enemy>();
        //part = FindObjectOfType<Part>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerHp()
    {
        playerHp = playerMaxHp; // Hp �ʱ�ȭ; ������ ���� �� ����
    }

    public void UpdatePlayerHp() //�� �ϸ��� ����
    {
        playerHpBar.value = (float)playerHp / playerMaxHp;
    }

    public void PlayerTurnStart()
    {
        //battleManager.contentText.text = "� �ൿ�� �ұ�?";
        StartCoroutine(battleManager.ContentTextWriter("� �ൿ�� �ұ�?"));
        battleManager.buttons.gameObject.SetActive(true);
        battleManager.partText.gameObject.SetActive(false);
        battleManager.hpBoxes.gameObject.SetActive(false);
    }

    public void AttackButton()
    {
        battleManager.contentText.text = "���� ���� ����";
        battleManager.buttons.gameObject.SetActive(false);

        enemy.currentPart = enemy.parts[0]; //currentPart �ʱ�ȭ
        battleManager.partText.text = enemy.ReplacePartText(enemy.currentPart);
        battleManager.partText.gameObject.SetActive(true);
        battleManager.UpdateHpBoxes();

        battleManager.state = BattleManager.State.PLAYERTURN_ATTACK;
    }

    public void SelectAttackPart()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) //������ ����Ű
        {
            //Debug.Log("�� �Է�");
            int i = FindListIndex(enemy.parts, enemy.currentPart);
            Debug.Log($"FindListIndex(parts, currentPart): {i}");

            if (++i < enemy.parts.Count) //++i�� �����ϸ�
            {
                enemy.currentPart = enemy.parts[i];
                battleManager.partText.text = enemy.ReplacePartText(enemy.currentPart); //�̰� ������ ���� �ϳ�? ��� ����?

                //isDestroyed ���� �Ǻ�
                if (enemy.isDestroyed[i])
                {
                    battleManager.partText.color = Color.grey;
                }
                else
                {
                    battleManager.partText.color = Color.white;
                }
                battleManager.UpdateHpBoxes();
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //Debug.Log("�� �Է�");
            int i = FindListIndex(enemy.parts, enemy.currentPart);
            Debug.Log($"FindListIndex(parts, currentPart): {i}");

            if (--i >= 0) //++i�� �����ϸ�
            {
                enemy.currentPart = enemy.parts[i];
                battleManager.partText.text = enemy.ReplacePartText(enemy.currentPart); //�̰� ������ ���� �ϳ�? ��� ����?

                //isDestroyed ���� �Ǻ�
                if (enemy.isDestroyed[i])
                {

                    battleManager.partText.color = Color.grey;
                }
                else
                {
                    battleManager.partText.color = Color.white;
                }
                battleManager.UpdateHpBoxes();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            int i = FindListIndex(enemy.parts, enemy.currentPart);

            if(enemy.isDestroyed[i]) //�ı��Ǿ�������
            {
                //�ƹ� �ϵ� �� �Ͼ��... �ʱ�� ���ư��ų� SelectPart�� ���ư���?
                //��� �����Ÿ� ��򰡷� ���ư��� �ؾ߰ڴ�.
                Debug.Log($"${enemy.currentPart} isDestroyed[${i}] == true");
            }
            else
            {
                PlayerAttack(enemy.partComponents[i]);
            }
        }
    }

    //string List�� ��� �̸��� �Ű������� List�� Index ��ȯ
    private int FindListIndex(List<string> _list, string _element)
    {
        if(_list == null) //����ó��
        {
            return -1;
        }

        for (int i = 0; i < _list.Count; i++)
        {
            if (_list[i] == _element)
            {
                return i;
            }
        }
        //�� ���� ���Ҵµ��� �� ã����
        return -1;
    }

    public void InventoryButton()
    {
        //battleManager.state = BattleManager.State.PLAYERTURN_INVENTORY;

        //��������
    }

    public void RunButton()
    {
        battleManager.state = BattleManager.State.PLAYERTURN_RUN;
    }
    
    public void Run()
    {
        //���⼭ bool�� ���� ���� �����ؼ� ������ �� Setting �����ϱ�?
        battleManager.contentText.text = "";
        battleManager.buttons.SetActive(false);

        battleManager.PlaySFX(battleManager.playerRunSFX);
        ////�̰͸� �Ҹ� �� ���� �׳� ���� �����ϱ�
        //battleManager.battleAudioSource.Stop();
        //battleManager.battleAudioSource.clip = battleManager.playerRunSFX;
        //battleManager.battleAudioSource.time = 0;
        //battleManager.battleAudioSource.Play();

        battleManager.Invoke("ExitBattleScene", 3);
        PlayerPrefs.SetInt("PlayerRun", 1);
        PlayerPrefs.Save();
    }


    public void PlayerAttack(Part part)
    {
        Debug.Log("PlayerAttack(enemy, part) ����");

        battleManager.contentText.text = "";

        //battleManager.PlaySFX(battleManager.playerAttackSFX);
        battleManager.battleAudioSource.Stop();
        battleManager.battleAudioSource.clip = battleManager.playerAttackSFX;
        battleManager.battleAudioSource.time = 0;
        battleManager.battleAudioSource.Play();


        part.partHp -= attackDamage; //attackDamage��ŭ partHp ����
        enemy.UpdateEnemyHp();

        //PlayerTurnEnd();
        Invoke("PlayerTurnEnd", 1);
    }

    void PlayerTurnEnd()
    {
        Debug.Log("PlayerTurnEnd() ����");
        if (enemy.name == "Aphrodite" && enemy.isDestroyed[battleManager.FindListIndex(enemy.parts, "Body")]) //�����ε��� && ���� �ı�
        {
            battleManager.state = BattleManager.State.WIN;
        }
        else
        {
            battleManager.state = BattleManager.State.ENEMYTURN;
        }

        battleManager.isPlayerTurnStarted = false;
    }
}