using UnityEngine;
using System.Collections;

public class miningTimer : MonoBehaviour {

	private float timer = 0;
	private TextMesh text;

	void Start () 
	{
		timer = GameObject.Find("Level Manager").GetComponent<levelManager2>().miningTime;
		text = GetComponent<TextMesh>();
		text.GetComponent<Renderer>().sortingLayerID = 3;
	}
	
	// Update is called once per frame
	void Update () 
	{
		timer -= Time.fixedDeltaTime;
		text.text = timer.ToString("F0")+"s";
		if(timer <=0) Destroy(gameObject);
	}
}
