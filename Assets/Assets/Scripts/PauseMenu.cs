using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    //private static PauseMenu instance = null;
    //public static PauseMenu GetInstance()
    //{
    //    if (instance == null) instance = FindObjectOfType<PauseMenu>();
    //    return instance;
    //}

    private static string[] _Page1 =
    {
        "Camera Pitch",
        "Camera FOV",
        "Wind Mode",
        "Global Speed Modifier",
        "Rest Mode"
    };

    private static string[] _Page2 =
    {
        "Thruster Speed",
        "Set Thruster Axis",
        "Inverse Thruster"
    };
    private static string[] _Page3 =
    {
        "Pitch Speed",
        "Yaw Speed",
        "Roll Speed",
        "Set Pitch Axis",
        "Set Yaw Axis",
        "Set Roll Axis",
        "Inverse Pitch",
        "Inverse Yaw",
        "Inverse Roll"
    };

    public Text _SettingsName = null;
    private void UpdateNames()
    {
        if(_SettingsName != null)
        {
            string text = "";

            for(int i = 0; i < _Pages[_PageIndex].Length; i++)
            {
                text += _Pages[_PageIndex][i] + '\n';
            }

            _SettingsName.text = text;
        }
    }
    public Text _SettingsData = null;
    private bool Left = false;
    private bool Right = false;
    private void AddText(ref string original, bool b)
    {
        original += "< " + ((b == true) ? "ON" : "OFF") + " >";
        original += '\n';
    }
    private void AddText(ref string original, float f, int decimals = 0, bool Degrees = false)
    {
        string D = "F" + decimals.ToString();
        original += "< " + f.ToString(D) + ((Degrees == true) ? "°" : "") + " >";
        original += '\n';
    }
    private void AddText(ref string original, string s)
    {
        original += "< " + s + " >\n";
    }
    private void UpdateValue(ref float value, float amount, float min, float max)
    {
        if (Left)
            value -= amount;
        else if (Right)
            value += amount;

        value = Mathf.Clamp(value, min, max);

    }
    private void UpdateValue(ref bool value)
    {
        if (Left)
            value = false;
        else if (Right)
            value = true;
    }
    private void UpdateSettings()
    {
        Quadcopter.Settings S = _Main._Settings;

        // Menu Left / Right
        int Direction = 0;
        if (Quadcopter.QuadcopterControls.MenuLeft())
            Direction -= 1;
        if (Quadcopter.QuadcopterControls.MenuRight())
            Direction += 1;

        Left = (Direction < 0);
        Right = (Direction > 0);

        string text = "";

        switch (_PageIndex)
        {
            case 0:
                {
                    float Pitch = S._CameraPitchAngle;
                    float FOV = S._CameraFOV;
                    WindControl Wind = WindGlobal.GetInstance()._Control;
                    float GlobalSpeed = S._GlobalSpeedModifier;
                    bool RestMode = S._ThrusterRest;

                    AddText(ref text, Pitch, 1, true);
                    AddText(ref text, FOV, 1, true);
                    AddText(ref text, Wind.ToString());
                    AddText(ref text, GlobalSpeed);
                    AddText(ref text, RestMode);

                    // Controls
                    switch(_SelectionIndex)
                    {
                        case 0:
                            UpdateValue(ref S._CameraPitchAngle, 1.0f, -45.0f, 45.0f);
                            S.UpdateCameraPitch();
                            break;

                        case 1:
                            UpdateValue(ref S._CameraFOV, 1.0f, 90.0f, 170.0f);
                            S.UpdateFOV();
                            break;

                        case 2:
                            break;

                        case 3:
                            UpdateValue(ref S._GlobalSpeedModifier, 0.01f, 0.0f, 1.0f);
                            break;

                        case 4:
                            UpdateValue(ref S._ThrusterRest);
                            break;
                    }
                }
                break;

            case 1:
                {
                    float T_S = S._ThrusterSpeed;
                    Quadcopter.AnalogPossibilities T_C = S._ThrusterControl;
                    bool T_I = S._InverseThrusters;

                    AddText(ref text, T_S, 2);
                    AddText(ref text, T_C.ToString());
                    AddText(ref text, T_I);

                    switch (_SelectionIndex)
                    {
                        case 0:
                            UpdateValue(ref S._ThrusterSpeed, 0.02f, 0.0f, 5.0f);
                            break;

                        case 1:
                            break;

                        case 2:
                            UpdateValue(ref S._InverseThrusters);
                            break;
                    }
                }
                break;

            case 2:
                {
                    float Pitch_S = S._PitchSpeed;
                    float Yaw_S = S._YawSpeed;
                    float Roll_S = S._RollSpeed;

                    Quadcopter.AnalogPossibilities Pitch_C = S._YawControl;
                    Quadcopter.AnalogPossibilities Yaw_C = S._YawControl;
                    Quadcopter.AnalogPossibilities Roll_C = S._YawControl;

                    bool Pitch_I = S._InversePitch;
                    bool Yaw_I = S._InverseYaw;
                    bool Roll_I = S._InverseRoll;

                    AddText(ref text, Pitch_S, 2);
                    AddText(ref text, Yaw_S, 2);
                    AddText(ref text, Roll_S, 2);
                    AddText(ref text, Pitch_C.ToString());
                    AddText(ref text, Yaw_C.ToString());
                    AddText(ref text, Roll_C.ToString());
                    AddText(ref text, Pitch_I);
                    AddText(ref text, Yaw_I);
                    AddText(ref text, Roll_I);

                    switch (_SelectionIndex)
                    {
                        case 0:
                            UpdateValue(ref S._PitchSpeed, 0.02f, 0.0f, 5.0f);
                            break;

                        case 1:
                            UpdateValue(ref S._YawSpeed, 0.02f, 0.0f, 5.0f);
                            break;

                        case 2:
                            UpdateValue(ref S._RollSpeed, 0.02f, 0.0f, 5.0f);
                            break;

                        case 6:
                            UpdateValue(ref S._InversePitch);
                            break;

                        case 7:
                            UpdateValue(ref S._InverseYaw);
                            break;

                        case 8:
                            UpdateValue(ref S._InverseRoll);
                            break;

                    }
                }
                break;
        }

        _SettingsData.text = text;
    }

    public RectTransform _Selection = null;
    public float _SelectionYFirst = 0.0f;
    public float _SelectionYDiff = 0.0f;
    private void UpdateSelection()
    {
        if(_Selection != null)
        {
            float y = _SelectionYFirst - _SelectionYDiff * _SelectionIndex;

            var V = _Selection.transform.localPosition;
            V.y = y;
            _Selection.transform.localPosition = V;
        }
    }

    private static string[][] _Pages = {_Page1, _Page2, _Page3};
    private int _PageIndex = 0;
    private int _SelectionIndex = 0;

    private Quadcopter.Main _Main = null;
    private bool _IsInit = false;

    public void Setup(Quadcopter.Main M)
    {
        _Main = M;
        _IsInit = true;
    }

    private void Update()
    {
        if(_IsInit)
        {
            // Update Text
            UpdateNames();
            UpdateSettings();

            // Page Selection
            if (Quadcopter.QuadcopterControls.PauseNextPage())
                _PageIndex++;
            else if (Quadcopter.QuadcopterControls.PausePreviousPage())
                _PageIndex--;

            // Clamp Page Index
            if (_PageIndex >= _Pages.Length)
                _PageIndex -= _Pages.Length;
            if (_PageIndex < 0)
                _PageIndex = _Pages.Length - 1;

            // Update Selection
            UpdateSelection();

            // Selection Change
            if (Quadcopter.QuadcopterControls.MenuUp())
                _SelectionIndex--;
            else if (Quadcopter.QuadcopterControls.MenuDown())
                _SelectionIndex++;

            // Clamp Selection Index
            if (_SelectionIndex >= _Pages[_PageIndex].Length)
                _SelectionIndex -= _Pages[_PageIndex].Length;
            if (_SelectionIndex < 0)
                _SelectionIndex = _Pages[_PageIndex].Length - 1;

        }
    }

}
