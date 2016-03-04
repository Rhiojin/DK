using UnityEngine;
using System.Collections;

public class ChickMove : MonoBehaviour {
	public float timer = 0;
	float DeathClock = 0;
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.forward/100);
		timer+=Time.deltaTime;
		DeathClock+=Time.deltaTime;
		if(timer >=Random.Range(3,6)){
			transform.Translate(Vector3.up/4);
			transform.Rotate(new Vector3(0,Random.Range(10,200),0));
			timer = 0;
		}
		if(DeathClock >= 60){
			Destroy(gameObject);
		}
	}
}
