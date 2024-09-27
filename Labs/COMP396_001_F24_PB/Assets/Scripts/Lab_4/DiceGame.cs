using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class DiceGame : MonoBehaviour
{
    public string inputValue = "1";
    public TMP_Text outputText;
    public TMP_InputField inputField;
    public Button button;

    [ContextMenu("Throw Dice")]
    int ThrowDice()
    {
        Debug.Log("Throwing dice...");
        Debug.Log("Finding random between 1 to 6...");
        int diceResult = Random.Range(1, 6);
        Debug.Log($"Result: {diceResult}");
        return diceResult;
    }

    public void ProcessGame()
    {
        inputValue = inputField.text;
        try
        {
            int inputInteger = int.Parse(inputValue);
            int totalSix = 0;
            for (var i = 0; i < 10; i++)
            {
                var diceResult = ThrowDice();
                if (diceResult == 6)
                {
                    totalSix++;
                }
                if (diceResult == inputInteger)
                {
                    outputText.text = $"DICE RESULT: {diceResult} \r\n<color=#00FF00>YOU WIN!</color>";
                }
                else
                {
                    outputText.text = $"DICE RESULT: {diceResult} \r\n<color=#FF0000>YOU LOSE!</color>";
                }
            }
            Debug.Log($"Total of six: {totalSix}");
        }
        catch
        {
            outputText.text = "Input is not a number!";
            Debug.LogError("Input is not a number!");
        }
    }
}