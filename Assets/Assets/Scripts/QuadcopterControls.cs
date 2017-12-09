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

        public static bool ResetLevel()
        {
            return GamepadManager.GetKeyDown(PlayerIndex.One, GamepadButton.Y);
        }

        public static bool MenuUp()
        {
            if (
                GamepadManager.GetKeyDown(PlayerIndex.One, GamepadDpadButton.UP) ||
                GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > +ANALOG_DEADZONE ||
                GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y > +ANALOG_DEADZONE
                )
                return true;

            return false;
        }
        public static bool MenuDown()
        {
            if (
                GamepadManager.GetKeyDown(PlayerIndex.One, GamepadDpadButton.DOWN) ||
                GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < -ANALOG_DEADZONE ||
                GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y < -ANALOG_DEADZONE
                )
                return true;

            return false;
        }

        public static bool MenuLeft()
        {
            if (
                GamepadManager.GetKeyDown(PlayerIndex.One, GamepadDpadButton.LEFT) ||
                GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < -ANALOG_DEADZONE ||
                GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X < -ANALOG_DEADZONE
                )
                return true;

            return false;
        }
        public static bool MenuRight()
        {
            if (
                GamepadManager.GetKeyDown(PlayerIndex.One, GamepadDpadButton.RIGHT) ||
                GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > +ANALOG_DEADZONE ||
                GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X > +ANALOG_DEADZONE
                )
                return true;

            return false;
        }

        public static bool PausePreviousPage()
        {
            return GamepadManager.GetKeyDown(PlayerIndex.One, GamepadButton.LEFTSHOULDER);
        }
        public static bool PauseNextPage()
        {
            return GamepadManager.GetKeyDown(PlayerIndex.One, GamepadButton.RIGHTSHOULDER);
        }
    }
}
