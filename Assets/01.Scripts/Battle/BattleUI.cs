using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    #region References
    BattleManager battleManager;
    Player player;
    Enemy enemy;
    #endregion

    #region Variables
    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public Text contentText;

    public GameObject blackBoard;

    [Header("Dialogue UI")]
    public GameObject dialogueButtons;
    private List<GameObject> dialogueButtonList = new List<GameObject>();

    public bool isFadeInOut;

    [Header("PartSelect UI")]
    public GameObject partButtons;
    private List<GameObject> partButtonList = new List<GameObject>();

    public GameObject pageArrows;
    private Image prePageArrow;
    private Image nextPageArrow;

    [Header("Button Sprites")]
    public Sprite ButtonDefault;
    public Sprite ButtonHighlighted;

    [Header("HpBoxes & HpBox Sprites")]
    public GameObject hpBoxes;
    public Sprite hpBoxEmpty;
    public Sprite hpBoxFull;

    [Header("Dialogue & PartSelect Control")]
    private int currentDialogueButtonIndex;
    private int currentPartPageIndex;
    public int currentPartButtonIndex;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        battleManager = FindObjectOfType<BattleManager>();
        player = FindObjectOfType<Player>();
        enemy = FindObjectOfType<Enemy>();

        prePageArrow = pageArrows.transform.Find("Image_PrePageArrow").GetComponent<Image>();
        nextPageArrow = pageArrows.transform.Find("Image_NextPageArrow").GetComponent<Image>();
    }

    void Update()
    {
        if (dialogueButtons.activeSelf && !battleManager.isContentTextWriting)
        {
            if (Input.anyKeyDown)
            {
                DialogueButtonInputHandler();
            }
        }
        else if (battleManager.isStatePLAYERTURN_ATTACK_PartSelecting && !battleManager.isContentTextWriting)
        {
            if (Input.anyKeyDown) //키 입력 시에만
            {
                PartButtonInputHandler();
            }
        }
    }
    #endregion

    #region ButtonInputHandlers
    private void DialogueButtonInputHandler() //버튼 3개 기준으로 작성됨
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
                player.SelectAttack();
                dialogueButtons.SetActive(false);
            }
            else if (selectedButtonText == "소지품을 확인한디")
            {
                player.SelectInventory();
                //SelectInventory() 함수 내용 작성 필요
            }
            else if (selectedButtonText == "도망친다")
            {
                player.SelectRun();
                dialogueButtons.SetActive(false);
            }
            //dialogueButtons.SetActive(false); //Inventory 구현 이후에 if문 안에 빼고 이거 사용
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
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentPartButtonIndex % 2 == 0)
            {
                if (isThereButton(currentPartPageIndex, currentPartButtonIndex, 1))
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
                //Melpomene Mask-Head 우선순위
                if (enemy.enemyName == "Melpomene" && enemy.isMasked && enemy.parts[selectedPartIndex] == "Head")
                {
                    return;
                }

                if (enemy.isDestroyed[selectedPartIndex])
                {
                    Debug.Log("This part is already destroyed"); //Delete
                }
                else
                {
                    StartCoroutine(player.PlayerAttack(enemy.partComponents[selectedPartIndex]));

                    battleManager.isStatePLAYERTURN_ATTACK_PartSelecting = false;
                }

                //초기화
                currentPartPageIndex = 0;
                currentPartButtonIndex = 0;
                pageArrows.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            //UI초기화
            contentText.text = "";
            partButtons.SetActive(false);

            battleManager.isStatePLAYERTURN = false;
            battleManager.isStatePLAYERTURN_ATTACK = false;
            battleManager.isStatePLAYERTURN_ATTACK_PartSelecting = false;
            battleManager.state = BattleManager.State.PLAYERTURN_START;

            //초기화
            currentPartPageIndex = 0;
            currentPartButtonIndex = 0;
            pageArrows.SetActive(false);
        }
        Debug.Log($"현재 페이지: {currentPartPageIndex}, 선택된 버튼 인덱스: {currentPartButtonIndex}"); //delete

        HighlightPartButton();
    }

    private bool isThereButton(int _pageIndex, int _buttonIndex, int _increraseButtonIndex)
    {
        int targetButtonIndex = _buttonIndex + _increraseButtonIndex;

        if (targetButtonIndex > 3)
        {
            targetButtonIndex -= 4;
        }

        int lastButtonIndex = enemy.parts.Count - _pageIndex * 4;

        return targetButtonIndex < lastButtonIndex;        
    }
    #endregion

    #region DialogueButton
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
    #endregion

    #region PartButton
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

    public void UpdatePartButtons() //page 바뀔 때마다 실행
    {
        int startIndex = currentPartPageIndex * 4;

        for (int i = 0; i < partButtonList.Count; i++)
        {
            int partIndex = startIndex + i;
            GameObject partButton = partButtonList[i];
            Debug.Log($"partButton 이름: {partButton}"); //Delete

            if (partIndex < enemy.parts.Count) //part 개수 만큼만
            {
                partButton.SetActive(true);

                Text partText = partButton.transform.Find("Text (Legacy)").GetComponent<Text>();                
                Color partColor = enemy.isDestroyed[partIndex] ? Color.grey : Color.white; //isdetroyed로 동작
                partText.text = enemy.ReplacePartText(enemy.parts[partIndex]); //part 영->한 번역

                //Melpomene Mask-Head 우선순위
                if (enemy.enemyName == "Melpomene" && enemy.isMasked && enemy.parts[partIndex] == "Head")
                {
                    partColor = Color.grey;
                }

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
        UpdatePartPageArrows();
    }

    private void UpdatePartPageArrows()
    {
        pageArrows.SetActive(true);

        int totalPages = Mathf.CeilToInt((float)enemy.parts.Count / 4);

        //prePageArrow
        if (currentPartPageIndex > 0)
        {
            prePageArrow.gameObject.SetActive(true);
        }
        else
        {
            prePageArrow.gameObject.SetActive(false);
        }

        //nextPageArrow
        if (currentPartPageIndex < totalPages - 1)
        {
            nextPageArrow.gameObject.SetActive(true);
        }
        else
        {
            nextPageArrow.gameObject.SetActive(false);
        }
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

    public void UpdateHpBoxes(GameObject _hpBoxes, Part _part)
    {
        for (int i = 0; i < _hpBoxes.transform.childCount; i++)
        {
            GameObject hpBox = _hpBoxes.transform.GetChild(i).gameObject;

            hpBox.SetActive(i < _part.partMaxHp);
            hpBox.GetComponent<Image>().sprite = (i < _part.partHp) ? hpBoxFull : hpBoxEmpty; //part별 남은 hp
        }
    }
    #endregion

    public int FindListIndex(List<string> _list, string _element)
    {
        return _list.IndexOf(_element);
    }

    public string KorParticle(string _word, string _particleWithFinal, string _particleWithoutFinal)
    {
        if (string.IsNullOrEmpty(_word))
        {
            return _particleWithoutFinal;
        }

        char lastChar = _word[_word.Length - 1]; //마지막 글자

        if (lastChar < 0xAC00 || lastChar > 0xD7A3) //한글 여부 확인
        {
            return _particleWithoutFinal;
        }

        //받침 여부
        bool hasFinal = (lastChar - 0xAC00) % 28 != 0;
        return hasFinal ? _particleWithFinal : _particleWithoutFinal;
    }

    public IEnumerator FadeInOut(bool _isFadeIn, float _duration)
    {
        Debug.Log("FadeInout() 실행");
        isFadeInOut = true;

        Image image = blackBoard.GetComponent<Image>();

        //Fade In/Out 설정
        float startAlpha = _isFadeIn ?1f : 0f;
        float endAlpha = _isFadeIn ? 0f : 1f;

        //초기화
        float time = 0f;
        Color color = image.color;

        while (time < _duration)
        {
            float t = time / _duration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            image.color = color;

            time += Time.deltaTime;
            yield return null;
        }

        // 마지막 값 보정
        color.a = endAlpha;
        image.color = color;

        isFadeInOut = false;
    }
}
