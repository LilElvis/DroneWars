using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour 
{
	Rigidbody body;
	Vector3[] propellers = new Vector3[4];
	float propellerX = 0.4f;
	float propellerY = 0.2f;
	float propellerZ = 0.4f;
	public GameObject explosion;

	// Use this for initialization
	void Start () 
	{
		propellers[0] = new Vector3(-propellerX, propellerY, -propellerZ);
		propellers[1] = new Vector3(propellerX, propellerY, -propellerZ);
		propellers[2] = new Vector3(-propellerX, propellerY, propellerZ);
		propellers[3] = new Vector3(propellerX, propellerY, propellerZ);
		body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		for(int i = 0; i < propellers.Length; ++i)
		{
			body.AddForceAtPosition(
				this.transform.rotation * new Vector3(0.0f, 2.0f, 0.0f), 
				this.transform.rotation * propellers[i]);
		}

		body.AddRelativeForce(new Vector3(
			10.0f * Input.GetAxis("Horizontal"),
			10.0f * Input.GetAxis("Height"),
			10.0f * Input.GetAxis("Vertical")));
	}

	void OnCollisionEnter(Collision other)
	{
		Instantiate(explosion, this.transform.position, this.transform.rotation);
	}
}
