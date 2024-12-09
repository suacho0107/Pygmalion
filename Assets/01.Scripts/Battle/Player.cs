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
    private int playerMaxHp = 100; //임의 설정
    //private int playerMaxHp = 30; //임의 설정

    private int attackDamage = 1; //피 1칸씩 깔 거임

    public Slider playerHpBar;

    public bool isCharmed; //매혹당함

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
        playerHp = playerMaxHp; // Hp 초기화; 재진입 구현 시 수정
    }

    public void UpdatePlayerHp() //매 턴마다 실행
    {
        playerHpBar.value = (float)playerHp / playerMaxHp;
    }

    public void PlayerTurnStart()
    {
        //battleManager.contentText.text = "어떤 행동을 할까?";
        StartCoroutine(battleManager.ContentTextWriter("어떤 행동을 할까?"));
        battleManager.buttons.gameObject.SetActive(true);
        battleManager.partText.gameObject.SetActive(false);
        battleManager.hpBoxes.gameObject.SetActive(false);
    }

    public void AttackButton()
    {
        battleManager.contentText.text = "공격 부위 선택";
        battleManager.buttons.gameObject.SetActive(false);

        enemy.currentPart = enemy.parts[0]; //currentPart 초기화
        battleManager.partText.text = enemy.ReplacePartText(enemy.currentPart);
        battleManager.partText.gameObject.SetActive(true);
        battleManager.UpdateHpBoxes();

        battleManager.state = BattleManager.State.PLAYERTURN_ATTACK;
    }

    public void SelectAttackPart()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) //오른쪽 방향키
        {
            //Debug.Log("→ 입력");
            int i = FindListIndex(enemy.parts, enemy.currentPart);
            Debug.Log($"FindListIndex(parts, currentPart): {i}");

            if (++i < enemy.parts.Count) //++i가 가능하면
            {
                enemy.currentPart = enemy.parts[i];
                battleManager.partText.text = enemy.ReplacePartText(enemy.currentPart); //이거 밑으로 빼야 하나? 상관 없나?

                //isDestroyed 여부 판별
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
            //Debug.Log("← 입력");
            int i = FindListIndex(enemy.parts, enemy.currentPart);
            Debug.Log($"FindListIndex(parts, currentPart): {i}");

            if (--i >= 0) //++i가 가능하면
            {
                enemy.currentPart = enemy.parts[i];
                battleManager.partText.text = enemy.ReplacePartText(enemy.currentPart); //이거 밑으로 빼야 하나? 상관 없나?

                //isDestroyed 여부 판별
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

            if(enemy.isDestroyed[i]) //파괴되어있으면
            {
                //아무 일도 안 일어나고... 초기로 돌아가거나 SelectPart로 돌아가게?
                //대사 넣을거면 어딘가로 돌아가긴 해야겠다.
                Debug.Log($"${enemy.currentPart} isDestroyed[${i}] == true");
            }
            else
            {
                PlayerAttack(enemy.partComponents[i]);
            }
        }
    }

    //string List의 요소 이름을 매개변수로 List의 Index 반환
    private int FindListIndex(List<string> _list, string _element)
    {
        if(_list == null) //예외처리
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
        //한 바퀴 돌았는데도 못 찾으면
        return -1;
    }

    public void InventoryButton()
    {
        //battleManager.state = BattleManager.State.PLAYERTURN_INVENTORY;

        //구현예정
    }

    public void RunButton()
    {
        battleManager.state = BattleManager.State.PLAYERTURN_RUN;
    }
    
    public void Run()
    {
        //여기서 bool로 도망 여부 저장해서 재진입 시 Setting 변경하기?
        battleManager.contentText.text = "";
        battleManager.buttons.SetActive(false);

        battleManager.PlaySFX(battleManager.playerRunSFX);
        ////이것만 소리 안 나서 그냥 냅다 실행하기
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
        Debug.Log("PlayerAttack(enemy, part) 실행");

        battleManager.contentText.text = "";

        //battleManager.PlaySFX(battleManager.playerAttackSFX);
        battleManager.battleAudioSource.Stop();
        battleManager.battleAudioSource.clip = battleManager.playerAttackSFX;
        battleManager.battleAudioSource.time = 0;
        battleManager.battleAudioSource.Play();


        part.partHp -= attackDamage; //attackDamage만큼 partHp 차감
        enemy.UpdateEnemyHp();

        //PlayerTurnEnd();
        Invoke("PlayerTurnEnd", 1);
    }

    void PlayerTurnEnd()
    {
        Debug.Log("PlayerTurnEnd() 실행");
        if (enemy.name == "Aphrodite" && enemy.isDestroyed[battleManager.FindListIndex(enemy.parts, "Body")]) //아프로디테 && 몸통 파괴
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