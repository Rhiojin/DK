using UnityEngine;
using System.Collections;

public class HP : MonoBehaviour {
	public float HealthPoints; 
	
	// Update is called once per frame
	void Update () {
		if(HealthPoints <= 0){
			Destroy(gameObject);
			if(gameObject.name == "DungeonHeart"){
				GameOver();
			}
		}
	}
	void GameOver(){
	// show game over thingy
	// WAH WAhAAHAHH WAAAAAH
	// end game
		
	}
}
