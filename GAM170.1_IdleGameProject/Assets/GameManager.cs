using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //variable definitions -> DONE
    //variables being set on the basis of conditions being met -> DONE
    //casting variables -> DONE
    //variables modifying other variables -> DONE
    //variables being set to random values -> DONE
    //operands + - / * = == -> DONE
    //if else statements -> DONE
    //testing variables via debugging -> DONE
    //appropriate camel casing -> DONE
    //win and loss conditions and game restarting

    private enum IntervalType { second, tenSeconds, thirtySeconds, minute }

    [SerializeField] private IntervalType incrementInterval = IntervalType.second;
    [Header("Upgrade Settings")]
    [Range(0.5f, 10f)] [SerializeField] private float costModifier = 1.5f;
    [SerializeField] private float upgradeACost = 25, upgradeBCost = 50, upgradeCCost = 125, upgradeDCost = 250, upgradeECost = 1000;
    [Header("General UI Settings")]
    [SerializeField] private string currencyName = "Money";
    [Range(0.1f, 1f)][SerializeField] private float scrollTime = 0.25f;
    [SerializeField] AnimationCurve curve;
    [Header("UI References")]
    [SerializeField] private Text currentValueText;
    [SerializeField] private Text incrementPerSecondText;
    [SerializeField] private Text aCostText, bCostText, cCostText, dCostText, eCostText;

    private float intervalTimer = -1, lerpTimer = -1;

    public static float DisplayedCurrencyValue { get; private set; } = 0;
    public static float ActualCurrencyValue { get; private set; } = 0;
    public static float CurrencyPerSecond { get; private set; } = 0;

    /// <summary>
    /// Returns an interval float value based on the value of the interval enum.
    /// </summary>
    private float Interval 
    {
        get
        {
            //return value corresponding with enum label
            switch (incrementInterval) 
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
        currentValueText.text = currencyName + ": " + DisplayedCurrencyValue;
        incrementPerSecondText.text = currencyName + " per " + Interval + " seconds: " + CurrencyPerSecond;
        aCostText.text = "1. Upgrade A Cost: " + upgradeACost;
        bCostText.text = "2. Upgrade B Cost: " + upgradeBCost;
        cCostText.text = "3. Upgrade C Cost: " + upgradeCCost;
        dCostText.text = "4. Upgrade D Cost: " + upgradeDCost;
        eCostText.text = "5. Upgrade E Cost: " + upgradeECost;
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        //interval timer functionality
        if(intervalTimer >= 0)
        {
            intervalTimer += Time.deltaTime; //increment timer by time from last frame to this frame
            if(intervalTimer >= Interval) //if timer has expired
            {
                //generate random value, and increment currency (1 in 4 chance to only increment by half of currency per second)
                int randomNo = Random.Range(0, 100);
                if (randomNo > 24)
                {
                    AddCurrency(CurrencyPerSecond);
                    Debug.Log("Random value is " + randomNo + ". Increment per second added to current currency. Current currency is " + ActualCurrencyValue);
                    incrementPerSecondText.color = Color.white; //change UI text color to white
                }
                else
                {
                    AddCurrency(CurrencyPerSecond / 2);
                    Debug.Log("Random value is " + randomNo + ". Half of increment per second added to current currency. Current currency is " + ActualCurrencyValue);
                    incrementPerSecondText.color = Color.red; //change UI text color to red
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
                DisplayedCurrencyValue = (int)Mathf.Lerp(DisplayedCurrencyValue, ActualCurrencyValue, curve.Evaluate(lerpTimer / scrollTime));
            }
            currentValueText.text = currencyName + ": " + DisplayedCurrencyValue; //Update currency UI text
            if (lerpTimer >= scrollTime) //turn off lerp timer if it has expired
            {
                lerpTimer = -1;
            }
        }

        //Player input functionality
        if(Input.GetKeyDown(KeyCode.Space) == true)
        {
            //Spacebar pressed, increment currency by 1 and update UI text
            //both currency tracking variables are incremented to bypass timer
            AddCurrency(1, true);
            Debug.Log("Added 1 to current currency.");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) == true)
        {
            //Alpha 1 pressed, if upgrade A purchase successful increment associated cost and update UI text
            if(UpgradeIncrementPerSecond(upgradeACost, 0.5f) == true)
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
            CurrencyPerSecond += incrementAmount;
            AddCurrency(-cost);
            lerpTimer = 0;
            incrementPerSecondText.text = currencyName + " per " + Interval + " seconds: " + CurrencyPerSecond;
            return true;
        }
        return false;
    }

    private void AddCurrency(float amount, bool overrideDisplayedCurrency = false)
    {
        ActualCurrencyValue += amount;
        if (ActualCurrencyValue < 0)
        {
            ActualCurrencyValue = 0;
        }
        if (overrideDisplayedCurrency == true)
        {
            DisplayedCurrencyValue += amount;
            if (DisplayedCurrencyValue < 0)
            {
                DisplayedCurrencyValue = 0;
            }
            currentValueText.text = currencyName + ": " + DisplayedCurrencyValue;
        }
    }

    public static void ResetStaticValues()
    {
        DisplayedCurrencyValue = 0;
        ActualCurrencyValue = 0;
        CurrencyPerSecond = 0;
    }
}
