using System;
using System.Linq;

using TMPro;

using UnityEngine;

public class FSM : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resultText;

    [Serializable]
    public enum FSMState
    {
        Chase,
        Flee,
        SelfDestruct,
    }
    [Serializable]
    public struct FSMProbability
    {
        public FSMState state;
        public int weight;
    }
    public FSMProbability[] states;
    FSMState selectState()
    {
        // Sum the weights of every state.
        var weightSum = states.Sum(state => state.weight);
        var randomNumber = UnityEngine.Random.Range(0, weightSum);
        var i = 0;
        while (randomNumber >= 0)
        {
            var state = states[i];
            randomNumber -= state.weight;
            if (randomNumber <= 0)
            {
                return state.state;
            }
            i++;
        }
        // It is not possible to reach this point!
        throw new Exception("Something is wrong in the selectState algorithm!");
    }

    private void Start()
    {
        _resultText.text = "<color=#FF0000>STATE: </color> \n Press SPACE or Click Below to Set!";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetState();
        }
    }

    public void GetState()
    {
        FSMState randomState = selectState();
        Debug.Log(randomState.ToString());

        _resultText.text = "<color=#30D5C8>STATE: </color>" + randomState.ToString().ToUpper();
    }
}
