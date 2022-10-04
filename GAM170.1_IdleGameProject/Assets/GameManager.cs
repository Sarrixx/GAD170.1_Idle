using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private enum IntervalType { second, tenSeconds, thirtySeconds, minute }

    [Tooltip("Defines the length time an 'interval' is.")]
    [SerializeField] private IntervalType intervalIncrement = IntervalType.second;
    [Header("Upgrade Settings")]
    [Tooltip("Defines the value by which upgrade costs are multiplied.")]
    [Range(0.5f, 10f)] [SerializeField] private float costModifier = 1.5f;
    [Header("General UI Settings")]
    [Tooltip("Defines the name of the game's currency.")]
    [SerializeField] private string currencyName = "Money";
    [Tooltip("Defines the fraction of time that the currency text scroll animation lasts for.")]
    [Range(0.1f, 1f)][SerializeField] private float scrollTime = 0.25f;
    [Tooltip("Defines the curve used by the text scroll animation.")]
    [SerializeField] AnimationCurve scrollCurve;
    [Header("UI References")]
    [Tooltip("Reference to the UI text display for the player's currency.")]
    [SerializeField] private Text currentValueText;
    [Tooltip("Reference to the UI text display for the player's interval earnings.")]
    [SerializeField] private Text incrementPerIntervalText;
    [Tooltip("Reference to the UI text display for the player's remaining debt.")]
    [SerializeField] private Text remainingText;
    [Tooltip("Reference to the UI text for upgrade A.")]
    [SerializeField] private Text aCostText;
    [Tooltip("Reference to the UI text for upgrade B.")]
    [SerializeField] private Text bCostText;
    [Tooltip("Reference to the UI text for upgrade C.")]
    [SerializeField] private Text cCostText;
    [Tooltip("Reference to the UI text for upgrade D.")]
    [SerializeField] private Text dCostText;
    [Tooltip("Reference to the UI text for upgrade E.")]
    [SerializeField] private Text eCostText;

    private float intervalTimer = -1, lerpTimer = -1;

    private static float upgradeACost = 25, upgradeBCost = 50, upgradeCCost = 125, upgradeDCost = 250, upgradeECost = 1000;

    /// <summary>
    /// The currency value currently being displayed.
    /// </summary>
    public static float DisplayedCurrencyValue { get; private set; } = 0;
    /// <summary>
    /// The player's actual currency value.
    /// </summary>
    public static float ActualCurrencyValue { get; private set; } = 0;
    /// <summary>
    /// The currency earned per interval.
    /// </summary>
    public static float CurrencyPerInterval { get; private set; } = 0;

    /// <summary>
    /// Returns an interval float value based on the value of the interval enum.
    /// </summary>
    private float Interval 
    {
        get
        {
            //return value corresponding with enum label
            switch (intervalIncrement) 
            {
                case IntervalType.second:
                    return 1f;
                case IntervalType.tenSeconds:
                    return 10f;
                case IntervalType.thirtySeconds:
                    return 30f;
                case IntervalType.minute:
                    return 60;
                default:
                    return 1f;
            }
        } 
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        intervalTimer = 0; //Initiates the '1 second' timer
        //Set the initial UI text values
        currentValueText.text = currencyName + ": " + string.Format("{0:n0}", DisplayedCurrencyValue);
        incrementPerIntervalText.text = currencyName + " per " + Interval + " seconds: " + CurrencyPerInterval;
        remainingText.text = "Remaining debt: " + string.Format("{0:n0}", Mathf.Clamp(100000 - ActualCurrencyValue, 0, 100000)); //format remaining as number with comma
        aCostText.text = "1. Upgrade A Cost: " + Mathf.CeilToInt(upgradeACost);
        bCostText.text = "2. Upgrade B Cost: " + Mathf.CeilToInt(upgradeBCost);
        cCostText.text = "3. Upgrade C Cost: " + Mathf.CeilToInt(upgradeCCost);
        dCostText.text = "4. Upgrade D Cost: " + Mathf.CeilToInt(upgradeDCost);
        eCostText.text = "5. Upgrade E Cost: " + Mathf.CeilToInt(upgradeECost);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        //interval timer functionality
        if (intervalTimer >= 0)
        {
            intervalTimer += Time.deltaTime; //increment timer by time from last frame to this frame
            if (intervalTimer >= Interval) //if timer has expired
            {
                //generate random value, and increment currency (1 in 4 chance to only increment by half of currency per second)
                int randomNo = Random.Range(0, 100);
                if (randomNo > 24)
                {
                    AddCurrency(CurrencyPerInterval);
                    Debug.Log("Random value is " + randomNo + ". Increment per second added to current currency. Current currency is " + ActualCurrencyValue);
                    incrementPerIntervalText.color = Color.white; //change UI text color to white
                }
                else
                {
                    AddCurrency(CurrencyPerInterval / 2);
                    Debug.Log("Random value is " + randomNo + ". Half of increment per second added to current currency. Current currency is " + ActualCurrencyValue);
                    incrementPerIntervalText.color = Color.red; //change UI text color to red
                }
                //reset timers to 0
                intervalTimer = 0;
                lerpTimer = 0;
            }
        }

        //Currency UI text lerp timer functionality (for 'scrolling' text effect)
        if (lerpTimer >= 0)
        {
            lerpTimer += Time.deltaTime; //increment timer by time from last frame to this frame
            if (DisplayedCurrencyValue != ActualCurrencyValue) //continue interpolating toward actual value if displayed value does not yet match
            {
                DisplayedCurrencyValue = (int)Mathf.Lerp(DisplayedCurrencyValue, ActualCurrencyValue, scrollCurve.Evaluate(lerpTimer / scrollTime));
            }
            currentValueText.text = currencyName + ": " + string.Format("{0:n0}", DisplayedCurrencyValue); //update currency UI text
            if (lerpTimer >= scrollTime) //turn off lerp timer if it has expired
            {
                lerpTimer = -1;
            }
        }

        //Player input functionality
        if (Input.GetKeyDown(KeyCode.Alpha1) == true)
        {
            //Alpha 1 pressed, if upgrade A purchase successful increment associated cost and update UI text
            if (UpgradeIncrementPerSecond(upgradeACost, 0.5f) == true)
            {
                upgradeACost *= costModifier;
                aCostText.text = "1. Upgrade A Cost: " + Mathf.CeilToInt(upgradeACost);
                Debug.Log("Upgrade A purchased. New cost is " + upgradeACost);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) == true)
        {
            //Alpha 2 pressed, if upgrade B purchase successful increment associated cost and update UI text
            if (UpgradeIncrementPerSecond(upgradeBCost, 1f) == true)
            {
                upgradeBCost *= costModifier;
                bCostText.text = "2. Upgrade B Cost: " + Mathf.CeilToInt(upgradeBCost);
                Debug.Log("Upgrade B purchased. New cost is " + upgradeBCost);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) == true)
        {
            //Alpha 3 pressed, if upgrade C purchase successful increment associated cost and update UI text
            if (UpgradeIncrementPerSecond(upgradeCCost, 5) == true)
            {
                upgradeCCost *= costModifier;
                cCostText.text = "3. Upgrade C Cost: " + Mathf.CeilToInt(upgradeCCost);
                Debug.Log("Upgrade C purchased. New cost is " + upgradeCCost);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) == true)
        {
            //Alpha 4 pressed, if upgrade D purchase successful increment associated cost and update UI text
            if (UpgradeIncrementPerSecond(upgradeDCost, 10) == true)
            {
                upgradeDCost *= costModifier;
                dCostText.text = "4. Upgrade D Cost: " + Mathf.CeilToInt(upgradeDCost);
                Debug.Log("Upgrade D purchased. New cost is " + upgradeDCost);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) == true)
        {
            //Alpha 5 pressed, if upgrade E purchase successful increment associated cost and update UI text
            if (UpgradeIncrementPerSecond(upgradeECost, 50) == true)
            {
                upgradeECost *= costModifier;
                eCostText.text = "5. Upgrade E Cost: " + Mathf.CeilToInt(upgradeECost);
                Debug.Log("Upgrade E purchased. New cost is " + upgradeECost);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            SceneManager.LoadScene(0);
        }
    }

    /// <summary>
    /// Can be used to purchase an upgrade with a specified cost, resulting in the passed increment amount being added
    /// to the current increment per second amount.
    /// </summary>
    /// <param name="cost">The amount of currency the upgrade costs.</param>
    /// <param name="incrementAmount">The amount the current increment per second amount will be increased by.</param>
    /// <returns>Returns true if the upgrade was successfully purchased.
    /// Returns false if the player does not have enough currency to make the purchase.</returns>
    private bool UpgradeIncrementPerSecond(float cost, float incrementAmount)
    {
        if(ActualCurrencyValue >= cost)
        {
            CurrencyPerInterval += incrementAmount;
            AddCurrency(-cost);
            lerpTimer = 0;
            incrementPerIntervalText.text = currencyName + " per " + Interval + " seconds: " + CurrencyPerInterval;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Adds the passed amount to the player's current currency.
    /// </summary>
    /// <param name="amount">The amount of currency to add to the current amount. May be negative.</param>
    /// <param name="overrideDisplayedCurrency">If true, will also override the currently displayed currency value.</param>
    private void AddCurrency(float amount, bool overrideDisplayedCurrency = false)
    {
        ActualCurrencyValue += amount;
        if (ActualCurrencyValue < 0)
        {
            ActualCurrencyValue = 0;
        }
        Debug.Log("Added " + amount + " to current currency.");
        remainingText.text = "Remaining debt: " + string.Format("{0:n0}", Mathf.Clamp(100000 - ActualCurrencyValue, 0, 100000)); //format remaining as number with comma
        if (overrideDisplayedCurrency == true)
        {
            DisplayedCurrencyValue += amount;
            if (DisplayedCurrencyValue < 0)
            {
                DisplayedCurrencyValue = 0;
            }
            currentValueText.text = currencyName + ": " + string.Format("{0:n0}", DisplayedCurrencyValue);
        }
    }

    /// <summary>
    /// Use to add one to value when UI window button is clicked.
    /// </summary>
    public void ClickButton()
    {
        AddCurrency(1, true);
    }

    /// <summary>
    /// Resets all static values associated with the GameManager.
    /// </summary>
    public static void ResetStaticValues()
    {
        DisplayedCurrencyValue = 0;
        ActualCurrencyValue = 0;
        CurrencyPerInterval = 0;
        upgradeACost = 0;
        upgradeBCost = 0;
        upgradeCCost = 0;
        upgradeDCost = 0;
        upgradeECost = 0;
    }
}
