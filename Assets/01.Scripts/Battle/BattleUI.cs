using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    BattleManager battleManager;
    Player player;
    Enemy enemy;

    public Text contentText;

    public GameObject dialogueButtons;
    private List<GameObject> dialogueButtonList = new List<GameObject>();
    public GameObject partButtons;
    private List<GameObject> partButtonList = new List<GameObject>();
    public Sprite ButtonDefault;
    public Sprite ButtonHighlighted;

    public GameObject hpBoxes;
    public Sprite hpBoxEmpty;
    public Sprite hpBoxFull;

    public GameObject blackBoard;

    private int currentDialogueButtonIndex;
    private int currentPartPageIndex;
    public int currentPartButtonIndex;

    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
        player = FindObjectOfType<Player>();
        enemy = FindObjectOfType<Enemy>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(dialogueButtons.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                DialogueButtonInputHandler();
            }
        }
        else if (battleManager.isPartSelecting)
        {
            if (Input.anyKeyDown) //키 입력 시에만
            {
                PartButtonInputHandler();
            }
        }
    }

    private void DialogueButtonInputHandler() //버튼 3개 기준으로 작성
    {
        //상하 이동
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentDialogueButtonIndex == 2)
            {
                currentDialogueButtonIndex -= 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentDialogueButtonIndex == 0)
            {
                currentDialogueButtonIndex += 2;
            }
        }

        //좌우 이동
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentDialogueButtonIndex % 2 == 1)
            {
                currentDialogueButtonIndex--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentDialogueButtonIndex == 0)
            {
                currentDialogueButtonIndex++;
            }
        }

        //선택
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            string selectedButtonText = dialogueButtonList[currentDialogueButtonIndex].GetComponentInChildren<Text>().text;

            if (selectedButtonText == "공격한다")
            {
                player.AttackButton();
            }
            else if (selectedButtonText == "소지품을 확인한디")
            {
                player.InventoryButton();
                //InventoryButton() 함수 내용 작성 필요
            }
            else if (selectedButtonText == "도망친다")
            {
                player.RunButton();
            }

            dialogueButtons.SetActive(false);
        }

        HighlightDialogueButton();
    }

    private void PartButtonInputHandler()
    {
        //상하 이동
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentPartButtonIndex == 2 || currentPartButtonIndex == 3)
            {
                currentPartButtonIndex -= 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentPartButtonIndex == 0 || currentPartButtonIndex == 1)
            {
                if (isThereButton(currentPartPageIndex, currentPartButtonIndex, 2))
                {
                    currentPartButtonIndex += 2;
                }
            }
        }

        //좌우 이동
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentPartButtonIndex % 2 == 0)
            {
                if (currentPartPageIndex > 0)
                {
                    currentPartPageIndex--;
                    currentPartButtonIndex++;

                    UpdatePartButtons();
                }
            }
            else
            {
                currentPartButtonIndex--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentPartButtonIndex % 2 == 0)
            {
                if (isThereButton(currentPartPageIndex, currentPartButtonIndex, 2))
                {
                    currentPartButtonIndex++;
                }
            }
            else if (currentPartButtonIndex % 2 == 1)
            {
                if ((currentPartPageIndex + 1) * 4 < enemy.parts.Count)
                {
                    if (currentPartButtonIndex == 1)
                    {
                        currentPartPageIndex++;
                        currentPartButtonIndex--;

                        UpdatePartButtons();
                    }
                    else if (currentPartButtonIndex == 3)
                    {
                        currentPartPageIndex++;

                        if (isThereButton(currentPartPageIndex, currentPartButtonIndex, -1))
                        {
                            currentPartButtonIndex--;
                        }
                        else
                        {
                            currentPartButtonIndex = 0;
                        }

                        UpdatePartButtons();
                    }
                }
            }
        }

        else if (Input.GetKeyDown(KeyCode.Space))
        {
            int selectedPartIndex = currentPartPageIndex * 4 + currentPartButtonIndex;

            if (selectedPartIndex < enemy.partComponents.Count)
            {
                if (enemy.isDestroyed[selectedPartIndex])
                {
                    Debug.Log("This part is already destroyed"); //Delete
                }

                player.PlayerAttack(enemy.partComponents[selectedPartIndex]);
                battleManager.isPartSelecting = false;

                //초기화
                currentPartPageIndex = 0;
                currentPartPageIndex = 0;
            }
        }

        Debug.Log($"현재 페이지: {currentPartPageIndex}, 선택된 버튼 인덱스: {currentPartButtonIndex}"); //delete

        HighlightPartButton();
    }

    private bool isThereButton(int pageIndex, int buttonIndex, int increraseButtonIndex)
    {
        int targetButtonIndex = buttonIndex + increraseButtonIndex;

        if (targetButtonIndex > 3)
        {
            targetButtonIndex -= 4;
        }

        int lastButtonIndex = enemy.parts.Count - pageIndex * 4;

        return targetButtonIndex < lastButtonIndex;        
    }


    public void SetDialogueButtons()
    {
        dialogueButtonList.Clear(); //중복추가 방지

        for (int i = 0; i < dialogueButtons.transform.childCount; i++)
        {
            GameObject dialogueButton = dialogueButtons.transform.GetChild(i).gameObject;

            dialogueButtonList.Add(dialogueButton);
        }

        currentDialogueButtonIndex = 0;
        HighlightDialogueButton();
    }

    private void HighlightDialogueButton()
    {
        for (int i = 0; i < dialogueButtonList.Count; i++)
        {
            Image image = dialogueButtonList[i].GetComponent<Image>();
            image.sprite = (i == currentDialogueButtonIndex) ? ButtonHighlighted : ButtonDefault;
        }
    }

    public void SetPartButtons()
    {
        partButtonList.Clear(); //중복추가 방지

        for (int i = 0; i < partButtons.transform.childCount; i++)
        {
            GameObject partButton = partButtons.transform.GetChild(i).gameObject;

            partButtonList.Add(partButton);
        }

        //초기화
        currentPartPageIndex = 0;
        currentPartButtonIndex = 0;
    }

    public void UpdatePartButtons()
    {
        int startIndex = currentPartPageIndex * 4;

        for (int i = 0; i < partButtonList.Count; i++)
        {
            int partIndex = startIndex + i;
            GameObject partButton = partButtonList[i];
            Debug.Log($"partButton 이름: {partButton}"); //Delete

            if (partIndex < enemy.parts.Count)
            {
                partButton.SetActive(true);

                Text partText = partButton.transform.Find("Text (Legacy)").GetComponent<Text>();
                Color partColor = enemy.partComponents[partIndex].partHp > 0 ? Color.white : Color.gray;
                partText.text = enemy.ReplacePartText(enemy.parts[partIndex]);
                partText.color = partColor;

                GameObject hpBoxes = partButton.transform.Find("HpBoxes").gameObject;
                UpdateHpBoxes(hpBoxes, enemy.partComponents[partIndex]);
            }
            else
            {
                partButton.SetActive(false);
            }
        }

        HighlightPartButton();
    }

    private void HighlightPartButton()
    {
        for (int i = 0; i < partButtonList.Count; i++)
        {
            GameObject partButton = partButtonList[i];

            if(!partButton.activeSelf)
            {
                continue;
            }

            int partIndex = currentPartPageIndex * 4 + i;

            if (partIndex < enemy.partComponents.Count)
            {
                bool isSelected = (i == currentPartButtonIndex) && partButton.activeSelf;
                partButton.GetComponent<Image>().sprite = isSelected ? ButtonHighlighted : ButtonDefault;
            }

        }
    }

    public void UpdateHpBoxes(GameObject hpBoxes, Part part)
    {
        for (int i = 0; i < hpBoxes.transform.childCount; i++)
        {
            GameObject hpBox = hpBoxes.transform.GetChild(i).gameObject;

            hpBox.SetActive(i < part.partMaxHp);
            hpBox.GetComponent<Image>().sprite = (i < part.partHp) ? hpBoxFull : hpBoxEmpty;
        }
    }

    public int FindListIndex(List<string> list, string element)
    {
        return list.IndexOf(element);
    }
}
