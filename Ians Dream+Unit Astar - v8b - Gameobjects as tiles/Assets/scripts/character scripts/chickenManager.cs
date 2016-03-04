using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class chickenManager : MonoBehaviour {
	

	
	public bool pause;
	public bool invalidPath = false;
	public bool computingPath = false;
	public bool busy = false;
	private Vector3 nullVector = new Vector3(-1,-1,-1);
	public bool targetFound = false;
	//public bool showDFSMarkers = false;
	
	//--A* pathfinding Options
	private bool noDiagonal = false;
	private bool euclidian = false;
	
	private Vector3 clickPoint;
	
	
	//---------Script Grabs
	public chickenControl unitScript;
	private levelManager2 levelScript;
	
	
	public int pcXpos;
	public int pcYpos;
	
	//---
	
	private int xBounds;
	private int yBounds;
	
	public bool hasOrders = false;
	private bool waiting = true;
//	private GameObject theWorkOrder;
//	private int theMinedBlockX;
//	private int theMinedBlockY;
	
	private bool pathFound = false;
	
	public float miningTime = 6f;

	public Vector3 pos = Vector3.zero;
	private Vector3 originalPos = Vector3.zero;

	private bool dead = false;
	public Sprite blood;

	//-----------------------------------ALL COORDINATES AND POSITIONS NEED TO BE INTS OR ROUNDED AT LEAST--------------------------
	
	void Start () 
	{
		
		//-----Script Grabs
		unitScript = transform.GetComponent<chickenControl>();
		levelScript = GameObject.Find("Level Manager").GetComponent<levelManager2>();	
		xBounds = levelScript.playArea.GetLength(0)-1;
		yBounds = levelScript.playArea.GetLength(1)-1;
		
		//levelScript.workers.Add(gameObject);
		miningTime = levelScript.miningTime;

		pos = transform.position; 
		originalPos = transform.position;

		Meander();
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(!dead)
		{
			if(computingPath == false)
			{
				UpdatePCPositionFromList();
			}
			else
			{

				ComputePCPathWithAStar(unitScript.currentPos, unitScript.targetPos);
			}
		}
		

		
	}

	IEnumerator MeanderStarter()
	{
		yield return new WaitForSeconds(2);
		Meander();
	}

	void Meander()
	{
		if(hasOrders == false)
		{
			hasOrders = true;
			waiting = false;
			computingPath = true;
			invalidPath = false;
			unitScript.clearPath.Clear();
			unitScript.SetListPos(0);
			unitScript.prevState = unitScript.GetState();
			unitScript.SetState("Searching");

			pos.x = Mathf.Round( Random.Range(originalPos.x-1, originalPos.x+1));
			pos.y = Mathf.Round( Random.Range(originalPos.y-1, originalPos.y+1));

			unitScript.targetPos = pos;
			
			
		}

	}

	public void Kill()
	{
		if(!dead)
		{
			dead = true;
			GetComponent<SpriteRenderer>().sprite = blood;
			levelScript.chickenCurrentCount--;
			StopAllCoroutines();
			Destroy(gameObject,4);
		}
	}

//	public void GetWorkOrder(int x, int y, int x2, int y2)
//	{
//		//need to be updated with a second pos for position nearest workorder
//		theWorkOrder = GetPlayAreaObject(x,y);
//		theMinedBlockX = x2;
//		theMinedBlockY = y2;
//		
//		if(hasOrders == false)
//		{
//			hasOrders = true;
//			waiting = false;
//			computingPath = true;
//			invalidPath = false;
//			unitScript.clearPath.Clear();
//			unitScript.SetListPos(0);
//			unitScript.prevState = unitScript.GetState();
//			unitScript.SetState("Searching");
//			
//			
//			
//		}
//		
//		if(theWorkOrder == null)
//		{
//			//unitScript.targetPos = levelScript.workOrders[0];
//		}
//		
//		else if(theWorkOrder.CompareTag("soil") || theWorkOrder.CompareTag("goldSoil") || theWorkOrder.CompareTag("unoccupied"))
//		{
//			unitScript.targetPos = levelScript.workOrders[0];
//			
//		}
//		
//		
//		//		if(hasOrders == false)
//		//		{
//		//			hasOrders = true;
//		//			waiting = false;
//		//			computingPath = true;
//		//			invalidPath = false;
//		//			unitScript.clearPath.Clear();
//		//			unitScript.SetListPos(0);
//		//			unitScript.prevState = unitScript.GetState();
//		//			unitScript.SetState("Searching");
//		//			
//		//			
//		//			
//		//						if(GetPlayAreaObject(x,y) == null) //--Update with workorder type
//		//						{
//		//							//do nothing
//		//							unitScript.targetPos = levelScript.workOrders[0];
//		//			
//		//						}
//		//		}
//	}
	
	
	
	int ComputeHValue_Euclid(Vector3 _cur, Vector3 _target)
	{
		int dx = (int)(_target.x - _cur.x)*10;
		int dy = (int)(_target.y - _cur.y)*10;
		
		return (int)Mathf.Sqrt((dx*dx) + (dy*dy));
	}
	
	int ComputeHValue_Manhattan(Vector3 _cur, Vector3 _target)
	{
		int dx = (int)Mathf.Abs(_target.x - _cur.x)*10;
		int dy = (int)Mathf.Abs(_target.y - _cur.y)*10;
		
		return dx+dy;
	}
	
	void UpdateUnitPosition()
	{
		if(unitScript.targetPos.x > unitScript.currentPos.x) unitScript.nextPos.x = unitScript.currentPos.x+1;
		if(unitScript.targetPos.x < unitScript.currentPos.x) unitScript.nextPos.x = unitScript.currentPos.x-1;
		if(unitScript.targetPos.y > unitScript.currentPos.y) unitScript.nextPos.y = unitScript.currentPos.y+1;
		if(unitScript.targetPos.y < unitScript.currentPos.y) unitScript.nextPos.y = unitScript.currentPos.y-1;
		
		unitScript.CheckForCollision();
		
		if(levelScript.playArea[(int)unitScript.nextPos.x, (int)unitScript.nextPos.y] == null && !unitScript.moving)
		{
			unitScript.currentPos = unitScript.nextPos;
		}
		else if(levelScript.playArea[(int)unitScript.nextPos.x, (int)unitScript.nextPos.y].CompareTag("unoccupied")&& !unitScript.moving)
		{
			unitScript.currentPos = unitScript.nextPos;
		}
	}
	
	void UpdatePCPositionFromList()
	{
		float dx = unitScript.targetPos.x - unitScript.currentPos.x;
		float dy = unitScript.targetPos.y - unitScript.currentPos.y;
		//float angle = Mathf.Atan2(dx,dy);
		
		if(Mathf.Abs(dx) > 0.1f || Mathf.Abs(dy) > 0.1f)
		{
			
			if(unitScript.clearPath.Count > 0)
			{
				if(unitScript.CheckListPos()+1 > unitScript.clearPath.Count -1)
				{
					
				}
				else
				{
					unitScript.nextPos.x = unitScript.clearPath[unitScript.CheckListPos()+1].x;
					unitScript.nextPos.y = unitScript.clearPath[unitScript.CheckListPos()+1].y;
				}
			}
			
			
		}
		else
		{
			if(hasOrders)
			{
				hasOrders = false;
				
			}
			if(waiting == false && !busy)
			{
				
				//if(pathFound) StartCoroutine( RevealBlock(theMinedBlockX, theMinedBlockY) );
				pathFound = false;
				waiting = true;
				StartCoroutine( MeanderStarter() );
			}
			
		}
		
		
		
		//--Check if intended next spot is valid
		//		if(levelScript.playArea[(int)unitScript.nextPos.x, (int)unitScript.nextPos.y] == null && !unitScript.moving)
		//		{
		//			levelScript.playArea[(int)unitScript.currentPos.x, (int)unitScript.currentPos.y] = null;
		//			levelScript.playArea[(int)unitScript.nextPos.x, (int)unitScript.nextPos.y] = gameObject;
		//			unitScript.currentPos = unitScript.nextPos;
		//			
		//		}
		
		//		if(levelScript.playArea[(int)unitScript.nextPos.x, (int)unitScript.nextPos.y] == null && !unitScript.moving )
		//		{
		//			levelScript.playArea[(int)unitScript.currentPos.x, (int)unitScript.currentPos.y] = null;
		//			levelScript.playArea[(int)unitScript.nextPos.x, (int)unitScript.nextPos.y] = gameObject;
		//			unitScript.currentPos = unitScript.nextPos;
		//			
		//		}
		//if(levelScript.playArea[(int)unitScript.nextPos.x, (int)unitScript.nextPos.y].CompareTag("unoccupied") && !unitScript.moving)
		//{
			//levelScript.playArea[(int)unitScript.currentPos.x, (int)unitScript.currentPos.y].tag = "unoccupied";
			//levelScript.playArea[(int)unitScript.nextPos.x, (int)unitScript.nextPos.y].tag = "occupied";
			unitScript.currentPos = unitScript.nextPos;
		//}
	}
	
	
	IEnumerator RevealBlock(int x, int y)
	{
		print("about to show block");
		pathFound = false;
		waiting = true;
		
		//Instantiate(levelScript.miningTimer,new Vector3((float)x,(float)y,-1),levelScript.miningTimer.transform.rotation);
		yield return new WaitForSeconds(miningTime-1);
		
		if(levelScript.GetPlayAreaObject(x,y) != null)
		{
			if(levelScript.GetPlayAreaObject(x,y).name == "goldSoil(Clone)")
			{
				levelScript.UpdateGold(true, 15);
			}
		}
		
		levelScript.playArea[x,y] = Instantiate(levelScript.claimedFloorB,new Vector3((float)x,(float)y,0),levelScript.claimedFloorB.transform.rotation) as GameObject;
		levelScript.RevealAdjacentTiles(x,y);
		
		
		levelScript.workers.Add(gameObject);
		
	}
	
	
	void ComputePCPath()
	{
		float dx = unitScript.targetPos.x - unitScript.tempCurrentPos.x;
		float dy = unitScript.targetPos.y - unitScript.tempCurrentPos.y;
		float angle = Mathf.Atan2(dx,dy);
		
		if(Mathf.Abs(dx) > 0.1f)
		{
			unitScript.tempNextPos.x = unitScript.tempCurrentPos.x + Mathf.Round(1.4f*Mathf.Sin(angle));
		}
		
		if(Mathf.Abs(dy) > 0.1f)
		{
			unitScript.tempNextPos.y = unitScript.tempCurrentPos.y + Mathf.Round(1.4f*Mathf.Cos(angle));
		}
		
		if(unitScript.targetPos == unitScript.tempNextPos)
		{
			unitScript.SetState(unitScript.prevState);
			computingPath = false;
		}
		
		invalidPath = unitScript.CheckForCollision();
		if(invalidPath)
		{
			unitScript.SetState(unitScript.prevState);
			computingPath = false;
		}
	}
	
	#region Breadth first search
	
	void ComputePCPathWithBFS(Vector3 _startPos, Vector3 _targetPos)
	{
		if(busy == false)
		{
			busy = true;
			unitScript.parentsList.Clear();
			unitScript.checkedList.Clear();
			targetFound = false;
			BFSPath(_startPos,_targetPos);
		}
	}
	
	bool BFSPath(Vector3 _startPos, Vector3 _targetPos)
	{
		Vector3 node;
		node = _startPos;
		unitScript.parentsList.Add(nullVector);
		unitScript.openList.Add(node);
		unitScript.checkedList.Add(node);
		while(unitScript.openList.Count>0)
		{
			Vector3 n = unitScript.openList[0];
			unitScript.openList.RemoveAt(0);
			if(n ==_targetPos)
			{
				unitScript.openList.Clear();
				targetFound = true;
				busy = false;
				BackTrackPath(n);
				return true;
			}
			
			for(int i = (int)n.x-1; i < (int)n.x+2; i++)
			{
				for(int j = (int)n.y-1; j < (int)n.y+2; j++)
				{
					Vector3 neighbour = new Vector3(i,j,0);
					
					
					if(i < 0 || i > xBounds)
					{
						continue;
					}
					else if(j < 0 || j > yBounds)
					{
						continue;
					}
					
					if(!CheckNode(neighbour))
					{
						continue;
					}
					
					unitScript.parentsList.Add(n);
					unitScript.openList.Add(neighbour);
					
				}
			}
		}
		return false;
	}
	
	bool CheckNode(Vector3 _node)
	{
		for(int i = 0; i < unitScript.checkedList.Count; i++)
		{
			if(_node == unitScript.checkedList[i])
			{
				return false;
			}
		}
		if(levelScript.playArea[(int)_node.x, (int)_node.y] == null)
		{
			unitScript.checkedList.Add(_node);
			return true;
		}
		else if(levelScript.playArea[(int)_node.x, (int)_node.y].CompareTag("unoccupied"))
		{
			unitScript.checkedList.Add(_node);
			return true;
		}
		else
		{
			//print ("failing @ check node");
			return false;
		}
	}
	
	void BackTrackPath(Vector3 _fin)
	{
		List<Vector3> _temp = new List<Vector3>();
		unitScript.clearPath.Clear();
		while(unitScript.parentsList[unitScript.checkedList.IndexOf(_fin)] != nullVector)
		{
			unitScript.clearPath.Add(_fin);
			_fin = unitScript.parentsList[unitScript.checkedList.IndexOf(_fin)];
			if(_fin == unitScript.currentPos)
			{
				unitScript.clearPath.Add(_fin);
			}
		}
		
		//loop backwards
		for(int i = unitScript.clearPath.Count - 1; i > -1; i--)
		{
			_temp.Add(unitScript.clearPath[i]);
		}
		
		unitScript.clearPath = _temp;
		computingPath = false;
		unitScript.SetState(unitScript.prevState);
		pathFound = true;
		//print ("pathfound");
	}
	
	#endregion
	
	#region A* pathfinding
	
	void ComputePCPathWithAStar(Vector3 _startPos, Vector3 _targetPos)
	{
		if(busy == false)
		{
			//unit is busy thinking
			busy = true;
			//clear lists
			unitScript.costList.Clear();
			unitScript.hValue.Clear();
			unitScript.parentsList.Clear();
			unitScript.checkedList.Clear();
			//start the search
			//			if(AStarPath(_startPos, _targetPos))
			//			{
			//				Debug.Log("No. Nodes checked with A* = " + unitScript.checkedList.Count);
			//			}
			//			else
			//			{
			//				Debug.Log("target not found!!");
			//			}
			
			StartCoroutine( AStarPath2(_startPos, _targetPos) );
			
		}
	}
	
	bool AStarPath(Vector3 _startPos, Vector3 _targetPos)
	{
		Vector3 _node;
		_node = _startPos;
		unitScript.parentsList.Add(nullVector);
		unitScript.openList.Add(_node);
		unitScript.checkedList.Add(_node);
		unitScript.costList.Add(0);
		unitScript.hValue.Add(0);
		
		while(unitScript.openList.Count > 0)
		{
			
			Vector3 _n = LowestF();
			unitScript.openList.RemoveAt(unitScript.openList.IndexOf(_n));
			if(_n == _targetPos)
			{
				unitScript.openList.Clear();
				busy = false;
				BackTrackPath(_n);
				
				return true;
			}
			
			for(int i = (int)_n.x - 1; i<(int)_n.x+2; i++)
			{
				for(int j = (int)_n.y - 1; j<(int)_n.y+2; j++)
				{
					int _gval;
					int _new_gval;
					int _new_hval;
					
					Vector3 _neighbor = new Vector3(i,j,0);
					if(_neighbor == _n)
					{
						continue;
					}
					if(i<0 || i > xBounds) //--Playarea max size
					{
						continue;
					}
					else if(j < 0 || j > yBounds)
					{
						continue;
					}
					
					if(noDiagonal)
					{
						if(j == (int)_n.y + 1 && i == (int)_n.x - 1)
						{
							continue;
						}
						else if(j == (int)_n.y + 1 && i == (int)_n.x + 1)
						{
							continue;
						}
						else if(j == (int)_n.y - 1 && i == (int)_n.x - 1)
						{
							continue;
						}
						else if(j == (int)_n.y - 1 && i == (int)_n.x + 1)
						{
							continue;
						}
					}
					//diaganol
					if(j == (int)_n.y + 1 && i == (int)_n.x - 1)
					{
						_gval = 14;
					}
					else if(j == (int)_n.y + 1 && i == (int)_n.x + 1)
					{
						_gval = 14;
					}
					else if(j == (int)_n.y - 1 && i == (int)_n.x - 1)
					{
						_gval = 14;
					}
					else if(j == (int)_n.y - 1 && i == (int)_n.x + 1)
					{
						_gval = 14;
					}
					else
					{
						_gval = 10;
					}
					
					//compute new g value
					_new_gval = unitScript.costList[unitScript.checkedList.IndexOf(_n)]+_gval;
					//compute Hvalue with euclidian or manhattan
					if(euclidian)
					{
						_new_hval = ComputeHValue_Euclid(_neighbor, _targetPos);
					}
					else
					{
						_new_hval = ComputeHValue_Manhattan(_neighbor, _targetPos);
					}
					
					if(!CheckNode(_neighbor))
					{
						continue;
					}
					else
					{
						if(OnOpenList(_neighbor))
						{
							if(_new_gval < unitScript.costList[unitScript.checkedList.IndexOf(_neighbor)])
							{
								unitScript.parentsList[unitScript.checkedList.IndexOf(_neighbor)] = _n;
								unitScript.costList[unitScript.checkedList.IndexOf(_neighbor)] = _new_gval;
								unitScript.hValue[unitScript.checkedList.IndexOf(_neighbor)] = _new_hval;
							}
						}
						else
						{
							unitScript.openList.Add(_neighbor);
							unitScript.parentsList.Add(_n);
							unitScript.costList.Add(_new_gval);
							unitScript.hValue.Add(_new_hval);
						}
					}				
				}
			}
		}
		
		busy = false;
		unitScript.targetPos = unitScript.currentPos;
		unitScript.clearPath.Add (unitScript.targetPos);
		print ("path not found");
		return false;
	}
	
	IEnumerator AStarPath2(Vector3 _startPos, Vector3 _targetPos)
	{
		Vector3 _node;
		_node = _startPos;
		unitScript.parentsList.Add(nullVector);
		unitScript.openList.Add(_node);
		unitScript.checkedList.Add(_node);
		unitScript.costList.Add(0);
		unitScript.hValue.Add(0);
		
		while(unitScript.openList.Count > 0)
		{
			
			Vector3 _n = LowestF();
			unitScript.openList.RemoveAt(unitScript.openList.IndexOf(_n));
			if(_n == _targetPos)
			{
				unitScript.openList.Clear();
				busy = false;
				BackTrackPath(_n);
				//return true;
				yield break;
			}
			
			for(int i = (int)_n.x - 1; i<(int)_n.x+2; i++)
			{
				for(int j = (int)_n.y - 1; j<(int)_n.y+2; j++)
				{
					int _gval;
					int _new_gval;
					int _new_hval;
					
					Vector3 _neighbor = new Vector3(i,j,0);
					if(_neighbor == _n)
					{
						continue;
					}
					if(i<0 || i > xBounds) //--Playarea max size
					{
						continue;
					}
					else if(j < 0 || j > yBounds)
					{
						continue;
					}
					
					if(noDiagonal)
					{
						if(j == (int)_n.y + 1 && i == (int)_n.x - 1)
						{
							continue;
						}
						else if(j == (int)_n.y + 1 && i == (int)_n.x + 1)
						{
							continue;
						}
						else if(j == (int)_n.y - 1 && i == (int)_n.x - 1)
						{
							continue;
						}
						else if(j == (int)_n.y - 1 && i == (int)_n.x + 1)
						{
							continue;
						}
					}
					//diaganol
					if(j == (int)_n.y + 1 && i == (int)_n.x - 1)
					{
						_gval = 14;
					}
					else if(j == (int)_n.y + 1 && i == (int)_n.x + 1)
					{
						_gval = 14;
					}
					else if(j == (int)_n.y - 1 && i == (int)_n.x - 1)
					{
						_gval = 14;
					}
					else if(j == (int)_n.y - 1 && i == (int)_n.x + 1)
					{
						_gval = 14;
					}
					else
					{
						_gval = 10;
					}
					
					//compute new g value
					_new_gval = unitScript.costList[unitScript.checkedList.IndexOf(_n)]+_gval;
					//compute Hvalue with euclidian or manhattan
					if(euclidian)
					{
						_new_hval = ComputeHValue_Euclid(_neighbor, _targetPos);
					}
					else
					{
						_new_hval = ComputeHValue_Manhattan(_neighbor, _targetPos);
					}
					
					if(!CheckNode(_neighbor))
					{
						continue;
					}
					else
					{
						if(OnOpenList(_neighbor))
						{
							if(_new_gval < unitScript.costList[unitScript.checkedList.IndexOf(_neighbor)])
							{
								unitScript.parentsList[unitScript.checkedList.IndexOf(_neighbor)] = _n;
								unitScript.costList[unitScript.checkedList.IndexOf(_neighbor)] = _new_gval;
								unitScript.hValue[unitScript.checkedList.IndexOf(_neighbor)] = _new_hval;
							}
						}
						else
						{
							unitScript.openList.Add(_neighbor);
							unitScript.parentsList.Add(_n);
							unitScript.costList.Add(_new_gval);
							unitScript.hValue.Add(_new_hval);
						}
					}
					
					yield return new WaitForFixedUpdate();
				}
			}
		}
		
		busy = false;
		unitScript.targetPos = unitScript.currentPos;
		unitScript.clearPath.Add (unitScript.targetPos);
		//return false;
	}
	
	bool OnOpenList(Vector3 _node)
	{
		for(int i = 0; i < unitScript.openList.Count; i++)
		{
			if(_node == unitScript.openList[i])
			{
				return true;
			}
		}
		
		return false;
	}
	
	Vector3 LowestF()
	{
		Vector3 _lowest = unitScript.openList[0];
		int cost = unitScript.costList[unitScript.checkedList.IndexOf(_lowest)];
		int heuristic;
		if(euclidian)
		{
			heuristic = ComputeHValue_Euclid(_lowest,unitScript.targetPos);
		}
		else
		{
			heuristic = ComputeHValue_Manhattan(_lowest, unitScript.targetPos);
		}
		int lowestFval = cost + heuristic;
		
		foreach(Vector3 _node in unitScript.openList)
		{
			cost = unitScript.costList[unitScript.checkedList.IndexOf(_node)];
			heuristic = unitScript.hValue[unitScript.checkedList.IndexOf(_node)];
			int fVal = cost + heuristic;
			if(fVal < lowestFval)
			{
				_lowest = _node;
				lowestFval = fVal;
			}
		}
		
		return _lowest;
	}
	
	#endregion
	
	//GETTERS AND SETTERS FOR PLAY AREA
	public GameObject GetPlayAreaObject()
	{
		return levelScript.playArea[pcXpos,pcYpos];
	}
	
	public GameObject GetPlayAreaObject(float x, float y)
	{
		if(levelScript.playArea[(int)x,(int)y] == null)
			return null;
		else return levelScript.playArea[(int)x,(int)y];
	}
	
	public void SetPlayAreaObject(GameObject go)
	{
		levelScript.playArea[pcXpos,pcYpos] = go;
	}
	//----END G+S-------------------
	
	
	
	
	
}
