using UnityEngine;
using System.Collections;

public class SoilWall : MonoBehaviour {
	
	public float HP = 5;
	bool Gold = false;
	GameObject GoldOBJ;
	public GameObject GoldPB;
	MapAI MapAi;
	// Use this for initialization
	void Start () {
		MapAi = GameObject.Find("MapAI").GetComponent("MapAI") as MapAI;
		if(Random.Range(0,10) < 6){
			Gold = true;	
		}
			
	}	
	// Update is called once per frame
	void Update () {
		if(HP < 0){
			if(Gold){
				GoldOBJ = Instantiate(GoldPB,transform.position,transform.rotation) as GameObject;
				GoldOBJ.name = "Gold";
			}
			MapAi.RemoveMineBlock(gameObject);
			Destroy(gameObject);	
		}
	}
}
