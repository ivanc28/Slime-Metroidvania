using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [Header("Currency")]
    [SerializeField] TextMeshProUGUI currencyText;
    [SerializeField] TextMeshProUGUI currencyAccText;
    [SerializeField] float currencyDelayBeforeAcc;
    [SerializeField] float currencyDelayAfterAcc;
    [Tooltip("How much time between each increment of player's final currency (coming from accumulatedCurrency)")]
    [SerializeField] float delayBetweenIncrements;
    [SerializeField] float delayBetweenDecrements;
    [SerializeField] Animator currencyAnim;
    private int prevCurrencyBeforeAccumulation;
    private int accumulatedCurrency;
    // ADDING CURRENCY
    private float currencyTimer;
    private bool startCurrencyTimer;
    private bool startedGettingCurrency;
    // SUBTRACTING CURRENCY
    private float subCurrencyTimer;
    private bool subStartCurrencyTimer;
    private bool subStartedLosingCurrency;

    [Header("Grappling Bar")]
    [SerializeField] Slider grappleSlider;
    [SerializeField] Transform grappleNotchesContainer;
    [SerializeField] GridLayoutGroup grappleGridGroup;
    [SerializeField] GameObject notchPrefab;
    [SerializeField] TextMeshProUGUI grappleCountText;
    [SerializeField] float grappleSliderWidthIncrements;
    [SerializeField] TextMeshProUGUI plusOneGrapplePrefab;
    [SerializeField] float plusGrappleMoveSpeed;
    [Tooltip("How long the text moves upward")]
    [SerializeField] float plusGrappleMoveUpTime;
    [SerializeField] float plusGrappleFadeTime;
    private float startingGrappleSliderWidth;

    [Header("Tool Display")]
    [SerializeField] Image toolImage;

    [Header("Collection Text")]
    [SerializeField] Animator collectTextAnim;
    [SerializeField] GameObject collectTextContainer;
    [SerializeField] TextMeshProUGUI collectText;
    [SerializeField] AudioClip collectPopUpClip;

    [Header("Inventory")]
    [SerializeField] GameObject inventory;
    [SerializeField] TextMeshProUGUI inventoryCurrencyText;
    [SerializeField] Transform questSlotContainer;
    [SerializeField] Transform toolSlotContainer;
    [SerializeField] GridLayoutGroup gridLayout;
    [SerializeField] InventorySlot slotPrefab;
    [SerializeField] Image inventoryFrame;
    [SerializeField] float frameSpeed;

    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemDescription;

    [SerializeField] AudioClip openClip;
    [SerializeField] AudioClip closeClip;
    [SerializeField] AudioClip swipeClip;

    private List<GameObject> addedQuestSlots;
    private List<QuestCollectableData> addedQuestItems;
    private List<GameObject> addedToolSlots;
    private List<ToolInventoryData> addedToolItems;
    private bool inventoryOpen;
    private bool lookingAtQuestItems;
    private int currQuestItemIndex;
    private int currToolItemIndex;
    private Vector2 frameTargetPos;
    private bool recalculatedLayout;

    [Header("Screen Transition")]
    [SerializeField] Animator screenAnim;

    [Header("Region Title")]
    [SerializeField] TextMeshProUGUI regionTitleText;
    [Tooltip("0: Sweet, 1: Savory, 2: Sour, 3: Bitter")]
    [SerializeField] string[] regionNames;
    [SerializeField] Animator regionTitleAnim;

    [Header("Pause Menu")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject pauseElements;
    [SerializeField] GameObject optionsMenu;
    private bool optionsOpen;
    [SerializeField] ButtonFX buttonFX;

    [Header("Map")]
    [SerializeField] GameObject mapDisplay;
    [SerializeField] Image mapImage;
    [Tooltip("0 is Empty Map, 1 is Sweet, 2 is Savory, 3 is Sour, 4 is Bitter")]
    [SerializeField] Sprite[] mapSprites;

    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        canvas.worldCamera = Camera.main;
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 101;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnableCurrencyText(false);
        currencyTimer = currencyDelayBeforeAcc;
        subCurrencyTimer = currencyDelayBeforeAcc;
        currencyText.text = Player.Instance.currencyData.GetCurrency().ToString();
        addedQuestSlots = new();
        addedQuestItems = new();
        addedToolSlots = new();
        addedToolItems = new();
        UpdateToolDispay(Player.Instance.tools.GetCurrToolOption());
        startingGrappleSliderWidth = grappleSlider.GetComponent<RectTransform>().rect.width;
        SetGrappleSliderWidth();
        while (grappleNotchesContainer.childCount < Player.Instance.maxGrappleCharges)
        {
            AddNewGrappleNotch(false);
        }

        EnableGrappleSlider();
    }

    // Update is called once per frame
    void Update()
    {
        #region Currency Update       
        // adding
        if (startCurrencyTimer)
        {
            if(currencyTimer > 0)
            {
                currencyTimer -= Time.deltaTime;
            }
            else
            {
                StartCoroutine(AddAccCurrencyToFinal());
                startCurrencyTimer = false;
            }
        }

        // subtracting
        if (subStartCurrencyTimer)
        {
            if (subCurrencyTimer > 0)
            {
                subCurrencyTimer -= Time.deltaTime;
            }
            else
            {
                StartCoroutine(SubAccCurrencyToFinal());
                subStartCurrencyTimer = false;
            }
        }
        #endregion
        #region Grapple Charges
        UpdateGrappleSlider();
        #endregion

        if (Player.Instance.InSceneTransition)
        {
            return;
        }
        #region Inventory
        if (!inventoryOpen)
        {
            if (Input.GetKeyDown(KeyCode.I) && !GameManager.Instance.GamePaused)
            {
                OpenInventory();
                inventoryOpen = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                CloseInventory();
                inventoryOpen = false;
            }
        }

        if (inventoryOpen)
        {
            if (recalculatedLayout)
            {
                inventoryFrame.rectTransform.position = Vector2.Lerp(inventoryFrame.rectTransform.position, frameTargetPos, Time.unscaledDeltaTime * frameSpeed);
            }
            // Looking at quest items section
            if (lookingAtQuestItems)
            {                
                int nextSlot = currQuestItemIndex;
                int colsPerRow = gridLayout.constraintCount;
                if (Input.GetKeyDown(KeyCode.W))
                {
                    if (nextSlot - colsPerRow >= 0)
                    {
                        nextSlot -= colsPerRow;
                    }
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    if (nextSlot + colsPerRow < addedQuestSlots.Count)
                    {
                        nextSlot += colsPerRow;
                    }
                    else if (currQuestItemIndex < addedQuestSlots.Count - 1 && currQuestItemIndex + colsPerRow - (currQuestItemIndex % colsPerRow) < addedQuestSlots.Count)
                    {
                        nextSlot = addedQuestSlots.Count - 1;
                    }
                    else
                    {
                        // do nothing?
                    }
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    if (nextSlot - 1 >= 0 && currQuestItemIndex % colsPerRow != 0)
                    {
                        nextSlot--;
                    }
                    else
                    {
                        if(addedToolItems.Count > 0)
                        {
                            // go left to tools
                            // set next slot to be same as currQuestItemIndex so that we skip the if-statement that checks their inequality
                            nextSlot = 0;
                            currQuestItemIndex = 0;
                            currToolItemIndex = 0;
                            lookingAtQuestItems = false;
                            UpdateItemDescription(addedToolItems[0]);
                            inventoryFrame.transform.SetParent(toolSlotContainer);
                            SetFrameTargetPos(addedToolSlots[0].GetComponent<RectTransform>().position);
                            if (SoundManager.Instance != null)
                            {
                                SoundManager.Instance.PlaySound(swipeClip, 0.5f);
                            }
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    if (nextSlot + 1 < addedQuestSlots.Count && nextSlot % colsPerRow != colsPerRow - 1)
                    {
                        nextSlot++;
                    }
                }
                if (nextSlot != currQuestItemIndex)
                {
                    currQuestItemIndex = nextSlot;
                    UpdateItemDescription(addedQuestItems[currQuestItemIndex]);
                    SetFrameTargetPos(addedQuestSlots[currQuestItemIndex].GetComponent<RectTransform>().position);
                    if(SoundManager.Instance != null)
                    {
                        SoundManager.Instance.PlaySound(swipeClip, 0.5f);
                    }
                }
            }
            // Looking at tools sections
            else
            {
                int nextSlot = currToolItemIndex;
                if (Input.GetKeyDown(KeyCode.W))
                {
                    nextSlot--;
                    if(nextSlot < 0)
                    {
                        nextSlot = addedToolSlots.Count - 1;
                    }
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    nextSlot++;
                    if(nextSlot >= addedToolSlots.Count)
                    {
                        nextSlot = 0;
                    }
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    if(addedQuestItems.Count > 0)
                    {
                        // go right quest items
                        // set next slot to be same as currToolItemIndex so that we skip the if-statement that checks their inequality
                        nextSlot = 0;
                        currQuestItemIndex = 0;
                        currToolItemIndex = 0;
                        lookingAtQuestItems = true;
                        UpdateItemDescription(addedQuestItems[0]);
                        inventoryFrame.transform.SetParent(questSlotContainer);
                        SetFrameTargetPos(addedQuestSlots[0].GetComponent<RectTransform>().position);
                        if (SoundManager.Instance != null)
                        {
                            SoundManager.Instance.PlaySound(swipeClip, 0.5f);
                        }
                    }
                }
                if (nextSlot != currToolItemIndex)
                {
                    currToolItemIndex = nextSlot;
                    if(addedToolItems.Count > 0)
                    {
                        UpdateItemDescription(addedToolItems[currToolItemIndex]);
                        SetFrameTargetPos(addedToolSlots[currToolItemIndex].GetComponent<RectTransform>().position);
                        if (SoundManager.Instance != null)
                        {
                            SoundManager.Instance.PlaySound(swipeClip, 0.5f);
                        }
                    }
                }
            }
            
        }
        #endregion
        #region Pause Menu
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (inventoryOpen)
            {
                CloseInventory();
                inventoryOpen = false;
            }
            else
            {
                if (GameManager.Instance.GamePaused)
                {
                    if (!optionsOpen)
                    {
                        ResumeGame();
                    }
                    else
                    {
                        ToggleOptions(false);
                        pauseElements.SetActive(true);
                    }
                }
                else
                {
                    DisplayPauseMenu(true);
                    GameManager.Instance.SetPaused(true);
                    ToggleOptions(false);
                }
            }
        }
        #endregion
        #region Map
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DisplayMap(GameManager.Instance.GetCurrRegion());
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            HideMap();
        }        
        #endregion
    }
    // Called everytime we increase our currency 
    public void UpdateCurrencyUp(int amt)
    {
        if (!startedGettingCurrency)
        {
            prevCurrencyBeforeAccumulation = Player.Instance.currencyData.GetCurrency();
            startedGettingCurrency = true;
        }
        EnableCurrencyText(true);

        accumulatedCurrency += amt;
        currencyAccText.text = $"+{accumulatedCurrency}";

        currencyTimer = currencyDelayBeforeAcc;
        startCurrencyTimer = true;

    }
    private IEnumerator AddAccCurrencyToFinal()
    {
        while (accumulatedCurrency > 0)
        {
            yield return new WaitForSeconds(delayBetweenIncrements);
            prevCurrencyBeforeAccumulation++;
            currencyText.text = $"{prevCurrencyBeforeAccumulation}";
            accumulatedCurrency--;
            currencyAccText.text = $"+{accumulatedCurrency}";
            if(currencyTimer > 0)
            {
                yield break;
            }
        }
        yield return new WaitForSeconds(currencyDelayAfterAcc);
        if (currencyTimer > 0)
        {
            yield break;
        }
        // finished accumulating our currency
        EnableCurrencyText(false);
        startedGettingCurrency = false;
    }

    public void UpdateCurrencyDown(int amt)
    {
        if (!subStartedLosingCurrency)
        {
            prevCurrencyBeforeAccumulation = Player.Instance.currencyData.GetCurrency();
            subStartedLosingCurrency = true;
        }
        EnableCurrencyText(true);

        accumulatedCurrency += amt;
        currencyAccText.text = $"-{accumulatedCurrency}";

        subCurrencyTimer = currencyDelayBeforeAcc / 2;
        subStartCurrencyTimer = true;

    }
    private IEnumerator SubAccCurrencyToFinal()
    {
        while (accumulatedCurrency > 0)
        {
            yield return new WaitForSeconds(delayBetweenDecrements);
            prevCurrencyBeforeAccumulation--;
            currencyText.text = $"{prevCurrencyBeforeAccumulation}";
            accumulatedCurrency--;
            currencyAccText.text = $"-{accumulatedCurrency}";
            if (subCurrencyTimer > 0)
            {
                yield break;
            }
        }
        yield return new WaitForSeconds(currencyDelayAfterAcc * 2);
        if (subCurrencyTimer > 0)
        {
            yield break;
        }
        // finished accumulating our currency
        EnableCurrencyText(false);
        subStartedLosingCurrency = false;
    }
    private void EnableCurrencyText(bool enabled)
    {
        currencyAnim.SetBool("currencyVisible", enabled);
        currencyAccText.enabled = enabled;
    }
    public void DisplayCurrency(bool value)
    {
        currencyAnim.SetBool("currencyVisible", value);
    }
    private void UpdateGrappleSlider()
    {
        if(Player.Instance.maxGrappleCharges > 0)
        { 
            grappleSlider.value = (Player.Instance.grappleCharges + (Player.Instance.grappleRechargeTimer / Player.Instance.grappleRechargeTime)) / Player.Instance.maxGrappleCharges;
            grappleCountText.text = $"{Player.Instance.grappleCharges}/{Player.Instance.maxGrappleCharges}";
        }

    }
    public void AddNewGrappleNotch(bool updateWidth)
    {
        if (updateWidth)
        {
            SetGrappleSliderWidth();
        }
        Instantiate(notchPrefab, grappleNotchesContainer);
        float totalNotchWidth = grappleGridGroup.cellSize.x * (Player.Instance.maxGrappleCharges - 1);
        float remainingSpace = grappleNotchesContainer.GetComponent<RectTransform>().rect.width - totalNotchWidth;
        float spacingBetween = remainingSpace / Player.Instance.maxGrappleCharges;
        grappleGridGroup.spacing = new Vector2(spacingBetween, 0);

        // // spawn an increment grapple obj UI
        // TextMeshProUGUI extraGrappleTxt = Instantiate(plusOneGrapplePrefab, canvas.transform);
        // extraGrappleTxt.transform.SetAsFirstSibling();
        // StartCoroutine(FadeText(extraGrappleTxt, plusGrappleFadeTime, plusGrappleMoveUpTime, plusGrappleMoveSpeed));
    }

    public void ShowGrappleIncrement()
    {
        // spawn an increment grapple obj UI
        TextMeshProUGUI extraGrappleTxt = Instantiate(plusOneGrapplePrefab, canvas.transform);
        extraGrappleTxt.transform.SetAsFirstSibling();
        StartCoroutine(FadeText(extraGrappleTxt, plusGrappleFadeTime, plusGrappleMoveUpTime, plusGrappleMoveSpeed));
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float fadeTime, float moveUpTime, float moveSpeed)
    {
        float timer = 0;
        while (timer < moveUpTime)
        {
            timer += Time.deltaTime;
            text.transform.position += moveSpeed * Time.deltaTime * Vector3.up;
            yield return null;
        }
        timer = fadeTime;
        Color textColor = text.color;
        Color targetColor = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            text.color = Color.Lerp(targetColor, textColor, timer / fadeTime);
            text.transform.position += moveSpeed * Time.deltaTime * Vector3.up;
            yield return null;
        }
        Destroy(text.gameObject);
    }

    private void SetGrappleSliderWidth()
    {
        RectTransform sliderRect = grappleSlider.GetComponent<RectTransform>();
        float newWidth = startingGrappleSliderWidth + grappleSliderWidthIncrements * (Player.Instance.maxGrappleCharges - 1);
        sliderRect.sizeDelta = new Vector2(newWidth, sliderRect.rect.height);
        RectTransform containerRect = grappleNotchesContainer.GetComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(newWidth, containerRect.rect.height);
    }

    public void UpdateToolDispay(ToolOption toolOption)
    {
        if(toolOption == null)
        {
            toolImage.enabled = false;
            return;
        }
        if (Player.Instance.tools.HasTool(toolOption.tool))
        {
            toolImage.enabled = true;
            if (toolOption.toolSprite != null)
            {
                toolImage.sprite = toolOption.toolSprite;
                toolImage.color = Color.white;
            }
            else
            {
                toolImage.color = Color.clear;
            }
        }
        else
        {
            toolImage.enabled = false;
        }
    }

    public void DisplayCollectText(string text)
    {
        collectTextContainer.SetActive(true);
        collectText.text = text;
        collectTextAnim.SetBool("isCollecting", true);
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(collectPopUpClip, 0.2f, true, 1.1f, 1.1f);
        }
    }
    public void DisableCollectText()
    {
        collectTextAnim.SetBool("isCollecting", false);
        collectTextContainer.SetActive(false);
    }
    public IEnumerator ShowCollectTextAndWait(string text)
    {
        DisplayCollectText(text);
        yield return null;
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        DisableCollectText();
    }

    public void FadeOut()
    {
        screenAnim.SetTrigger("FadeOut");
    }
    public void FadeIn()
    {
        screenAnim.SetTrigger("FadeIn");
    }
    public void OpenInventory()
    {
        GameManager.Instance.SetPaused(true);
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(openClip, 1);
        }
        inventory.SetActive(true);
        inventoryCurrencyText.text = Player.Instance.currencyData.GetCurrency().ToString();
        QuestCollectableData[] items = Player.Instance.inventory.GetAllItems().ToArray();
        foreach(QuestCollectableData item in items)
        {
            InventorySlot slot = Instantiate(slotPrefab, questSlotContainer);
            addedQuestSlots.Add(slot.gameObject);
            addedQuestItems.Add(item);
            slot.Initialize(item);
        }
        ToolInventoryData[] tools = Player.Instance.inventory.GetAllTools().ToArray();
        foreach(ToolInventoryData tool in tools)
        {
            InventorySlot slot = Instantiate(slotPrefab, toolSlotContainer);
            addedToolSlots.Add(slot.gameObject);
            addedToolItems.Add(tool);
            slot.Initialize(tool);
        }
        if(tools.Length > 0)
        {
            inventoryFrame.enabled = true;
            UpdateItemDescription(tools[0]);

            // Calculate the layout grid and set the pos of the inventory frame
            LayoutRebuilder.ForceRebuildLayoutImmediate(toolSlotContainer.GetComponent<RectTransform>());
            Vector2 targetPos = addedToolSlots[0].GetComponent<RectTransform>().position;

            frameTargetPos = targetPos;
            inventoryFrame.transform.SetParent(toolSlotContainer);
            inventoryFrame.rectTransform.position = targetPos;
            recalculatedLayout = true;
            lookingAtQuestItems = false;
        }
        // hover over a quest item first, so we set up the layout grid of quest item 2nd
        if(items.Length > 0)
        {
            inventoryFrame.enabled = true;
            UpdateItemDescription(items[0]);

            // Calculate the layout grid and set the pos of the inventory frame
            LayoutRebuilder.ForceRebuildLayoutImmediate(questSlotContainer.GetComponent<RectTransform>());
            Vector2 targetPos = addedQuestSlots[0].GetComponent<RectTransform>().position;

            frameTargetPos = targetPos;
            inventoryFrame.transform.SetParent(questSlotContainer);
            inventoryFrame.rectTransform.position = targetPos;
            recalculatedLayout = true;
            lookingAtQuestItems = true;
        }
        currQuestItemIndex = 0;
        currToolItemIndex = 0;
    }
    public void CloseInventory()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(closeClip, 1);
        }
        foreach (GameObject slot in addedQuestSlots)
        {
            Destroy(slot);
        }
        foreach(GameObject slot in addedToolSlots)
        {
            Destroy(slot);
        }
        addedQuestSlots.Clear();
        addedQuestItems.Clear();
        addedToolSlots.Clear();
        addedToolItems.Clear();
        inventoryFrame.enabled = false;
        recalculatedLayout = false;
        inventory.SetActive(false);
        GameManager.Instance.SetPaused(false);
    }

    private void SetFrameTargetPos(Vector2 targetPos)
    {
        frameTargetPos = targetPos;
    }

    public void UpdateItemDescription(QuestCollectableData item)
    {
        itemName.text = item.itemName;
        itemDescription.text = item.description;
    }
    public void UpdateItemDescription(ToolInventoryData tool)
    {
        itemName.text = tool.toolName;
        itemDescription.text = tool.description;
    }

    public void EnableGrappleSlider()
    {
        bool active = Player.Instance.maxGrappleCharges > 0;
        grappleSlider.gameObject.SetActive(active);
        grappleCountText.enabled = active;
    }

    public void DisplayRegionTitle(GameManager.Region region)
    {
        switch (region)
        {
            case GameManager.Region.Sweet:
                regionTitleText.text = regionNames[0];
                break;
            case GameManager.Region.Savory:
                regionTitleText.text = regionNames[1];
                break;
            case GameManager.Region.Sour:
                regionTitleText.text = regionNames[2];
                break;
            case GameManager.Region.Bitter:
                regionTitleText.text = regionNames[3];
                break;
            default:
                regionTitleText.text = "";
                break;
        }
        regionTitleAnim.SetTrigger("showName");
    }

    public void DisplayPauseMenu(bool enabled)
    {
        pauseMenu.SetActive(enabled);
    }
    // called by buttons
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        GameManager.Instance.SetPaused(false);
        buttonFX.ResetButtonSprites();
    }
    public void QuitToMenu()
    {
        GameManager.Instance.ResetGame();
        GameManager.Instance.SetPaused(false);
        EndGameManager.ResetTrophiesClaimed();
        SceneManager.LoadScene("MainMenu");
    }
    public void ToggleOptions(bool open)
    {
        optionsMenu.SetActive(open);
        optionsOpen = open;
    }

    public void DisplayMap(GameManager.Region currRegion)
    {
        if (Player.Instance.inventory.HasMap(currRegion))
        {
            int index = 0;
            switch (currRegion)
            {
                case GameManager.Region.Sweet:
                    index = 1;
                    break;
                case GameManager.Region.Savory:
                    index = 2;
                    break;
                case GameManager.Region.Sour:
                    index = 3;
                    break;
                case GameManager.Region.Bitter:
                    index = 4;
                    break;
            }
            mapImage.sprite = mapSprites[index];
        }
        else
        {
            mapImage.sprite = mapSprites[0];
        }
        mapDisplay.SetActive(true);
    }
    public void HideMap()
    {
        mapDisplay.SetActive(false);
    }
}
