using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class WeightedSlotMachine : MonoBehaviour
{
    public float spinDuration = 2.0f;
    public int numberOfSym = 10;

    public TextMeshProUGUI firstReel;
    public TextMeshProUGUI secondReel;
    public TextMeshProUGUI thirdReel;
    public TextMeshProUGUI betResult;

    public TextMeshProUGUI totalCredits;
    public TMP_InputField inputBet;

    private bool startSpin = false;
    private bool firstReelSpun = false;
    private bool secondReelSpun = false;
    private bool thirdReelSpun = false;

    private int betAmount = 100;
    private int credits = 1000;

    [Header("Custom ADDED")]
    [SerializeField] private Image _background;
    [SerializeField] private GameObject _slotMachine;
    [SerializeField] private GameObject _gameOver;
    [SerializeField] private Color _gameOverColor;

    [Serializable]
    public struct WeightedProbability
    {
        public int number;
        public int weight;
    }

    [SerializeField]
    private List<WeightedProbability> weightedReelPoll = new List<WeightedProbability>();
    private int zeroProbability = 50;

    private int firstReelResult = 0;
    private int secondReelResult = 0;
    private int thirdReelResult = 0;

    private float elapsedTime = 0.0f;



    // Use this for initialization
    void Start()
    {
        weightedReelPoll.Add(new WeightedProbability
        {
            number = 0,
            weight = zeroProbability
        });

        int remainingValuesProb = (100 - zeroProbability) / 9;

        for (int i = 1; i < 10; i++)
        {
            weightedReelPoll.Add(new WeightedProbability
            {
                number = i,
                weight = remainingValuesProb
            });
        }
    }


    public void Spin()
    {
        if (betAmount > 0)
        {
            startSpin = true;
        }
        else
        {
            betResult.text = "Insert a \n valid bet!";
        }
    }


    void OnGUI()
    {
        try
        {
            betAmount = int.Parse(inputBet.text);
        }
        catch
        {
            betAmount = 0;
        }
        totalCredits.text = credits.ToString();
    }

    void checkBet()
    {
        if (firstReelResult == secondReelResult && secondReelResult == thirdReelResult)
        {
            betResult.text = "<color=#00FF00>JACKPOT!</color>";
            credits += betAmount * 50;
        }
        else if (firstReelResult == 0 && thirdReelResult == 0)
        {
            betResult.text = "<color=#00FF00>YOU WIN\n</color>" + (betAmount / 2).ToString();
            credits -= (betAmount / 2);
        }
        else if (firstReelResult == secondReelResult)
        {
            betResult.text = "<color=#FFFF00>ALMOST\nJACKPOT!</color>";
        }
        else if (firstReelResult == thirdReelResult)
        {
            betResult.text = "<color=#00FF00>YOU WIN\n</color>" + (betAmount * 2).ToString();
            credits -= (betAmount * 2);
        }
        else
        {
            betResult.text = "<color=#FF0000>YOU LOSE!</color>";
            credits -= betAmount;
        }

        if (credits <= 0)
        {
            _background.color = _gameOverColor;
            _slotMachine.SetActive(false);
            _gameOver.SetActive(true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (startSpin)
        {
            elapsedTime += Time.deltaTime;
            int randomSpinResult = UnityEngine.Random.Range(0, numberOfSym);
            if (!firstReelSpun)
            {
                firstReel.text = randomSpinResult.ToString();
                if (elapsedTime >= spinDuration)
                {
                    int weightedRandom = PickNumber();
                    firstReel.text = weightedRandom.ToString();
                    firstReelResult = weightedRandom;
                    firstReelSpun = true;
                    elapsedTime = 0;
                }
            }
            else if (!secondReelSpun)
            {
                secondReel.text = randomSpinResult.ToString();
                if (elapsedTime >= spinDuration)
                {
                    secondReelResult = randomSpinResult;
                    secondReelSpun = true;
                    elapsedTime = 0;
                }
            }
            else if (!thirdReelSpun)
            {
                thirdReel.text = randomSpinResult.ToString();
                if (elapsedTime >= spinDuration)
                {
                    if ((firstReelResult == secondReelResult) &&
                        randomSpinResult != firstReelResult)
                    {
                        //the first two reels have resulted the same symbol
                        //but unfortunately the third reel missed
                        //so instead of giving a random number we'll return a symbol which is one less than the other 2

                        randomSpinResult = firstReelResult - 1;
                        if (randomSpinResult < firstReelResult)
                            randomSpinResult = firstReelResult - 1;
                        if (randomSpinResult > firstReelResult)
                            randomSpinResult = firstReelResult + 1;
                        if (randomSpinResult < 0)
                            randomSpinResult = 0;
                        if (randomSpinResult > 9)
                            randomSpinResult = 9;

                        thirdReel.text = randomSpinResult.ToString();
                        thirdReelResult = randomSpinResult;
                    }
                    else
                    {
                        int weightedRandom = PickNumber();
                        thirdReel.text = weightedRandom.ToString();
                        thirdReelResult = weightedRandom;
                    }

                    startSpin = false;
                    elapsedTime = 0;
                    firstReelSpun = false;
                    secondReelSpun = false;

                    checkBet();
                }
            }
        }
    }

    private int PickNumber()
    {
        // Sum the weights of every state.
        var weightSum = weightedReelPoll.Sum(state => state.weight);
        var randomNumber = UnityEngine.Random.Range(0, weightSum);
        var i = 0;
        while (randomNumber >= 0)
        {
            var candidate = weightedReelPoll[i];
            randomNumber -= candidate.weight;
            if (randomNumber <= 0)
            {
                return candidate.number;
            }
            i++;
        }
        // It should not be possible to reach this point!
        throw new Exception("Something is wrong in the selectState algorithm!");
    }
}
