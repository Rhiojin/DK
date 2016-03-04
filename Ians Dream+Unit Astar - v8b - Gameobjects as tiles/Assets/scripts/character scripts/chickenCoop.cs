using UnityEngine;
using System.Collections;

public class chickenCoop : MonoBehaviour {

	public GameObject chicken;
	private levelManager2 levelScript;

	void Start () 
	{
		levelScript = GameObject.Find("Level Manager").GetComponent<levelManager2>();
		InvokeRepeating("MakeChickens",0.01f, 10f);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void MakeChickens()
	{
		if(levelScript.chickenCurrentCount < levelScript.chickenMaxCount)
		{
			Instantiate( chicken, transform.position, chicken.transform.rotation);
			levelScript.chickenCurrentCount++;
		}
	}
}
