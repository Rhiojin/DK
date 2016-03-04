using UnityEngine;
using System.Collections;

public class Coop : MonoBehaviour {
	public GameObject ChickenOBJ;
	GameObject ChickenBlank;
	float Timer = 0;
	// Update is called once per frame
	void Update () {
		Timer+=Time.deltaTime;
		
		if(Timer >= 20){
			ChickenBlank = Instantiate(ChickenOBJ,transform.position,transform.rotation) as GameObject;
			ChickenBlank.transform.Translate(Vector3.up);
			ChickenBlank.transform.Rotate(new Vector3(0,Random.Range(0,360),0));
			ChickenBlank.transform.Translate(Vector3.forward);
			ChickenBlank = null;
			Timer = 0;
		}
	}
}
