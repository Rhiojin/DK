using UnityEngine;
using System.Collections;

public class FCT : MonoBehaviour {

	public Color color = new Color(0.8f,0.8f,0,1.0f);
	private float scroll = 0.4f;  // scrolling velocity
	private float duration = 1.0f; // time to die
	private float alpha;
	
	void Start()
	{
		//guiText.material.color = color; // set text color
		alpha = 1;
	}
	
	void Update()
	{
		if (color.a > 0)
		{
			transform.position += new Vector3(0, scroll*Time.deltaTime ,0);
			//alpha -= Time.deltaTime/duration; 
			color.a -= Time.deltaTime/duration; 
			//guiText.material.color = new Color(color.r,color.g,color.b,alpha); 
			GetComponent<TextMesh>().color = color;
		} 
		else 
		{
			Destroy(gameObject); // text vanished - destroy itself
		}
	}
	
}
