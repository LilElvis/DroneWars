using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quadcopter
{
    [System.Serializable]
    public class Settings
    {

        public readonly static float _Gravity = -9.81f;

        [Space]
        [Header("Global:")]
        public float _GravityModifier = 1.0f;
        public float _RotorSpeed_Aesthetic = 10.0f;
        [Range(0.0f, 1.0f)]
        public float _RotorSpeedTransition_Aesthetic = 0.2f;

        public bool _ThrusterRest = false;

        [Space]
        [Header("Thrusters:")]
        //[Range(0.0001f, 90.0f)]
        public float _ThrusterSpeedUpwards = 1.0f;
        public float _ThrusterSpeedDownwards = 1.0f;
        [Range(0.0f, 1.0f)]
        public float _ThrusterTransition_Upwards = 0.2f;
        [Range(0.0f, 1.0f)]
        public float _ThrusterTransition_Downwards = 0.1f;
        public bool _InverseThrusters = false;
        public AnalogPossibilities _ThrusterControl = AnalogPossibilities.LEFT_ANALOG_Y;

        [Space]
        [Header("Pitch:")]
        [Range(0.0001f, 90.0f)]
        public float _PitchSpeed = 0.1f;
       // [Range(0.0f, 1.0f)]
        //public float _PitchTransition = 0.05f;
        public bool _InversePitch = false;
        public AnalogPossibilities _PitchControl = AnalogPossibilities.RIGHT_ANALOG_Y;

        [Space]
        [Header("Yaw:")]
        [Range(0.0001f, 90.0f)]
        public float _YawSpeed = 0.1f;
        //[Range(0.0f, 1.0f)]
        //public float _YawTransition = 0.05f;
        public bool _InverseYaw = false;
        public AnalogPossibilities _YawControl = AnalogPossibilities.LEFT_ANALOG_X;

        [Space]
        [Header("Roll:")]
        [Range(0.0001f, 90.0f)]
        public float _RollSpeed = 0.1f;
        //[Range(0.0f, 1.0f)]
        //public float _RollTransition = 0.05f;
        public bool _InverseRoll = false;
        public AnalogPossibilities _RollControl = AnalogPossibilities.RIGHT_ANALOG_X;


    }

    public class Rotors
    {
        // All Rotors
        private Rotor[] _Rotors = null;
        public int Amount()
        {
            return _Rotors.Length;
        }

        // Acquire Rotors and Set them up
        public bool Setup(Quadcopter.Main M)
        {
            _Rotors = GameObject.FindObjectsOfType(typeof(Rotor)) as Rotor[];

            if (_Rotors == null)
            {
                Debug.LogError("You need at least one rotor for the quadcopter to setup");
                return false;
            }

            for (int i = 0; i < _Rotors.Length; i++)
            {
                _Rotors[i].Setup(M);
            }
            return true;
        } 

        //  // Get Data
        //  public float GetRotorSpeed(int index)
        //  {
        //      return _Rotors[index].GetSpeed();
        //  }
        //  public float GetOverallSpeed()
        //  {
        //      if (_Rotors == null)
        //          return 0.0f;
        //  
        //      float total = 0.0f;
        //  
        //      for (int i = 0; i < _Rotors.Length; i++)
        //      {
        //          total += GetRotorSpeed(i);
        //      }
        //      return total / (float)_Rotors.Length;
        //  }
        //  public float GetRotorSpeedRatio(int index)
        //  {
        //      return _Rotors[index].GetSpeedRatio();
        //  }
        //  public float GetOverallSpeedRatio()
        //  {
        //      if (_Rotors == null)
        //          return 0.0f;
        //  
        //      float total = 0.0f;
        //  
        //      for (int i = 0; i < _Rotors.Length; i++)
        //      {
        //          total += GetRotorSpeedRatio(i);
        //      }
        //      return total / (float)_Rotors.Length;
        //  }
    }

    // Enums
    public enum ThrusterState
    {
        OFF,
        STILL,
        UPWARDS,
        DOWNWARDS
    }
    public enum YawRotation
    {
        NONE,
        CW,
        CCW
    }
    public enum PitchRotation
    {
        NONE,
        CW,
        CCW
    }
    public enum RollRotation
    {
        NONE,
        CW,
        CCW
    }



    // State
    public class States
    {
        // States
        private ThrusterState thrusterState = ThrusterState.STILL;
        private YawRotation yawrotation = YawRotation.NONE;
        private PitchRotation pitchrotation = PitchRotation.NONE;
        private RollRotation rollrotation = RollRotation.NONE;
        private Vector3 eulerchanges = Vector3.zero;
        private float thrusters = 0.0f;

        public ThrusterState GetThrusterState()
        {
            return thrusterState;
        }
        public YawRotation GetYaw()
        {
            return yawrotation;
        }
        public PitchRotation GetPitch()
        {
            return pitchrotation;
        }
        public RollRotation GetRoll()
        {
            return rollrotation;
        }
        public Vector3 GetEulerChanges()
        {
            return eulerchanges;
        }
        public float GetThrusters()
        {
            return thrusters;
        }
        public float GetThrusterUpwards()
        {
            return Mathf.Max(0.0f, thrusters);
        }
        public float GetThrusterDownwards()
        {
            return Mathf.Abs(Mathf.Min(0.0f, thrusters));
        }

        public void UpdateStates(ref Quadcopter.Settings Settings)
        {
            // Thrusters
            thrusters = QuadcopterControls.GetAnalog(Settings._ThrusterControl);

            if (thrusters < 0.0f)
                thrusterState = ThrusterState.DOWNWARDS;
            else if (thrusters > 0.0f)
                thrusterState = ThrusterState.UPWARDS;
            else if (Settings._ThrusterRest)
                thrusterState = ThrusterState.STILL;
            else
                thrusterState = ThrusterState.OFF;

            // Pitch
            float Pitch = QuadcopterControls.GetAnalog(Settings._PitchControl);

            if (Pitch < 0.0f)
                pitchrotation = PitchRotation.CCW;
            else if (Pitch > 0.0f)
                pitchrotation = PitchRotation.CW;
            else
                pitchrotation = PitchRotation.NONE;

            // Yaw
            float Yaw = QuadcopterControls.GetAnalog(Settings._YawControl);

            if (Yaw < 0.0f)
                yawrotation = YawRotation.CCW;
            else if (Yaw > 0.0f)
                yawrotation = YawRotation.CW;
            else
                yawrotation = YawRotation.NONE;

            // Roll
            float Roll = QuadcopterControls.GetAnalog(Settings._RollControl);

            if (Roll < 0.0f)
                rollrotation = RollRotation.CCW;
            else if (Roll > 0.0f)
                rollrotation = RollRotation.CW;
            else
                rollrotation = RollRotation.NONE;

            // Return Euler Changes
            eulerchanges = new Vector3(Pitch, Yaw, Roll);
        }

    }

    public class Physics
    {
        // Rigidbody Reference
        private Rigidbody _Rigidbody = null;
        public bool Setup(GameObject Object)
        {
            _Rigidbody = Object.GetComponent<Rigidbody>();

            return (_Rigidbody != null);
        }
        public void SetVelocity(Vector3 v)
        {
            _Rigidbody.velocity = v;
        }
        public Vector3 GetVelocity()
        {
            return _Rigidbody.velocity;
        }

        // Position
        private Vector3 Position = Vector3.zero;
        public void UpdatePosition()
        {
            Position = _Rigidbody.transform.position;
        }

        // Quadcopter Axis
        private Vector3 Quadcopter_Up = Vector3.zero;
        private Vector3 Quadcopter_Right = Vector3.zero;
        private Vector3 Quadcopter_Forward = Vector3.zero;
        public void UpdateAxis()
        {
            Quadcopter_Up = _Rigidbody.gameObject.transform.up.normalized;
            Quadcopter_Right = _Rigidbody.gameObject.transform.right.normalized;
            Quadcopter_Forward = _Rigidbody.gameObject.transform.forward.normalized;

            // If close enough to be up, stay up
            if (Vector3.Distance(Quadcopter_Up, Vector3.up) < 0.1f)
                Quadcopter_Up = Vector3.up;
        }

        // Speed
        private float Speed_Current = 0.0f;
        public float GetCurrentSpeed()
        {
            return Speed_Current;
        }
        // Ideal Speed
        private float Speed_Ideal = 0.0f;
        
        // Update Position
        public void UpdatePosition(ref Quadcopter.States State, ref Quadcopter.Settings Settings)
        {
            // Float Thruster amount
            //float ThrusterAmount = State.GetThrusterUpwards();
            float ThrusterAmount = State.GetThrusters();
            // Inverse Setting
            if (Settings._InverseThrusters)
                ThrusterAmount = 1.0f - ThrusterAmount;

            // Gravity
            Vector3 Gravity = new Vector3(0, Settings._Gravity * Settings._GravityModifier, 0);

            // Rotor Speed Ideal
            if(ThrusterAmount >= 0.0f)
                Speed_Ideal = ThrusterAmount * Settings._ThrusterSpeedUpwards;
            else
                Speed_Ideal = ThrusterAmount * Settings._ThrusterSpeedDownwards;

            // If thrusters are being used
            if (ThrusterAmount > 0.0f)
            {
                // You move at least as the same speed as gravity
                Speed_Current = Mathf.Max(Speed_Current, -Gravity.y * 0.88f);
            }



            // Rest Stat
            bool RestSate = (ThrusterAmount == 0.0f && Settings._ThrusterRest == true);

            // Thruster rest
            if (RestSate)
            {
                // Lerp speed to rest
                Speed_Current = Mathf.Lerp(Speed_Current, -Gravity.y, 0.07f);

                // If close enough, snap to gravity speed
                if (Mathf.Abs(Speed_Current + Gravity.y) < 0.01f)
                    Speed_Current = -Gravity.y;
            }
            // Normal Transition
            else
            {
                // Lerp Speed to Ideal
                if (Speed_Current < Speed_Ideal)
                {
                    // Going Up
                    Speed_Current = Mathf.Lerp(Speed_Current, Speed_Ideal, Settings._ThrusterTransition_Upwards);
                }
                else
                {
                    // Going Down
                    Speed_Current = Mathf.Lerp(Speed_Current, Speed_Ideal, Settings._ThrusterTransition_Downwards);
                }
            }


            // Final Direction
            Vector3 FinalDirection = Gravity + Quadcopter_Up * Speed_Current;
            // If at rest, make sure y value clamps if close enough to zero
            if (RestSate && Mathf.Abs(FinalDirection.y) < 0.01f)
            {
                FinalDirection.y = 0.0f;
            }

            


            // Set Velocity
            SetVelocity(FinalDirection);

            //  // Calculated Speed
            //  Speed_Ideal = State.GetThrusterUpwards() * Settings._ThrusterSpeed * ((Settings._InverseThrusters) ? -1.0f : 1.0f);
            //  
            //  // Going upwards at all immediately combats Gravity
            //  //if (Speed_Ideal > 0.0f)
            //  //{
            //  //    Speed_Ideal += Mathf.Abs(Gravity.y);
            //  //}
            //  
            //  // Adjust Speed Based on going up or down
            //  if (Speed < Speed_Ideal) // Going up
            //      Speed = Mathf.Lerp(Speed, Speed_Ideal, Settings._ThrusterTransition_Upwards);
            //  else // Going down
            //      Speed = Mathf.Lerp(Speed, Speed_Ideal, Settings._ThrusterTransition_Downwards);
            //  
            //  Vector3 Direction = Quadcopter_Up * Speed + Gravity;
            //  
            //  
            //  // DEBUG
            //  Debug.DrawLine(_Rigidbody.gameObject.transform.position, _Rigidbody.gameObject.transform.position + Direction, Color.white);
            //  // End
            //  //_Rigidbody.velocity = Direction;

        }


        private float Pitch_Change = 0.0f; // X
        private float Yaw_Change = 0.0f; // Y
        private float Roll_change = 0.0f; // Z

        // Update Rotation Stuff (Returns Final Eulers)
        public void UpdateRotationEuler(ref Quadcopter.States State, ref Quadcopter.Settings Settings)
        {

            // Update Ideal Eulers
            Pitch_Change = State.GetEulerChanges().x * Settings._PitchSpeed * ((Settings._InversePitch) ? -1.0f : 1.0f);
            Yaw_Change = State.GetEulerChanges().y * Settings._YawSpeed * ((Settings._InverseYaw) ? -1.0f : 1.0f);
            Roll_change = State.GetEulerChanges().z * Settings._RollSpeed * ((Settings._InverseRoll) ? -1.0f : 1.0f);

            // Update Rotations
            _Rigidbody.gameObject.transform.RotateAround(Position, Quadcopter_Right, Pitch_Change);
            _Rigidbody.gameObject.transform.RotateAround(Position, Quadcopter_Up, Yaw_Change);
            _Rigidbody.gameObject.transform.RotateAround(Position, Quadcopter_Forward, Roll_change);

            // If rest state, fix orientation if controls aren't being touced
            if(Pitch_Change == 0.0f && Yaw_Change == 0.0f && Roll_change == 0.0f)
            {
                Vector3 Euler = _Rigidbody.gameObject.transform.eulerAngles;

                float spd = 0.06f;
                Euler.x = Mathfx.Clerp(Euler.x, 0.0f, spd);
                //Euler.y = Mathfx.Clerp(Euler.y, 0.0f, spd);
                Euler.z = Mathfx.Clerp(Euler.z, 0.0f, spd);

                _Rigidbody.gameObject.transform.eulerAngles = Euler;
            }

        }
    }

    [RequireComponent(typeof(Rigidbody))]
    public class Main : MonoBehaviour
    {

        // Movement Values
        private float _SideSpeed = 0.0f;
        private float _ForwardSpeed = 0.0f;

        // Dead Mode
        //private bool _DeadMode = false;

        // Paused
        private bool _Paused = false;
        public void SetPause(bool s)
        {
            _Paused = s;
        }
        public bool CheckPause()
        {
            return _Paused;
        }
        public void TogglePause()
        {
            _Paused = !_Paused;
        }



        // Rotors
        public Quadcopter.Rotors _Rotors = new Quadcopter.Rotors();
        private bool _RotorSetup = false;

        // Settings
        public Quadcopter.Settings _Settings = new Quadcopter.Settings();

        // States
        public Quadcopter.States _States = new Quadcopter.States();

        // Physics
        public Quadcopter.Physics _Physics = new Quadcopter.Physics();
        private bool _PhysicsSetup = false;

        // Failcheck
        private bool FailCheck()
        {
            // Fail Check
            if (!_RotorSetup)
            {
                Debug.LogError("Rotors not setup");
                return true;
            }
            if(!_PhysicsSetup)
            {
                Debug.LogError("Missing Rigidbody");
                return true;
            }

            return false;
        }

        // Use this for initialization
        void Start()
        {
            // Get all used rotors
            _RotorSetup = _Rotors.Setup(this);

            // Get Rigidbody
            _PhysicsSetup = _Physics.Setup(gameObject);

            // Fail Check
            if (FailCheck())
                return;

            // Outputs
            Debug.Log("Total Rotors acquired: " + _Rotors.Amount());
            
        }

        // Update is called once per frame
        void Update()
        {
            // Fail Check
            if (FailCheck())
                return;

            // Check Pause Button
            if (QuadcopterControls.Pause())
                TogglePause();

            if (!_Paused)
            {
                // Update State
                _States.UpdateStates(ref _Settings);
            }

            if (QuadcopterControls.ResetOrientation())
                transform.eulerAngles = Vector3.zero;

            if(QuadcopterControls.ResetLevel())
            {
                transform.position = new Vector3(400, 60, 100);
                transform.eulerAngles = Vector3.zero;
            }

            // Udate Controls
            GamepadManager.Update();
        }
        private void FixedUpdate()
        {
            if (!_Paused)
            {
                // Update Physics
                _Physics.UpdatePosition();
                _Physics.UpdateAxis();
                _Physics.UpdateRotationEuler(ref _States, ref _Settings);
                _Physics.UpdatePosition(ref _States, ref _Settings);
            }
        }
    }
}