using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindControl
{
    NONE,
    USER_CONTROL,
    RANDOM_LIGHT,
    RANDOM_MEDIUM,
    RANDOM_ROUGH,
    CONSISTENT_CALM
}

public class WindGlobal : MonoBehaviour
{
    private ParticleSystem _ParticleSystem = null;

    private Quadcopter.Main Q = null;
    private bool IsInit = false;

    private static WindGlobal instance = null;
    public static WindGlobal GetInstance()
    {
        if (instance == null) instance = FindObjectOfType<WindGlobal>();
        return instance;
    }

    private Vector3 _WindDirection = Vector3.zero;
    public Vector3 _IdealWindDirection = Vector3.zero;
    public Vector3 GetWindDirection()
    {
        return _WindDirection;
    }

    private float _WindStrength = 0.75f;
    [Range(0.0f, 13.0f)]
    public float _IdealWindStrength = 0.75f;
    private static readonly float _MaxWindStrength = 13.0f;
    public float GetWindStrength()
    {
        return _WindStrength;
    }

    public Vector3 GetWind()
    {
        return _WindDirection * _WindStrength;
    }

    private float RandomWindTime = 0.0f;

    public WindControl _Control = WindControl.USER_CONTROL;
    [HideInInspector]
    public bool _State = true;

    public bool _AlwaysWind = false;

    //private List<GameObject> WindTrails = new List<GameObject>();
    //private int MaxTrails = 10;

    //private float SpawnTime = 0.0f;
    //private float SpawnTimeStall = 0.3f;

    public void Setup(Quadcopter.Main _Q)
    {
        Q = _Q;
        _ParticleSystem = GetComponent<ParticleSystem>();

        if(_ParticleSystem != null)
            IsInit = true;

    }

    private void NoWind()
    {
        _IdealWindStrength = 0.0f;
        _IdealWindDirection = Vector3.zero;
    }
    private void ChangeWindRandom(int min_second, int max_seconds, float min_speed, float max_speed, int percent_change_of_no_wind)
    {
        if(Time.time > RandomWindTime)
        {
            int number = Random.Range(1, 101);

            // Wind change
            if (number > percent_change_of_no_wind || _AlwaysWind)
            {
                // Random Wind Direction
                _IdealWindDirection = Random.insideUnitSphere;
                // Y only goes downwards
                _IdealWindDirection.y = -Mathf.Abs(_IdealWindDirection.y);

                // Set Speed randomly
                _IdealWindStrength = Random.Range(min_speed, max_speed);
            }
            // No wind
            else
            {
                NoWind();
            }

            // Update again at a random time
            RandomWindTime += Random.Range(min_second, max_seconds);
        }
    }

    private void UpdateWind()
    {
        // Only update state if not paused
        if (!Q.CheckPause())
        {
            switch (_Control)
            {
                case WindControl.RANDOM_LIGHT:
                    ChangeWindRandom(
                        15, 50,
                        0.0f, 4.0f,
                        70
                        );
                    break;

                case WindControl.RANDOM_MEDIUM:
                    ChangeWindRandom(
                        10, 30,
                        1.0f, 9.0f,
                        40
                        );
                    break;

                case WindControl.RANDOM_ROUGH:
                    ChangeWindRandom(
                        5, 15,
                        4.0f, 13.0f,
                        10
                        );
                    break;

                case WindControl.CONSISTENT_CALM:
                    ChangeWindRandom(
                        5, 5,
                        2.0f, 6.0f,
                        0
                        );
                    break;

                case WindControl.NONE:
                    NoWind();
                    break;

            }
        }

        // Max Window
        _IdealWindStrength = Mathf.Clamp(_IdealWindStrength, 0.0f, _MaxWindStrength);

        // Lerp to actual value
        _WindDirection = Vector3.Lerp(_WindDirection, _IdealWindDirection, 0.07f);
        _WindStrength = Mathf.Lerp(_WindStrength, _IdealWindStrength, 0.07f);
    }

    private Vector3 LocalPos = Vector3.zero;

    private float VisualSpeed = 10.0f;

    public void Update()
    {
        if(IsInit)
        {
            // Toggle particle system based on strength
            if (_IdealWindStrength == 0.0f)
                _ParticleSystem.Stop();
            else
                _ParticleSystem.Play();

            // Lerp Position based on opposite of wind direction
            LocalPos = Vector3.Lerp(LocalPos, -_WindDirection * _WindStrength * 20.0f, 0.2f);
            transform.position = Q.GetPosition() + LocalPos;

            var E = _ParticleSystem.velocityOverLifetime;

            E.xMultiplier = _WindStrength * VisualSpeed;
            E.yMultiplier = _WindStrength * VisualSpeed;
            E.zMultiplier = _WindStrength * VisualSpeed;

            E.x = _WindDirection.x * _WindStrength * VisualSpeed;
            E.y = _WindDirection.y * _WindStrength * VisualSpeed;
            E.z = _WindDirection.z * _WindStrength * VisualSpeed;

            
            UpdateWind();
        }


           


            //Debug.Log(transform.localPosition);
            //Debug.Log(_WindDirection + " " + _WindStrength);

            

            // Spawn new ones
            //  if (WindTrails.Count < MaxTrails)
            //  {
            //      if (Time.time > SpawnTime)
            //      {
            //          Vector3 P = Q.GetPosition() + Random.insideUnitSphere * Random.Range(4.0f, 20.0f);
            //  
            //          float TimeAlive = Random.Range(2, 7);
            //  
            //          GameObject Trail = MonoBehaviour.Instantiate(Q._Settings._WindTrailPrefab);
            //          Trail.transform.position = P;
            //  
            //          Trail.GetComponent<WindTrail>().Deadtime = Time.time + TimeAlive;
            //  
            //          // Stall for next update
            //          SpawnTime = Time.time + SpawnTimeStall;
            //  
            //          WindTrails.Add(Trail);
            //      }
            //  }

        
    }

}
