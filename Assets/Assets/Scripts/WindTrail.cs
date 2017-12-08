using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTrail : MonoBehaviour
{
    private ParticleSystem _ParticleSystem = null;

    private WindGlobal G = null;

    public float Deadtime = Mathf.Infinity;

    // Use this for initialization
    void Start ()
    {
        _ParticleSystem = GetComponent<ParticleSystem>();


        G = WindGlobal.GetInstance();
    }
	
	// Update is called once per frame
	void Update ()
    {
        var E = _ParticleSystem.velocityOverLifetime;

        Vector3 Wind = G.GetWindDirection() * G.GetWindStrength();

        transform.position += Wind;

        if(Time.time > Deadtime)
        {
            _ParticleSystem.Stop();
        }

        if(_ParticleSystem.IsAlive() == false)
        {
            Destroy(gameObject);
        }
	}
}
