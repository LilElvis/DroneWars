using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class UIToggle : MonoBehaviour
{
    Canvas canvas;

	// Use this for initialization
	void Start ()
    {
        canvas = GetComponent<Canvas>();

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (GamepadManager.GetKeyDown(PlayerIndex.One, GamepadButton.BACK))
        {
            canvas.enabled = !canvas.enabled;
        }

    }
}
