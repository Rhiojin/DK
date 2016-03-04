using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class unitControls : MonoBehaviour {

	public Vector3 nextPos;
	public Vector3 currentPos;
	public Vector3 targetPos = new Vector3(-1,-1,-1);
	public Vector3 tempNextPos;
	public Vector3 tempCurrentPos;
	
	public bool moving;
	public float moveSpeed = 5f;
	private string state;
	public string prevState;
	private levelManager2 levelScript;
	private UnitManager managerScript;
	public Vector3 currentVector;

	private GameObject collisionObject;

	
	//-----LOS pathfinding
	public List<Vector3> clearPath = new List<Vector3>();
	public List<Vector3> checkedList = new List<Vector3>();
	
	//-----BFS + A* Pathfinding
	public List<Vector3> openList = new List<Vector3>();
	public List<Vector3> parentsList = new List<Vector3>();
	
	//A* g value
	public List<int> costList = new List<int>();
	//A* heuristic
	public List<int> hValue = new List<int>();
	
	public List<GameObject> pathMarksList = new List<GameObject>();
	public List<GameObject> markCheckedList = new List<GameObject>();
	
	
	private int listPos = 0;
	
	
	
	
	void Start () 
	{
		levelScript = GameObject.Find("Level Manager").GetComponent<levelManager2>();
		managerScript = GetComponent<UnitManager>();
		//managerScript.SetPlayAreaObject(gameObject);
		
		currentPos = transform.position;
		nextPos = currentPos;
		targetPos = currentPos;
		
		moving = false;
		state = "walk";
		
		tempNextPos = currentPos;
		tempCurrentPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!levelScript.pause)
		{
			//MoveUnit();
			AnimateUnit();
		}
	}
	
	public int CheckListPos()
	{
		return listPos;
	}
	
	public void SetListPos(int pos)
	{
		listPos = pos;
	}
	
	public string GetState()
	{
		return state;
	}
	
	public void SetState(string _state)
	{
		state = _state;
	}
	
	
	void AnimateUnit()
	{
		if(Vector3.Distance(currentPos, transform.position) > 0.1f)
		{
			
			//print (Vector3.Distance(currentPos, transform.position));
			//moving = true;
			
			
			if(transform.position.x > currentPos.x)
			{
				transform.position -= new Vector3(1,0,0) * moveSpeed * Time.deltaTime;
			}
			if(transform.position.x < currentPos.x)
			{
				transform.position += new Vector3(1,0,0) * moveSpeed * Time.deltaTime;
			}
			
			if(transform.position.y > currentPos.y)
			{
				transform.position -= new Vector3(0,1,0) * moveSpeed * Time.deltaTime;
			}
			if(transform.position.y < currentPos.y)
			{
				transform.position += new Vector3(0,1,0) * moveSpeed * Time.deltaTime;
			}

		}
		
		else
		{
			//moving = false;
			
			//			if(listPos < clearPath.Count)
			//			{
			//				listPos++;
			//			}
			
			transform.position = currentPos;
			if(CheckListPos() < clearPath.Count-1)
			{
				SetListPos(CheckListPos() + 1);
			}
			managerScript.hasOrders = false;
		}
		
	}

	
	public bool CheckForCollision()
	{
		managerScript.pcXpos = (int) tempNextPos.x;
		managerScript.pcYpos = (int) tempNextPos.y;
		
		//---if not empty
		collisionObject = managerScript.GetPlayAreaObject();
		if(collisionObject != null)
		{
			if(collisionObject.CompareTag("occupied") && tempNextPos != targetPos) 
			{
				tempNextPos = currentPos;
				tempCurrentPos = currentPos;
				targetPos = currentPos;
				return true;
			}
			else if(tempNextPos == targetPos)
			{
				if(collisionObject != null && collisionObject.CompareTag("occupied"))
				{
					tempNextPos = currentPos;
					tempCurrentPos = currentPos;
					targetPos = currentPos;
				}
			}
		}
		//--if empty
		else if(collisionObject == null && !moving)
		{
			clearPath.Add(tempNextPos);
			
			managerScript.pcXpos = (int) tempCurrentPos.x;
			managerScript.pcYpos = (int) tempCurrentPos.y;
			
			managerScript.pcXpos = (int) tempNextPos.x;
			managerScript.pcYpos = (int) tempNextPos.y;
			
			tempCurrentPos = tempNextPos;
			
			
		}
		return false;
	}
	

}
