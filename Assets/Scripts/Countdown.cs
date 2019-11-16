using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    Text countdownText;

    void Start()
    {
        countdownText = GetComponentInChildren<Text>();
    }
    public void StartCountdown_LinkedToButton()
    {
        countdownText.text = "";
    }
    IEnumerator StartCountdown()
    {
        countdownText.text = "3...";
        yield return new WaitForSeconds(.5f);
        countdownText.text += "2...";
        yield return new WaitForSeconds(.5f);
        countdownText.text += "1...";
        yield return new WaitForSeconds(.5f);
    }
}
