using UnityEngine;
using UnityEngine.XR;

public class ToggleVR : MonoBehaviour
{
    //Example of toggling VRSettings
    private void Update()
    {
        //If V is pressed, toggle VRSettings.enabled
        if (Input.GetKeyDown(KeyCode.V))
        {
            XRSettings.enabled = !XRSettings.enabled;
            Debug.Log("Changed VRSettings.enabled to:" + XRSettings.enabled);
        }
    }
}