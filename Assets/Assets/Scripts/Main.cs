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

        [Space]
        [Header("Thrusters:")]
        //[Range(0.0001f, 90.0f)]
        public float _ThrusterSpeed = 1.0f;
        //[Range(0.0f, 1.0f)]
        public float _ThrusterTransition_Upwards = 0.2f;
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

        // Get Data
        public float GetRotorSpeed(int index)
        {
            return _Rotors[index].GetSpeed();
        }
        public float GetOverallSpeed()
        {
            if (_Rotors == null)
                return 0.0f;

            float total = 0.0f;

            for (int i = 0; i < _Rotors.Length; i++)
            {
                total += GetRotorSpeed(i);
            }
            return total / (float)_Rotors.Length;
        }
        public float GetRotorSpeedRatio(int index)
        {
            return _Rotors[index].GetSpeedRatio();
        }
        public float GetOverallSpeedRatio()
        {
            if (_Rotors == null)
                return 0.0f;

            float total = 0.0f;

            for (int i = 0; i < _Rotors.Length; i++)
            {
                total += GetRotorSpeedRatio(i);
            }
            return total / (float)_Rotors.Length;
        }
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
        public float GetThruster()
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
            else
                thrusterState = ThrusterState.STILL;

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
    
        // Speed
        float Speed = 0.0f;
        // Ideal Speed
        float Speed_Ideal = 0.0f;
        
        // Update Position
        public void UpdatePosition(ref Quadcopter.States State, ref Quadcopter.Settings Settings)
        {
            // Upwards of Quadcopter
            Vector3 Quadcopter_Up = _Rigidbody.gameObject.transform.up.normalized;
            // If close enough to be up, stay up
            if (Vector3.Distance(Quadcopter_Up, Vector3.up) < 0.1f)
                Quadcopter_Up = Vector3.up;

            // Gravity
            Vector3 Gravity = new Vector3(0, Settings._Gravity * Settings._GravityModifier, 0);


            // Calculated Speed
            Speed_Ideal = State.GetThrusterUpwards() * Settings._ThrusterSpeed * ((Settings._InverseThrusters) ? -1.0f : 1.0f);

            // Going upwards at all immediately combats Gravity
            if (Speed_Ideal > 0.0f)
            {
                Speed_Ideal += Mathf.Abs(Gravity.y);
            }

            // Adjust Speed Based on going up or down
            if (Speed < Speed_Ideal) // Going up
                Speed = Mathf.Lerp(Speed, Speed_Ideal, Settings._ThrusterTransition_Upwards);
            else // Going down
                Speed = Mathf.Lerp(Speed, Speed_Ideal, Settings._ThrusterTransition_Downwards);

            Vector3 Direction = Quadcopter_Up * Speed + Gravity;


            // DEBUG
            Debug.DrawLine(_Rigidbody.gameObject.transform.position, _Rigidbody.gameObject.transform.position + Direction, Color.white);
            // End
            _Rigidbody.velocity = Direction;

            //  
            //  // Update Ideal Speed
            //  Speed_Ideal = State.GetThrusterSpeed() * Settings._ThrusterSpeed * ((Settings._InverseThrusters) ? -1.0f : 1.0f);
            //  
            //  // Update Speed Value
            //  Speed = Mathf.Lerp(Speed, Speed_Ideal, Settings._ThrusterTransition);
            //  
            //  //Debug.Log(Speed);

            //Speed_Ideal = State.GetThrusterSpeed() * Settings._ThrusterSpeed * ((Settings._InverseThrusters) ? -1.0f : 1.0f);
            //Debug.Log(Speed_Ideal);

            //  // Speed Targeted
            //  Speed_Ideal = State.GetThrusterRatio() * Settings._ThrusterSpeed * ((Settings._InverseThrusters) ? -1.0f : 1.0f);
            //  
            //  // Current Speed
            //  if(Speed < Speed_Ideal) // Going up
            //      Speed = Mathf.Lerp(Speed, Speed_Ideal, Settings._ThrusterTransition_Upwards);
            //  else // Going down
            //      Speed = Mathf.Lerp(Speed, Speed_Ideal, Settings._ThrusterTransition_Downwards);
            //  
            //  
            //  // Upwards
            //  Vector3 Quadcopter_Up = _Rigidbody.gameObject.transform.up.normalized;
            //  // If close enough to be up, stay up
            //  if(Vector3.Distance(Quadcopter_Up, Vector3.up) < 0.1f)
            //  {
            //      Quadcopter_Up = Vector3.up;
            //  }
            //  
            //  Vector3 MovementDirection = Quadcopter_Up * Speed;
            //  // Gravity
            //  Vector3 Gravity = new Vector3(0, Settings._Gravity * Settings._GravityModifier, 0);
            //  
            //  // Speed = 1 -> max speed
            //  // Speed = 0 -> grav
            //  // Speed = -1 -> 
            //  
            //  // Final Direction
            //  //Debug.Log(MovementDirection);
            //  Vector3 FinalDirection = MovementDirection + Gravity;
            //  
            //  _Rigidbody.velocity = FinalDirection;
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

            // Update Eulers
            _Rigidbody.gameObject.transform.eulerAngles += new Vector3(Pitch_Change, Yaw_Change, Roll_change);
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

            GamepadManager.Update();
        }
        private void FixedUpdate()
        {
            if (!_Paused)
            {
                // Update Physics
                _Physics.UpdateRotationEuler(ref _States, ref _Settings);
                _Physics.UpdatePosition(ref _States, ref _Settings);
            }
        }
    }
}