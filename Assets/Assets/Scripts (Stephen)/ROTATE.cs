using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROTATE : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 4000.0f * Time.deltaTime * Random.Range(1.0f, 1.1f));
	}
}
