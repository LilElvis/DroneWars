using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindState
{
    USER_CONTROL,
    RANDOM
}

public class WindGlobal
{
    private Quadcopter.Main Q = null;
    private bool IsInit = false;

    private static WindGlobal instance = null;
    public static WindGlobal GetInstance()
    {
        if (instance == null) instance = new WindGlobal();
        return instance;
    }

    private Vector3 _WindDirection = Vector3.zero;
    public Vector3 GetWindDirection()
    {
        return _WindDirection;
    }

    private float _WindStrength = 0.75f;
    public float GetWindStrength()
    {
        return _WindStrength;
    }

    public WindState _State = WindState.RANDOM;

    private List<GameObject> WindTrails = new List<GameObject>();
    private int MaxTrails = 10;

    private float SpawnTime = 0.0f;
    private float SpawnTimeStall = 0.3f;

    public void Setup(Quadcopter.Main _Q)
    {
        Q = _Q;
        IsInit = true;
    }

    public void ManualUpdate()
    {
        if (IsInit && !Q.CheckPause())
        {
            _WindDirection = _WindDirection = Vector3.left;
            _WindStrength = 0.8f;

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

}
