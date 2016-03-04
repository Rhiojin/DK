using UnityEngine;
using System.Collections;

public class ShockTemp : MonoBehaviour {
	float Timer;
	// Update is called once per frame
	void Update () {
		Timer+=Time.deltaTime;
		if(Timer>=5){
			Destroy(gameObject);
		}
	}	
	void OnTriggerEnter(Collider Hit){
		if(Hit.gameObject.tag == "NME"){
			EnemyAI EnemyAI;
			EnemyAI = Hit.gameObject.GetComponent("EnemyAI") as EnemyAI;
			EnemyAI.HP -= 100;
		}
	}
}
