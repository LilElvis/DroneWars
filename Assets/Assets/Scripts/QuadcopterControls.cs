using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

namespace Quadcopter
{
    public enum AnalogPossibilities
    {
        NONE,
        LEFT_ANALOG_X,
        LEFT_ANALOG_Y,
        RIGHT_ANALOG_X,
        RIGHT_ANALOG_Y
    }

    public static class QuadcopterControls
    {
        public static float ANALOG_DEADZONE = 0.1f;

        private static float FixDeadZone(float value)
        {
            if (Mathf.Abs(value) < ANALOG_DEADZONE)
                return 0.0f;

            return value;
        }

        public static bool Pause()
        {
            return GamepadManager.GetKeyDown(PlayerIndex.One, GamepadButton.START) || Input.GetKeyDown(KeyCode.Return);
        }
        public static bool Dead()
        {
            return GamepadManager.GetKeyDown(PlayerIndex.One, GamepadButton.BACK) || Input.GetKeyDown(KeyCode.Backspace);
        }
        public static float GetAnalog(AnalogPossibilities A)
        {
            switch (A)
            {
                case AnalogPossibilities.LEFT_ANALOG_X:
                    return FixDeadZone(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X);

                case AnalogPossibilities.LEFT_ANALOG_Y:
                    return FixDeadZone(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y);

                case AnalogPossibilities.RIGHT_ANALOG_X:
                    return FixDeadZone(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X);

                case AnalogPossibilities.RIGHT_ANALOG_Y:
                    return FixDeadZone(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y);
            }

            // Error
            return 0.0f;
        }

        public static bool ResetOrientation()
        {
            return GamepadManager.GetKeyDown(PlayerIndex.One, GamepadButton.A);
        }
    }
}
