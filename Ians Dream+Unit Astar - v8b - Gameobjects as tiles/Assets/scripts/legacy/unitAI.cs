using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class unitAI : MonoBehaviour {

	public Vector3 nextPos;
	public Vector3 currentPos;
	public Vector3 targetPos = new Vector3(-1,-1,-1);
	public Vector3 tempNextPos;
	public Vector3 tempCurrentPos;

	public bool moving;
	private float moveSpeed = 5f;
	private string state;
	public string prevState;
	private levelManager levelScript;
	public Vector3 currentVector;

	//private float targetDist = 0; 

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
		levelScript = GameObject.Find("Level Manager").GetComponent<levelManager>();

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
		}

	}

//	void MoveUnit()
//	{
//		float dx = targetPos.x - currentPos.x;
//		float dy = targetPos.y - currentPos.y;
//		float angle = Mathf.Atan2(dx,dy);
//		targetDist  = Mathf.Sqrt(dx*dx+dy*dy);
//
//		if(Mathf.Abs(dx) > 0.1f)
//		{
//			nextPos.x = currentPos.x + Mathf.Round(1.4f* Mathf.Sin(angle));
//		}
//
//		if(Mathf.Abs(dy) > 0.1f)
//		{
//			nextPos.y = currentPos.y + Mathf.Round(1.4f* Mathf.Cos(angle));
//		}
//	
//		CheckForCollision();
//	}



//	public void CheckForCollision()
//	{
//		if(nextPos.x < 0 || nextPos.x > levelScript.playArea.GetLength(0))
//		{
//			nextPos.x = currentPos.x;
//		}
//
//		if(nextPos.y < 0 || nextPos.y > levelScript.playArea.GetLength(1))
//		{
//			nextPos.y = currentPos.y;
//		}
//
//		if(levelScript.playArea[(int)nextPos.x, (int)nextPos.y] != null && (nextPos != targetPos))
//		{
//			int randX = Random.Range(-1,1);
//			int randY = Random.Range(-1,1);
//			while(currentPos.x + randX < 0 || currentPos.x+randX > levelScript.playArea.GetLength(0))
//			{
//				randX = Random.Range(-1,1);
//			}
//			nextPos.x = currentPos.x+randX;
//
//			while(currentPos.y + randY < 0 || currentPos.y+randY > levelScript.playArea.GetLength(1))
//			{
//				randY = Random.Range(-1,1);
//			}
//			nextPos.y = currentPos.y+randY;
//		}
//
//		if(levelScript.playArea[(int)nextPos.x, (int)nextPos.y] == null)
//		{
//			levelScript.playArea[(int)currentPos.x,(int)currentPos.y] = null;
//			levelScript.playArea[(int)nextPos.x,(int)nextPos.y] = gameObject;
//
//			currentPos = nextPos;
//		}
//	}

	public bool CheckForCollision()
	{
		levelScript.pcXpos = (int) tempNextPos.x;
		levelScript.pcYpos = (int) tempNextPos.y;

		//---if not empty
		if(levelScript.GetPlayAreaObject() && tempNextPos != targetPos) 
		{
			tempNextPos = currentPos;
			tempCurrentPos = currentPos;
			targetPos = currentPos;
			return true;
		}
		else if(tempNextPos == targetPos)
		{
			if(levelScript.GetPlayAreaObject() != null)
			{
				tempNextPos = currentPos;
				tempCurrentPos = currentPos;
				targetPos = currentPos;
			}
		}
		//--if empty
		if(levelScript.GetPlayAreaObject() == null && !moving)
		{
			clearPath.Add(tempNextPos);

			levelScript.pcXpos = (int) tempCurrentPos.x;
			levelScript.pcYpos = (int) tempCurrentPos.y;

			levelScript.pcXpos = (int) tempNextPos.x;
			levelScript.pcYpos = (int) tempNextPos.y;

			tempCurrentPos = tempNextPos;


		}
		return false;
	}


}
