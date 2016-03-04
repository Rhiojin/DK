using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	private levelManager2 levelScript;

	void Start () 
	{
		levelScript = GameObject.Find("Level Manager").GetComponent<levelManager2>();	
		SetPlayAreaObject();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetPlayAreaObject()
	{
		levelScript.playArea[19,10] = gameObject;
		print ("ha");
	}
}
