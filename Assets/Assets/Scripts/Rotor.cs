using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quadcopter
{
    public enum RotorType
    {
        NULL = 0,
        FRONT_LEFT,
        FRONT_RIGHT,
        BACK_LEFT,
        BACK_RIGHT
    }
    public enum RotorRotationDirection
    {
        NULL,
        CLOCKWISE,
        COUNTER_CLOCKWISE
    }

    public class Rotor : MonoBehaviour
    {
        // Init
        private bool _IsInit = false;

        // Type
        public RotorType _Type;
        private RotorRotationDirection _RotationDirection;

        // Reference to quadcopter main
        private Quadcopter.Main _Main;
        private Quadcopter.Settings _Settings;

        // rotation
        private float _Rotation = 0.0f;
        private float _RotationIncrease = 0.0f;

        private float _Speed = 0.0f;
        private float _SpeedRatio = 0.0f;
        public float GetSpeed()
        {
            return Mathf.Abs(_Speed);
        }
        public float GetSpeedRatio()
        {
            return _SpeedRatio;
        }

        // Setup
        public void Setup(Quadcopter.Main m)
        {
            _Main = m;
            _Settings = m._Settings;
            _IsInit = true;
        }

        private void UpdateRotationDirection()
        {
            bool R1 = (_Type == RotorType.FRONT_LEFT) || (_Type == RotorType.BACK_RIGHT);
            bool R2 = (_Type == RotorType.FRONT_RIGHT) || (_Type == RotorType.BACK_LEFT);

            //switch (_Main._States.GetVertical())
            //{
            //    case VerticalMovement.DEAD:
            //        {
            //            _RotationDirection = RotorRotationDirection.NULL;
            //        }
            //        break;
            //
            //    case VerticalMovement.STILL:
            //        {
            //            if (R1)
            //                _RotationDirection = RotorRotationDirection.CLOCKWISE;
            //            else if (R2)
            //                _RotationDirection = RotorRotationDirection.COUNTER_CLOCKWISE;
            //            else
            //                _RotationDirection = RotorRotationDirection.NULL;
            //        }
            //        break;
            //
            //    case VerticalMovement.UPWARDS:
            //        {
            //            _RotationDirection = RotorRotationDirection.COUNTER_CLOCKWISE;
            //        }
            //        break;
            //
            //    case VerticalMovement.DOWNWARDS:
            //        {
            //            _RotationDirection = RotorRotationDirection.CLOCKWISE;
            //        }
            //        break;
            //}
        }

        private void UpdateRotation()
        {
            if (!_Main.CheckPause())
            {
                // switch (_RotationDirection)
                // {
                //     case RotorRotationDirection.COUNTER_CLOCKWISE:
                //         _Speed = Mathf.Lerp(_Speed, +_Settings._RotorRotationSpeed, _Settings._RotorRotationChangespeed);
                //         break;
                // 
                //     case RotorRotationDirection.CLOCKWISE:
                //         _Speed = Mathf.Lerp(_Speed, -_Settings._RotorRotationSpeed, _Settings._RotorRotationChangespeed);
                //         break;
                // 
                //     case RotorRotationDirection.NULL:
                //         _Speed = Mathf.Lerp(_Speed, 0.0f, _Settings._RotorRotationChangespeed);
                //         break;
                // }
                // 
                //_Rotation += _Speed;
                // 
                // _SpeedRatio = Mathf.Clamp01(Mathf.Abs(_Speed) / _Settings._RotorRotationSpeed);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_IsInit)
            {
                UpdateRotationDirection();

                UpdateRotation();

                _Rotation += 1.0f;
                transform.localEulerAngles = new Vector3(0, _Rotation, 0);
            }
            else
            {
                Debug.LogError("Quadcopter Rotos has not been initialized");
            }
        }
    }
}