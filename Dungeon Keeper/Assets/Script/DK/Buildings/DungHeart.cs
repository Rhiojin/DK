using UnityEngine;
using System.Collections;

public class DungHeart : MonoBehaviour {
	public bool SpawnWiz = false;// can spawn wizaarrd?
	public bool SpawnOgre = false; // can spawn ogre
	
	public GameObject Goblin; // goblin prefab
	public GameObject Wizard; // wizard or warlock or wat evesss prefab
	public GameObject Ogre; // the ogre prefab grr
	GameObject Creature; // the holding var
	
	int CurrCreatures = 0; // how many creatures we got?
	int MaxCreatures = 20; // how many can we have
	
	public float Timer = 0; // how long till the next one?
	int Randomizer; // which unit to spawn?
	// Update is called once per frame
	void Update () {
		Timer+=Time.deltaTime;
		
		if(Timer >=50){
			if(CurrCreatures <= MaxCreatures){
				if(SpawnWiz){
					Creature = Instantiate(Wizard,transform.position,transform.rotation) as GameObject;
					Creature.transform.Translate(Vector3.up);
					Creature.transform.Translate(Vector3.forward);
				}
				else if(SpawnOgre){
					Creature = Instantiate(Ogre,transform.position,transform.rotation) as GameObject;
					Creature.transform.Translate(Vector3.up);
					Creature.transform.Translate(Vector3.forward);					
				}
				
				else{
					Creature = Instantiate(Goblin,transform.position,transform.rotation) as GameObject;
					Creature.transform.Translate(Vector3.up);
					Creature.transform.Translate(Vector3.forward);
				}
				
				CurrCreatures++;
			}
			else Timer = 0;
		}
	}
}
