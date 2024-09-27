using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
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
    private bool firstReelSpinned = false;
    private bool secondReelSpinned = false;
    private bool thirdReelSpinned = false;
    private int betAmount;
    private int credits = 1000;
    private int firstReelResult = 0;
    private int secondReelResult = 0;
    private int thirdReelResult = 0;
    private float elapsedTime = 0.0f;

    [Header("Custom ADDED")]
    [SerializeField] private Image _background;
    [SerializeField] private GameObject _slotMachine;
    [SerializeField] private GameObject _gameOver;
    [SerializeField] private Color _gameOverColor;


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

    private void OnGUI()
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
            betResult.text = "<color=#00FF00>YOU WIN!</color>";
            credits += 500 * betAmount;
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

    void FixedUpdate()
    {
        if (startSpin)
        {
            elapsedTime += Time.deltaTime;
            int randomSpinResult = Random.Range(0, numberOfSym);
            if (!firstReelSpinned)
            {
                firstReel.text = randomSpinResult.ToString();
                if (elapsedTime >= spinDuration)
                {
                    firstReelResult = randomSpinResult;
                    firstReelSpinned = true;
                    elapsedTime = 0;
                }
            }
            else if (!secondReelSpinned)
            {
                secondReel.text = randomSpinResult.ToString();
                if (elapsedTime >= spinDuration)
                {
                    secondReelResult = randomSpinResult;
                    secondReelSpinned = true;
                    elapsedTime = 0;
                }
            }
            else if (!thirdReelSpinned)
            {
                thirdReel.text = randomSpinResult.ToString();
                if (elapsedTime >= spinDuration)
                {
                    thirdReelResult = randomSpinResult;
                    startSpin = false;
                    elapsedTime = 0;
                    firstReelSpinned = false;
                    secondReelSpinned = false;
                    checkBet();
                }
            }
        }
    }
}