using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterTime : MonoBehaviour {

	float time = 0.0f;
	public float lifetime = 1.0f;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		time += Time.deltaTime;
		if(time > lifetime)
			Destroy(this.gameObject);
	}
}
