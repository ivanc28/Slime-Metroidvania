using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

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
            currencyAccText.text = $"+{accumulatedCurrency}";
            if (currencyTimer > 0)
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
}
