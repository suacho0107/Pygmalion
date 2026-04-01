using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    #region References
    [SerializeField] private BattleManager battleManager;
    private Enemy enemy;
    #endregion

    #region Variables
    [Header("Backgrounds")]
    public Image background;
    public Sprite museumBackground;
    public Sprite libraryBackGround;

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public Text contentText;

    [SerializeField] private GameObject blackBoard;
    [SerializeField] private GameObject blackCircle;

    [Header("DialogueSelect UI")]
    [SerializeField] private GameObject dialogueButtons;
    private List<GameObject> dialogueButtonList = new();

    private int currentDialogueButtonIndex;

    [Header("PartSelect UI")]
    [SerializeField] private GameObject partButtons;
    private List<GameObject> partButtonList = new();

    [SerializeField] private GameObject pageArrows;
    [SerializeField] private Image prePageArrow;
    [SerializeField] private Image nextPageArrow;

    private int currentPartPageIndex;
    public int currentPartButtonIndex;

    [Header("Button Sprites")]
    [SerializeField] private Sprite ButtonDefault;
    [SerializeField] private Sprite ButtonHighlighted;

    [Header("HpBoxes & HpBox Sprites")]
    [SerializeField] private Sprite hpBoxEmpty;
    [SerializeField] private Sprite hpBoxFull;

    [Header("Flags")]
    private bool isFading;
    public bool isTyping;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        enemy = FindObjectOfType<Enemy>();

        SetDialogueButtons();
        SetPartButtons();
    }

    private void Start()
    {
        FadeInCircle(2f);
    }

    void Update()
    {
        if (isFading || isTyping)
        {
            return;
        }

        HandleInput();
    }
    #endregion

    private void HandleInput()
    {
        switch (battleManager.state)
        {
            case BattleManager.State.PLAYERTURN_START:
                if (dialogueButtons.activeSelf)
                {
                    DialogueButtonInputHandler();
                }
                break;

            case BattleManager.State.PLAYERTURN_ATTACK:
                if (partButtons.activeSelf)
                {
                    PartButtonInputHandler();
                }
                break;
        }
    }

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
            switch (currentDialogueButtonIndex)
            {
                case 0: //공격한다
                    battleManager.OnSelectAttack();
                    break;

                case 1: //소지품을 확인한다
                    battleManager.OnSelectInventory();
                    break;

                case 2: //도망친다
                    battleManager.OnSelectRun();
                    break;
            }
            
        //초기화
        currentDialogueButtonIndex = 0;
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
        //선택
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            int selectedPartIndex = currentPartPageIndex * 4 + currentPartButtonIndex;
            if (selectedPartIndex >= enemy.parts.Count)
            {
                return;
            }

            Part selectedPart = enemy.parts[selectedPartIndex];

            //Melpomene 예외
            if (enemy.enemyType == EnemyType.Melpomene && !enemy.IsPartDestroyed(PartType.Mask) && selectedPart.partType == PartType.Head)
            {
                return;
            }

            if (enemy.IsPartDestroyed(selectedPart.partType))
            {
                Debug.Log("This part is already destroyed"); //Delete
                return;
            }
            else
            {
                battleManager.OnSelectPart(selectedPart);
            }

            ResetUI();

            //초기화
            currentPartPageIndex = 0;
            currentPartButtonIndex = 0;
            //pageArrows.SetActive(false);
        }
        //취소
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetUI();
            ////UI초기화
            //contentText.text = "";
            //partButtons.SetActive(false);

            battleManager.ChangeState(BattleManager.State.PLAYERTURN_START);

            //초기화
            currentPartPageIndex = 0;
            currentPartButtonIndex = 0;
            //pageArrows.SetActive(false);
        }
        //Debug.Log($"현재 페이지: {currentPartPageIndex}, 선택된 버튼 인덱스: {currentPartButtonIndex}"); //delete



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

    #region EscapeInventory Legacy
    //private void EscapeInventory() // InventoryUI UseAnItem()으로 이동, ESC 삭제
    //{
    //    InventoryUI.instance.activeInventory = false;
    //    InventoryUI.instance.activeSelect = false;
    //    StopAllCoroutines();
    //    InventoryUI.instance.inventoryPanel.SetActive(false);
    //    InventoryUI.instance.activeItem = false;
    //    InventoryUI.instance.SaveInventory();
    //    contentText.text = "";
    //    battleManager.isStatePLAYERTURN = false;
    //    battleManager.state = BattleManager.State.PLAYERTURN_START;
    //    //battleManager.isStatePLAYERTURN = true;
    //}
    #endregion
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

        //초기화
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
        int pageStartIndex = currentPartPageIndex * 4;
        int partCount = enemy.parts.Count;

        for (int i = 0; i < partButtonList.Count; i++)
        {
            int partIndex = pageStartIndex + i;
            GameObject partButton = partButtonList[i];

            if (partIndex < partCount) //part 개수 만큼만
            {
                Part part = enemy.parts[partIndex];

                partButton.SetActive(true);

                Text partText = partButton.transform.Find("Text (Legacy)").GetComponent<Text>();
                partText.text = TranslatePart(part); //part 영->한 번역

                Color partColor = enemy.IsPartDestroyed(part.partType) ? Color.grey : Color.white; //isdetroyed로 동작
                //Melpomene Mask-Head 우선순위
                if (enemy.enemyType == EnemyType.Melpomene && !enemy.IsPartDestroyed(PartType.Mask) && part.partType == PartType.Head)
                {
                    partColor = Color.grey;
                }

                partText.color = partColor;

                GameObject hpBoxes = partButton.transform.Find("HpBoxes").gameObject;
                UpdateHpBoxes(hpBoxes, part);
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

            if (!partButton.activeSelf)
            {
                continue;
            }

            int partIndex = currentPartPageIndex * 4 + i;

            if (partIndex < enemy.parts.Count)
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

            hpBox.SetActive(i < _part.maxHp);
            hpBox.GetComponent<Image>().sprite = (i < _part.hp) ? hpBoxFull : hpBoxEmpty; //part별 남은 hp
        }
    }
    #endregion

    #region PlayerTurn UI
    public void Playerturn_Start()
    {
        ResetUI();
        StartCoroutine(TypeWriter("어떤 행동을 할까?"));

        dialogueButtons.SetActive(true);
    }

    public void Playerturn_Attack()
    {
        ResetUI();
        contentText.text = "어느 부위를 공격할까?";

        UpdatePartButtons();
        partButtons.SetActive(true);

    }

    public void PlayerTurn_Inventory()
    {
        Debug.Log($"PlayerTurn_Inventory 실행");

        ResetUI();

        InventoryUI.instance.inventoryPanel.SetActive(true);
        InventoryUI.instance.activeItem = true;
        InventoryUI.instance.selectedItem = 0;
        InventoryUI.instance.ShowItem();
        InventoryUI.instance.activeInventory = true;

        InventoryUI.instance.activeSelect = false;
        InventoryUI.instance.keyUp = true;
    }
    #endregion

    public void ResetUI()
    {
        contentText.text = "";

        dialogueButtons.SetActive(false);

        partButtons.SetActive(false);
        pageArrows.SetActive(false);
    }

    #region Fade
    #region Board
    public void FadeInBoard(float duration)
    {
        if (isFading)
        {
            return;
        }
        StartCoroutine(FadeBoard(true, duration));
    }

    public void FadeOutBoard(float duration)
    {
        if (isFading)
        {
            return;
        }
        StartCoroutine(FadeBoard(false, duration));
    }

    private IEnumerator FadeBoard(bool isFadeIn, float duration)
    {
        isFading = true;

        blackBoard.SetActive(true);

        Image image = blackBoard.GetComponent<Image>();

        float start = isFadeIn ? 1f : 0f;
        float end = isFadeIn ? 0f : 1f;
        float time = 0f;
        Color color = image.color;

        while (time < duration)
        {
            float t = time / duration;
            color.a = Mathf.Lerp(start, end, t);
            image.color = color;

            time += Time.deltaTime;
            yield return null;
        }
        color.a = end;
        image.color = color;

        isFading = false;
    }
    #endregion

    #region Circle
    public void FadeInCircle(float duration)
    {
        if (isFading)
        {
            return;
        }
        StartCoroutine(FadeCircle(true, duration));
    }

    public void FadeOutCircle(float duration)
    {
        if (isFading)
        {
            return;
        }
        StartCoroutine(FadeCircle(false, duration));
    }

    private IEnumerator FadeCircle(bool isFadeIn, float duration)
    {
        isFading = true;

        blackCircle.SetActive(true);
        RectTransform circle = blackCircle.GetComponent<RectTransform>();

        float start = isFadeIn ? 2250 : 0;
        float end = isFadeIn ? 0 : 2250;
        float time = 0f;
        circle.sizeDelta = new Vector2(start, start);

        while (time < duration)
        {
            float t = time / duration;

            float size = Mathf.Lerp(start, end, t);
            circle.sizeDelta = new Vector2(size, size);

            time += Time.deltaTime;
            yield return null;
        }

        //마지막 값 보정
        circle.sizeDelta = new Vector2(end, end);

        isFading = false;
    }
    #endregion
    #endregion

    public IEnumerator Shake(Transform _target, float _dration, float _strength) //duration = 0.2f, strength = 10f)
    {
        Vector3 originPos = _target.localPosition;
        float elapsed = 0f;

        //_dration(sec) 동안 _strength만큼 흔들림
        while (elapsed < _dration)
        {
            float x = Random.Range(-1f, 1f) * _strength;
            float y = Random.Range(-1f, 1f) * _strength;

            _target.localPosition = originPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;

            yield return null;
        }

        //위치 초기화
        transform.localPosition = originPos;
    }

    public IEnumerator UpdateHpBar(Image hpBar, int newHp, int maxHp)
    {
        float start = hpBar.fillAmount;
        float end = (float)newHp / maxHp;
        float time = 0f;
        float duration = 0.5f;

        while (time < duration)
        {
            float t = time / duration;
            hpBar.fillAmount = Mathf.Lerp(start, end, t);

            time += Time.deltaTime;
            yield return null;
        }
        hpBar.fillAmount = end;
    }

    #region TypeWriter
    public IEnumerator TypeWriter(string _text)
    {
        if (isTyping)
        {
            yield return new WaitUntil(() => !isTyping);
        }

        isTyping = true;
        contentText.text = "";

        for (int i = 0; i < _text.Length; i++)
        {
            contentText.text += _text[i];
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(0.1f);

        isTyping = false;
    }
    #endregion

    #region Utility 
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
    #endregion

    #region State UI
    public void Win()
    {
        ResetUI();
        FadeOutBoard(2f);
    }

    public void Lose()
    {
        StartCoroutine(TypeWriter("눈앞이 흐려진다..."));
        FadeOutBoard(2f);
    }

    public void Run()
    {
        ResetUI();
        dialoguePanel.SetActive(false);
        FadeOutCircle(2f);
    }

    public void SetBackground(int stage)
    {
        switch (stage)
        {
            case 0: //미술관
                background.sprite = museumBackground;
                break;

            case 1: //도서관
                background.sprite = libraryBackGround;
                break;
        }
    }
    #endregion

    #region Translation
    public string TranslateEnemy(Enemy enemy)
    {
        EnemyType origin = enemy.enemyType;
        string translated;

        if (origin == EnemyType.Aphrodite)
        {
            translated = "아프로디테";
        }
        else if (origin == EnemyType.ReadingChild)
        {
            translated = "책을 읽는 아이";
        }
        else if (origin == EnemyType.Melpomene)
        {
            translated = "멜포메네";
        }
        else
        {
            translated = enemy.name;
        }

        return translated;
    }

    public string TranslatePart(Part part)
    {
        PartType origin = part.partType;

        if (origin == PartType.Head)
        {
            return "머리";
        }
        else if (origin == PartType.Mask)
        {
            return "가면";
        }
        else if (origin == PartType.Body)
        {
            return "몸통";
        }
        else if (origin == PartType.LArm)
        {
            return "왼팔";
        }
        else if (origin == PartType.RArm)
        {
            return "오른팔";
        }
        else if (origin == PartType.LLeg)
        {
            return "왼다리";
        }
        else if (origin == PartType.RLeg)
        {
            return "오른다리";
        }
        else
        {
            return part.name;
        }
    }
    #endregion
}

