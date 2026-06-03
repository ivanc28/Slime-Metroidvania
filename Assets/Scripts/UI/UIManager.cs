using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor.UI;

public class UIManager : MonoBehaviour
{
    [Header("Currency")]
    [SerializeField] TextMeshProUGUI currencyText;
    [SerializeField] TextMeshProUGUI currencyAccText;
    [SerializeField] TextMeshProUGUI grappleChargeText;
    [SerializeField] float currencyDelayBeforeAcc;
    [SerializeField] float currencyDelayAfterAcc;
    [Tooltip("How much time between each increment of player's final currency (coming from accumulatedCurrency)")]
    [SerializeField] float delayBetweenIncrements;
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

    [Header("Tool Display")]
    [SerializeField] Image toolImage;

    [Header("Collection Text")]
    [SerializeField] Animator collectTextAnim;
    [SerializeField] GameObject collectTextContainer;
    [SerializeField] TextMeshProUGUI collectText;

    [Header("Inventory")]
    [SerializeField] GameObject inventory;
    [SerializeField] Transform questSlotContainer;
    [SerializeField] Transform toolSlotContainer;
    [SerializeField] GridLayoutGroup gridLayout;
    [SerializeField] InventorySlot slotPrefab;
    [SerializeField] Image inventoryFrame;
    [SerializeField] float frameSpeed;

    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemDescription;

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

    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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
        if (Player.Instance.tools.GetCurrToolOption() != null)
        {
            UpdateToolDispay(Player.Instance.tools.GetCurrToolOption());
        }
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
        UpdateGrappleCharges();
        #endregion

        #region Inventory
        if (!inventoryOpen)
        {
            if (Input.GetKeyDown(KeyCode.I))
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
                    }
                }
                if (nextSlot != currToolItemIndex)
                {
                    currToolItemIndex = nextSlot;
                    UpdateItemDescription(addedToolItems[currToolItemIndex]);
                    SetFrameTargetPos(addedToolSlots[currToolItemIndex].GetComponent<RectTransform>().position);
                }
            }
            
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
            yield return new WaitForSeconds(delayBetweenIncrements);
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
        currencyText.enabled = enabled;
        currencyAccText.enabled = enabled;
    }
    public void DisplayCurrency(bool value)
    {
        currencyText.enabled = value;
    }
    public void UpdateGrappleCharges()
    {
        grappleChargeText.text = (Player.Instance.grappleCharges + (Player.Instance.grappleRechargeTimer/Player.Instance.grappleRechargeTime)).ToString("0.0");
    }

    public void UpdateToolDispay(ToolOption toolOption)
    {
        if (Player.Instance.tools.HasTool(toolOption.tool))
        {
            if (toolOption.toolImage != null)
            {
                toolImage.sprite = toolOption.toolImage.sprite;
                toolImage.color = Color.white;
            }
            else
            {
                toolImage.color = Color.clear;
            }
        }
    }

    public void DisplayCollectText(string text)
    {
        collectTextContainer.SetActive(true);
        collectText.text = text;
        collectTextAnim.SetBool("isCollecting", true);
    }
    public void DisableCollectText()
    {
        collectTextAnim.SetBool("isCollecting", false);
        collectTextContainer.SetActive(false);
    }
    public IEnumerator ShowCollectTextAndWait(string text)
    {
        DisplayCollectText(text);
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }
        DisableCollectText();
    }

    public void FadeIn()
    {
        screenAnim.SetTrigger("FadeIn");
    }
    public void FadeOut()
    {
        screenAnim.SetTrigger("FadeOut");
    }

    public void OpenInventory()
    {
        GameManager.Instance.SetPaused(true);
        inventory.SetActive(true);
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
}
