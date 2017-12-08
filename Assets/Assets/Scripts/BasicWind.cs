using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWind : MonoBehaviour
{
    private ParticleSystem _ParticleSystem = null;

    public Vector3 WindDirection = Vector3.zero;
    public float WindStrength = 1.0f;

    private void Start()
    {
        _ParticleSystem = GetComponent<ParticleSystem>();

        var E = _ParticleSystem.velocityOverLifetime;
        E.xMultiplier = 1.0f;
        E.yMultiplier = 1.0f;
        E.zMultiplier = 1.0f;
    }

    // Update is called once per frame
    private void Update ()
    {
        var E = _ParticleSystem.velocityOverLifetime;
        E.x = WindDirection.x * WindStrength;
        E.y = WindDirection.y * WindStrength;
        E.z = WindDirection.z * WindStrength;


    }
}
