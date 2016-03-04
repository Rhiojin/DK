using UnityEngine;
using System.Collections;
using Pathfinding;

public class ImpAI : MonoBehaviour {
	
	MapAI MapAI;
	public GameObject target;
	//pathfinding
	Seeker seeker;
	Path path;
	bool Job= false;
	int CurrentWayPoint;
	// Use this for initialization
	void Start () {
		MapAI = GameObject.Find("MapAI").GetComponent("MapAI") as MapAI;
		seeker = GetComponent<Seeker>();
		target =  gameObject;
	}
		
	void Update()
	{
		if(target == null)
		{
			target = MapAI.Mine[0];
			seeker.StartPath(transform.position,target.transform.position, OnPathComplete);
			if(Vector3.Distance(GameObject.Find("Gold").transform.position,transform.position)<2)
				{
					transform.LookAt(GameObject.Find("Gold").transform);
					transform.Translate(Vector3.forward*Time.fixedDeltaTime);
				}
			
		}
		
	}
	
	
	void OnCollisionEnter( Collision hit){
		if(hit.gameObject.tag == "Mined"){
			SoilWall Wall;
			Wall = hit.gameObject.GetComponent("SoilWall") as SoilWall;
			Wall.HP--;
			if(Wall.HP<0.1){
				target = null;
				Job = false;
			}
			transform.Translate(Vector3.back/10);
			
		}
	}
	
	public void OnPathComplete (Path p){
		if(!p.error){
			path = p;
			CurrentWayPoint = 0;
			Job = true;
		}
		else Debug.Log(p.error);
	}
	
	// Update is called once per frame
	
	void FixedUpdate () {
		if(path == null){
			Job = false;
			return;
		}
		if(CurrentWayPoint >= path.vectorPath.Count){
			Job = false;
			return;
		}	
		if(target != null){
			transform.LookAt(path.vectorPath[CurrentWayPoint]);
			transform.Translate(Vector3.forward*Time.fixedDeltaTime);
			
			if(Vector3.Distance(transform.position, path.vectorPath[CurrentWayPoint]) <= 0.6f){
				CurrentWayPoint++;
			}
		}
	}
	
	void GetNewTarget()
	{
		target = target = MapAI.Mine[0];
		seeker.StartPath(transform.position,target.transform.position, OnPathComplete);		
		if(Vector3.Distance(GameObject.Find("Gold").transform.position,transform.position)<2)
				{
					transform.LookAt(GameObject.Find("Gold").transform);
					transform.Translate(Vector3.forward*Time.fixedDeltaTime);
				}
	}
}
