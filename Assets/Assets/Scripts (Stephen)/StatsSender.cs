using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsSender : MonoBehaviour
{
    public Stats stats;
    WindGlobal windGlobal;
    Vector3 oldPosition;
    Vector3 newPosition;
    Vector3 velocity;
    float velocityFloat;
    float oldVelocityFloat;

    public float GetVelocity()
    {
        return velocityFloat;
    }

    // Use this for initialization
    void Start ()
    {
        newPosition = transform.position;
        oldPosition = newPosition;

        windGlobal = FindObjectOfType<WindGlobal>();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (stats != null)
        {

            newPosition = transform.position;
            velocity = newPosition - oldPosition;
            velocityFloat = Vector3.Distance(newPosition, oldPosition) / Time.fixedDeltaTime;

            stats.setWindSpeed(windGlobal._IdealWindStrength * 3.6f);


            if (velocityFloat < oldVelocityFloat + 5.0f &&
                velocityFloat > oldVelocityFloat - 5.0f)
                stats.setSpeed(velocityFloat);

            oldPosition = newPosition;
            oldVelocityFloat = velocityFloat;

        }
    }
}
