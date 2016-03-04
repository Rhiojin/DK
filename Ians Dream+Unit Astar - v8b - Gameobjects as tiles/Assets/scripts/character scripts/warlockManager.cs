using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class warlockManager : MonoBehaviour {

	
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
	public warlockControl unitScript;
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
	
	//public float miningTime = 6f;
	
	public Vector3 pos = Vector3.zero;
	private Vector3 originalPos = Vector3.zero;
	
	private bool dead = false;
	public Sprite blood;
	
	//---combat vars
	
	public GameObject combatTarget;
	private bool engaged = false;
	private int hp = 100;
	private int atk = 2;
	private float atkCooldown = 0;
	private float delay = 2;
	//-----------------------------------ALL COORDINATES AND POSITIONS NEED TO BE INTS OR ROUNDED AT LEAST--------------------------
	
	void Start () 
	{
		
		//-----Script Grabs
		unitScript = transform.GetComponent<warlockControl>();
		levelScript = GameObject.Find("Level Manager").GetComponent<levelManager2>();	
		xBounds = levelScript.playArea.GetLength(0)-1;
		yBounds = levelScript.playArea.GetLength(1)-1;
		
		
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
	
	private void EngageCombat()
	{
		if(combatTarget)
		{
			hasOrders = true;
			waiting = false;
			computingPath = true;
			invalidPath = false;
			unitScript.clearPath.Clear();
			unitScript.SetListPos(0);
			if(Vector3.Distance(transform.position, combatTarget.transform.position)<=1.2f)
			{
				Fight();
			}
		}
		else
		{
			engaged = false;
			StartCoroutine( MeanderStarter() );
		}
	}
	
	private void Fight()
	{
		//if(atkCooldown <= 0) atkCooldown = Time.realtimeSinceStartup + delay;
		if(atkCooldown <= Time.realtimeSinceStartup)
		{
			combatTarget.SendMessage("TakeDamage",atk);
			atkCooldown = Time.realtimeSinceStartup + delay;
		}
		//waiting = false;
	}
	
	public void Kill()
	{
		if(!dead)
		{
			dead = true;
			GetComponent<SpriteRenderer>().sprite = blood;
			
			StopAllCoroutines();
			Destroy(gameObject,4);
		}
	}
	
	
	
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
					if(engaged)
					{
						EngageCombat();
					}
					else
					{
						unitScript.nextPos.x = unitScript.clearPath[unitScript.CheckListPos()+1].x;
						unitScript.nextPos.y = unitScript.clearPath[unitScript.CheckListPos()+1].y;
					}
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
				
				pathFound = false;
				waiting = true;
				
				if(engaged)
				{

					EngageCombat();
				}
				else
				{
					StartCoroutine( MeanderStarter() );
				}
			}
			
		}
		
		
		unitScript.currentPos = unitScript.nextPos;
		
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
			
			
			StartCoroutine( AStarPath2(_startPos, _targetPos) );
			
		}
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
	
	public void Engage(GameObject obj)
	{
		engaged = true;
		combatTarget = obj;
	}
	
	public void TakeDamage(int dmg)
	{
		hp-= dmg;
		print ("Knight hp: " + hp);
		if(hp <=0) Kill();
	}

}
