using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class TimerScript : MonoBehaviour 
{
	Text text;
    public float time = 0.0f;
    bool timeActive = false;

    Color _timerColorOn = Color.green;
    Color _timerColorPaused = Color.yellow;
    Color _timerColorZero = Color.white;

    // Use this for initialization
    void Start () 
	{
		text = GetComponent<Text>();
        text.color = _timerColorZero;
    }

    // Update is called once per frame
    void Update()
    {
        if(GamepadManager.GetKeyDown(PlayerIndex.One, GamepadButton.LEFTSHOULDER))
        {
            timeActive = !timeActive;

            if(timeActive)
                text.color = _timerColorOn;
            else
                text.color = _timerColorPaused;
        }

        if (GamepadManager.GetKeyDown(PlayerIndex.One, GamepadButton.RIGHTSHOULDER))
        {
            timeActive = false;
            time = 0.0f;
            text.color = _timerColorZero;
            
        }

        //if(GamepadManager.GetKey(PlayerIndex.One, GamepadButton.RIGHTSHOULDER) &&
        //    GamepadManager.GetKey(PlayerIndex.One, GamepadButton.LEFTSHOULDER))
        //{
        //    time = 0.0f;
        //    //timeActive = false;
        //    text.color = _timerColorZero;
        //}


        if (timeActive)
            time += Time.deltaTime;
                
        float msec = Mathf.Floor((time * 100) % 100); // * 1000.0f;
        float seconds = Mathf.Floor(time % 60);
        float minutes = Mathf.Floor((time / 60) % 60);
        float hours = Mathf.Floor((time / 3600) % 60);

        string milliStr;
        string secondsStr;
        string minutesStr;
        string hoursStr;

        string blinkingColon = ":";
        string blinkingDot = ".";

        if (msec > 50)
        {
            blinkingColon = " ";
            blinkingDot = " ";
        }


        hoursStr = DoubleDigitConvert(hours);
        minutesStr = DoubleDigitConvert(minutes);
        secondsStr = DoubleDigitConvert(seconds);
        milliStr = DoubleDigitConvert(msec);

        string frameNumber = DoubleDigitConvert((Time.time - Mathf.Floor(Time.time)) * 30.0f);

        //string str = string.Format("{00}", hoursStr);

        //text.text = string.Format("{00}", (int)time) + blinkingDot +
        //    string.Format("{00}", (int)(Mathf.Repeat(time, 1.0f) * 100));

        text.text =
            //string.Format("{00}", hoursStr) + blinkingColon +
            string.Format("{00}", minutesStr) + blinkingColon +
            string.Format("{00}", secondsStr) + blinkingDot +
            string.Format("{00}", milliStr)
            + string.Format("  [{00}]", frameNumber);
    }

    string DoubleDigitConvert(float num)
    {
        if (num < 10)
            return "0" + Mathf.RoundToInt(Mathf.Floor(num)).ToString();
        else
            return Mathf.RoundToInt(Mathf.Floor(num)).ToString();
    }
}
