using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class GamepadButtonMemory
{
    public XInputDotNetPure.ButtonState[] States = new XInputDotNetPure.ButtonState[11];

    public XInputDotNetPure.ButtonState[] Previous_States = new XInputDotNetPure.ButtonState[11];

    public void Reset()
    {
        for (int i = 0; i < 11; i++)
        {
            States[i] = ButtonState.Released;
            Previous_States[i] = ButtonState.Released;
        }
    }
    public GamepadButtonMemory()
    {
        Reset();
    }
}
public class GamepadDpadMemory
{
    public XInputDotNetPure.ButtonState[] States = new XInputDotNetPure.ButtonState[4];

    public XInputDotNetPure.ButtonState[] Previous_States = new XInputDotNetPure.ButtonState[4];

    public void Reset()
    {
        for (int i = 0; i < 4; i++)
        {
            States[i] = ButtonState.Released;
            Previous_States[i] = ButtonState.Released;
        }
    }
    public GamepadDpadMemory()
    {
        Reset();
    }
}

public enum GamepadButton
{
    START,
    BACK,
    LEFTSTICK,
    RIGHTSTICK,
    LEFTSHOULDER,
    RIGHTSHOULDER,
    GUIDE,
    A,
    B,
    X,
    Y,
}
public enum GamepadDpadButton
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}


public static class GamepadManager
{
    private static bool _IsSetup = false;

    private static GamepadButtonMemory[] _ButtonData = new GamepadButtonMemory[4];
    private static GamepadDpadMemory[] _DpadData = new GamepadDpadMemory[4];

    private static GamepadButtonMemory _AllButtonData = new GamepadButtonMemory();
    private static GamepadDpadMemory _AllDpadData = new GamepadDpadMemory();

    private static GamepadButtonMemory GetButton(int index)
    {
        if (_ButtonData[index] == null)
            _ButtonData[index] = new GamepadButtonMemory();

        return _ButtonData[index];
    }
    private static GamepadDpadMemory GetDpad(int index)
    {
        if (_DpadData[index] == null)
            _DpadData[index] = new GamepadDpadMemory();

        return _DpadData[index];
    }

    // Controller Button Press
    public static bool GetKey(PlayerIndex controller_index, GamepadButton button)
    {
        return (GetButton((int)controller_index).States[(int)button] == XInputDotNetPure.ButtonState.Pressed);
    }
    public static bool GetKeyDown(PlayerIndex controller_index, GamepadButton button)
    {
        bool Now = (GetButton((int)controller_index).States[(int)button] == XInputDotNetPure.ButtonState.Pressed);
        bool Prev = (GetButton((int)controller_index).Previous_States[(int)button] == XInputDotNetPure.ButtonState.Pressed);

        return (Now == true && Prev == false);
    }
    public static bool GetKeyUp(PlayerIndex controller_index, GamepadButton button)
    {
        bool Now = (GetButton((int)controller_index).States[(int)button] == XInputDotNetPure.ButtonState.Pressed);
        bool Prev = (GetButton((int)controller_index).Previous_States[(int)button] == XInputDotNetPure.ButtonState.Pressed);

        return (Now == false && Prev == true);
    }
    // ANY Controller Button Press
    public static bool GetKey(GamepadButton button)
    {
        return (_AllButtonData.States[(int)button] == XInputDotNetPure.ButtonState.Pressed);
    }
    public static bool GetKeyDown(GamepadButton button)
    {
        bool Now = (_AllButtonData.States[(int)button] == XInputDotNetPure.ButtonState.Pressed);
        bool Prev = (_AllButtonData.Previous_States[(int)button] == XInputDotNetPure.ButtonState.Pressed);

        return (Now == true && Prev == false);
    }
    public static bool GetKeyUp(GamepadButton button)
    {
        bool Now = (_AllButtonData.States[(int)button] == XInputDotNetPure.ButtonState.Pressed);
        bool Prev = (_AllButtonData.Previous_States[(int)button] == XInputDotNetPure.ButtonState.Pressed);

        return (Now == false && Prev == true);
    }

    public static bool GetKey(PlayerIndex controller_index, GamepadDpadButton button)
    {
        return (GetDpad((int)controller_index).States[(int)button] == XInputDotNetPure.ButtonState.Pressed);
    }
    public static bool GetKeyDown(PlayerIndex controller_index, GamepadDpadButton button)
    {
        bool Now = (GetDpad((int)controller_index).States[(int)button] == XInputDotNetPure.ButtonState.Pressed);
        bool Prev = (GetDpad((int)controller_index).Previous_States[(int)button] == XInputDotNetPure.ButtonState.Pressed);

        return (Now == true && Now != Prev);
    }
    public static bool GetKeyUp(PlayerIndex controller_index, GamepadDpadButton button)
    {
        bool Now = (GetDpad((int)controller_index).States[(int)button] == XInputDotNetPure.ButtonState.Pressed);
        bool Prev = (GetDpad((int)controller_index).Previous_States[(int)button] == XInputDotNetPure.ButtonState.Pressed);

        return (Now == false && Now != Prev);
    }

    public static bool GetKey(GamepadDpadButton button)
    {
        return (_AllDpadData.States[(int)button] == XInputDotNetPure.ButtonState.Pressed);
    }
    public static bool GetKeyDown(GamepadDpadButton button)
    {
        bool Now = (_AllDpadData.States[(int)button] == XInputDotNetPure.ButtonState.Pressed);
        bool Prev = (_AllDpadData.Previous_States[(int)button] == XInputDotNetPure.ButtonState.Pressed);

        return (Now == true && Now != Prev);
    }
    public static bool GetKeyUp(GamepadDpadButton button)
    {
        bool Now = (_AllDpadData.States[(int)button] == XInputDotNetPure.ButtonState.Pressed);
        bool Prev = (_AllDpadData.Previous_States[(int)button] == XInputDotNetPure.ButtonState.Pressed);

        return (Now == false && Now != Prev);
    }

    public static void Update()
    {
        // Reset all states
        _AllButtonData.Reset();
        _AllDpadData.Reset();

        for (int i = 0; i < 4; i++)
        {
            if(GamePad.GetState((PlayerIndex)i).IsConnected)
            {
                if (_ButtonData[i] == null)
                    _ButtonData[i] = new GamepadButtonMemory();

                if (_DpadData[i] == null)
                    _DpadData[i] = new GamepadDpadMemory();

                // Copy current states into previous ones
                if (_IsSetup)
                {
                    System.Array.Copy(_ButtonData[i].States, _ButtonData[i].Previous_States, 11);
                    System.Array.Copy(_DpadData[i].States, _DpadData[i].Previous_States, 4);
                }
                else
                {
                    _IsSetup = true;
                }

                // Update all states of controller
                _ButtonData[i].States[0] = GamePad.GetState((PlayerIndex)i).Buttons.Start;
                _ButtonData[i].States[1] = GamePad.GetState((PlayerIndex)i).Buttons.Back;
                _ButtonData[i].States[2] = GamePad.GetState((PlayerIndex)i).Buttons.LeftStick;
                _ButtonData[i].States[3] = GamePad.GetState((PlayerIndex)i).Buttons.RightStick;
                _ButtonData[i].States[4] = GamePad.GetState((PlayerIndex)i).Buttons.LeftShoulder;
                _ButtonData[i].States[5] = GamePad.GetState((PlayerIndex)i).Buttons.RightShoulder;
                _ButtonData[i].States[6] = GamePad.GetState((PlayerIndex)i).Buttons.Guide;
                _ButtonData[i].States[7] = GamePad.GetState((PlayerIndex)i).Buttons.A;
                _ButtonData[i].States[8] = GamePad.GetState((PlayerIndex)i).Buttons.B;
                _ButtonData[i].States[9] = GamePad.GetState((PlayerIndex)i).Buttons.X;
                _ButtonData[i].States[10] = GamePad.GetState((PlayerIndex)i).Buttons.Y;
                for(int j = 0; j < 11; j++)
                {
                    if (_ButtonData[i].States[j] == ButtonState.Pressed)
                        _AllButtonData.States[j] = ButtonState.Pressed;

                    if (_ButtonData[i].Previous_States[j] == ButtonState.Pressed)
                        _AllButtonData.Previous_States[j] = ButtonState.Pressed;
                }


                _DpadData[i].States[0] = GamePad.GetState((PlayerIndex)i).DPad.Up;
                _DpadData[i].States[1] = GamePad.GetState((PlayerIndex)i).DPad.Down;
                _DpadData[i].States[2] = GamePad.GetState((PlayerIndex)i).DPad.Left;
                _DpadData[i].States[3] = GamePad.GetState((PlayerIndex)i).DPad.Right;
                for (int j = 0; j < 4; j++)
                {
                    if (_DpadData[i].States[j] == ButtonState.Pressed)
                        _AllDpadData.States[j] = ButtonState.Pressed;

                    if (_DpadData[i].Previous_States[j] == ButtonState.Pressed)
                        _AllDpadData.Previous_States[j] = ButtonState.Pressed;
                }
            }
        }
    }
}
