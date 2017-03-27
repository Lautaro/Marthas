using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    public Text lifeCounter;
    public MessageDisplay messageDisplay;
    public Text energyBallCounter;
    public static GUI Me;

    // Use this for initialization
    void Awake()
    {
        if (Me != null)
        {
            Debug.LogError("There should only be one GUI scrip in scene");
        }
        else
        {
            Me = this;
        }
    }

    public void SetLifeCounter(int life)
    {
        lifeCounter.text = "Life: " + life;

    }

    public void SetEnergyBallCounter(int amount)
    {
        energyBallCounter.text = "Energy Balls: " + amount;

    }

    public void ShowPresentation(string presentation)
    {

        messageDisplay.DisplayGuiMessage(presentation, Color.cyan);
    
    }

}
