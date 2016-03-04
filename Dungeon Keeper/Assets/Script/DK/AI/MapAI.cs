using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapAI : MonoBehaviour {
	public List<GameObject> Mine = new List<GameObject>(); 
	public ArrayList Beds = new ArrayList();
	public ArrayList Chickens = new ArrayList();
	public ArrayList Librarys = new ArrayList();
	public ArrayList Workshops = new ArrayList();
	public GameObject DungeonHeart;

	
	public void MineBlock(GameObject mine){
		Mine.Add(mine);		
	}
	public void RemoveMineBlock(GameObject mine){
		Mine.Remove(mine);	
	}
	
	public void AddBed(GameObject bed){
		Beds.Add(bed);	
	}
	public void RemoveBed(GameObject bed){
		Beds.Remove(bed);
	}
	
	public void AddChicekn(GameObject shed){
		Chickens.Add(shed);
	}
	
	public void RemoveChicken(GameObject shed){
		Chickens.Remove(shed);	
	}
	
	public void AddLibrary(GameObject Lib){
		Librarys.Add(Lib);	
	}
	
	public void RemoveLibrary(GameObject Lib){
		Librarys.Remove(Lib);
	}
	public void AddWorkshop (GameObject Work){
		Workshops.Add(Work);	
	}
	public void RemoveWorkshop (GameObject Work){
		Workshops.Remove(Work);	
	}
	
}
