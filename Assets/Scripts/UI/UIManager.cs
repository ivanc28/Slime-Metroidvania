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
    [SerializeField] Transform slotContainer;
    [SerializeField] GridLayoutGroup gridLayout;
    [SerializeField] InventorySlot slotPrefab;

    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemDescription;

    private List<GameObject> addedSlots;
    private List<QuestCollectableData> addedItems;
    private bool inventoryOpen;
    private int currItemIndex;

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
        addedSlots = new();
        addedItems = new();
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
            int nextSlot = currItemIndex;
            int colsPerRow = gridLayout.constraintCount;
            if (Input.GetKeyDown(KeyCode.W))
            {
                if(nextSlot - colsPerRow >= 0)
                {
                    nextSlot -= colsPerRow;
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (nextSlot + colsPerRow < addedSlots.Count)
                {
                    nextSlot += colsPerRow;
                }   
                else if(currItemIndex < addedSlots.Count - 1 && currItemIndex + colsPerRow - (currItemIndex % 3) < addedSlots.Count)
                {
                    nextSlot = addedSlots.Count - 1;
                }
                else
                {
                    // do nothing?
                }
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                if(nextSlot - 1  >= 0 && currItemIndex % colsPerRow != 0)
                {
                    nextSlot--;
                }
                else
                {
                    // go left to tools
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                if(nextSlot + 1 < addedSlots.Count)
                {
                    nextSlot++;
                }
            }
            if(nextSlot != currItemIndex)
            {
                currItemIndex = nextSlot;
                UpdateItemDescription(addedItems[currItemIndex]);
                Debug.Log("We swap to a new item");
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

    public void FadeIn()
    {
        screenAnim.SetTrigger("FadeIn");
    }

    public void OpenInventory()
    {
        GameManager.Instance.SetPaused(true);
        inventory.SetActive(true);
        QuestCollectableData[] items = Player.Instance.inventory.GetAllItems().ToArray();
        foreach(QuestCollectableData item in items)
        {
            InventorySlot slot = Instantiate(slotPrefab, slotContainer);
            addedSlots.Add(slot.gameObject);
            addedItems.Add(item);
            slot.Initialize(item);
        }
        if(items.Length > 0)
        {
            UpdateItemDescription(items[0]);
        }
        currItemIndex = 0;
    }
    public void CloseInventory()
    {
        foreach (GameObject slot in addedSlots)
        {
            Destroy(slot);
        }
        addedSlots.Clear();
        addedItems.Clear();
        inventory.SetActive(false);
        GameManager.Instance.SetPaused(false);
    }

    public void UpdateItemDescription(QuestCollectableData item)
    {
        itemName.text = item.itemName;
        itemDescription.text = item.description;
    }
}
