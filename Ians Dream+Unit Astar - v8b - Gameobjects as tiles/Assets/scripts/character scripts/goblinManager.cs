using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class goblinManager : MonoBehaviour {
	

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
	public goblinControl unitScript;
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

	//----Combat Vars
	public GameObject combatTarget;
	private Collider2D[] colliderSweep = new Collider2D[20];
	private Vector2 areaA = Vector2.zero;
	private Vector2 areaB = Vector2.zero;
	public bool checkingCombatSpot = false;
	private bool validSpot = false;

	private int hp = 100;
	private int atk = 3;
	private float atkCooldown = 0;
	private float delay = 1;
	private bool inCombat = false;

	private int fatigue = 0;
	private int hunger = 0;
	private bool eating = false;
	private bool sleeping = false;

	private GameObject foodTarget;
	private GameObject bedTarget;
	private stats statScript;

	//-----------------------------------ALL COORDINATES AND POSITIONS NEED TO BE INTS OR ROUNDED AT LEAST--------------------------
	
	void Start () 
	{
		
		//-----Script Grabs
		unitScript = transform.GetComponent<goblinControl>();
		levelScript = GameObject.Find("Level Manager").GetComponent<levelManager2>();	
		xBounds = levelScript.playArea.GetLength(0)-1;
		yBounds = levelScript.playArea.GetLength(1)-1;
		
		//levelScript.workers.Add(gameObject);
		//miningTime = levelScript.miningTime;
		
		pos = transform.position; 
		originalPos = transform.position;
		statScript = GetComponent<stats>();
		statScript.hp = hp;
		statScript.hunger = hunger;
		statScript.fatigue = fatigue;
		
		Meander();
		InvokeRepeating("Live",5,10);
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
			
			pos.x = Mathf.Round( Random.Range(20, 24));
			pos.y = Mathf.Round( Random.Range(20, 24));
			
			unitScript.targetPos = pos;
			
			
		}
		
	}

	void EngageCombat()
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
			
			StartCoroutine(GetCombatSpot());
			
			//unitScript.targetPos = pos;
			
			
		}
	}

	private void Fight()
	{
		//if(atkCooldown <= 0) atkCooldown = Time.realtimeSinceStartup + delay;
		if(atkCooldown < Time.realtimeSinceStartup)
		{
			combatTarget.SendMessage("TakeDamage",atk);
			atkCooldown = Time.realtimeSinceStartup + delay;
		}
		waiting = false;
	}

	public void Kill()
	{
		if(!dead)
		{
			dead = true;
			GetComponent<SpriteRenderer>().sprite = blood;

			StopAllCoroutines();
			Destroy(gameObject,1.5f);
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
		else // here is where decisions are made once a target is reached
		{

			if(hasOrders)
			{
				hasOrders = false;
				
			}
			if(waiting == false && !busy)
			{
				//print ("!");
				pathFound = false;
				waiting = true;

				if(eating)
				{
					EatFood();
				}
				else if(sleeping)
				{
					//sleep
				}

				else if(combatTarget)
				{
					if(Vector3.Distance(transform.position, combatTarget.transform.position) <=1.2f)
					{
						//print (Vector3.Distance(transform.position, combatTarget.transform.position));
						Fight();
					}
					else
					{
						EngageCombat();
					}
				}

				else if(CheckForEnemies())
				{
					EngageCombat();
				}
				else
				{
					if(hunger >= 100 && !eating)
					{
						Eat ();
					}
					else if(fatigue >= 100 &&  !sleeping)
					{
						Sleep();
					}
					else
					{
						StartCoroutine( MeanderStarter() );
					}
				}
			}
			
		}
		
		
		unitScript.currentPos = unitScript.nextPos;
	
	}
	
	
//	IEnumerator RevealBlock(int x, int y)
//	{
//		print("about to show block");
//		pathFound = false;
//		waiting = true;
//		
//		//Instantiate(levelScript.miningTimer,new Vector3((float)x,(float)y,-1),levelScript.miningTimer.transform.rotation);
//		yield return new WaitForSeconds(miningTime-1);
//		
//		if(levelScript.GetPlayAreaObject(x,y) != null)
//		{
//			if(levelScript.GetPlayAreaObject(x,y).name == "goldSoil(Clone)")
//			{
//				levelScript.UpdateGold(true, 15);
//			}
//		}
//		
//		levelScript.playArea[x,y] = Instantiate(levelScript.claimedFloorB,new Vector3((float)x,(float)y,0),levelScript.claimedFloorB.transform.rotation) as GameObject;
//		levelScript.RevealAdjacentTiles(x,y);
//		
//		
//		levelScript.workers.Add(gameObject);
//		
//	}
	
	

	
	#region A* pathfinding
	
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
	#endregion
	
	private bool CheckForEnemies()
	{
		//define the collision box: 5x5 squares around unit
		areaA.x = transform.position.x-2;
		areaA.y = transform.position.y+2;
		areaB.x = transform.position.x+2;
		areaB.y = transform.position.y-2;

		//areaA.x = transform.position.x;
		//areaA.y = transform.position.y;

		if(Physics2D.OverlapAreaNonAlloc(areaA,areaB,colliderSweep) > 0)
		//if(Physics2D.OverlapCircleNonAlloc(areaA,5,colliderSweep) > 0)
		{
			foreach(Collider2D obj in colliderSweep)
			{
				if(obj)
				{
					if(obj.gameObject.CompareTag("enemy"))
					{
						combatTarget = obj.gameObject;
						combatTarget.SendMessage("Engage", gameObject);
						//print("combat target: " + combatTarget.name);
						return true;
					}
				}
			}
			//if no collider was an enemy
			combatTarget = null;
			return false;
		}
		else
		{
			combatTarget = null;
			return false;
		}
	}

	IEnumerator GetCombatSpot()
	{
		checkingCombatSpot = true;

		Vector3 order = combatTarget.transform.position;
		//print (CheckTheList(order));

				//print(obj.name);
				for(int i = (int)order.x - 1; i<(int)order.x+2; i++)
				{
					for(int j = (int)order.y - 1; j<(int)order.y+2; j++)
					{
						
						if(i < 0 || i > levelScript.playArea.GetLength(0)-1 || j < 0 || j > levelScript.playArea.GetLength(1)-1)
						{
							//out of bounds
							continue;
						}
						else
						{
							if(levelScript.playArea[i,j] == null)
							{
								continue;
							}
							else if(levelScript.playArea[i,j].CompareTag("occupied"))
							{
								continue;
							}
							
							
							else if(levelScript.playArea[i,j].transform.position == order)
							{
								continue;
							}
							
							// dont check diagonalss 
							else if(j == (int)order.y + 1 && i == (int)order.x - 1)
							{
								continue;
							}
							else if(j == (int)order.y + 1 && i == (int)order.x + 1)
							{
								continue;
							}
							else if(j == (int)order.y - 1 && i == (int)order.x - 1)
							{
								continue;
							}
							else if(j == (int)order.y - 1 && i == (int)order.x + 1)
							{
								continue;
							}
							
							//-----------------------------------
							
							else if(levelScript.playArea[i,j].CompareTag("unoccupied"))
							{
								//print ("succesful return");
								validSpot = true;
								
								pos = levelScript.playArea[i,j].transform.position;
								unitScript.targetPos = pos;
								
								
								//workOrders.Add(workFromPoint);
								break;
							}
						}
						
						yield return new WaitForFixedUpdate();
						
					}
			}


			
			if(validSpot!= true)
			{
				//print ("it probably broke");
				//validWorkOrder = false;

				pos = transform.position;

			}
			
			checkingCombatSpot = false;

	}

	public void TakeDamage(int dmg)
	{
		hp-= dmg;
		statScript.hp-=dmg;
		//print ("Goblin hp: " + hp);
		levelScript.SpawnFCT(transform.position, dmg);
		if(hp <=0) Kill();
	}

//	public void ReportStats()
//	{
//		levelScript.currentTargetHp = hp;
//		levelScript.currentTargetHunger = hunger;
//		levelScript.currentTargetFatigue = fatigue;
//		levelScript.currentTargetUnit = gameObject;
//
//		levelScript.ShowUnitStats();
//	}

	private void Live()
	{
		hunger+=5;
		statScript.hunger+=5;
		fatigue+=2;
		statScript.fatigue+=2;

		if(hunger >= 100)
		{
			hunger = 100;
			statScript.hunger=100;
		}

		if(fatigue >= 100)
		{
			fatigue = 100;
			statScript.fatigue = 100;
		}
	}

	private void Eat()
	{
	
		foodTarget = GameObject.Find("UnitChicken(Clone)");


		if(foodTarget)
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
				pos.x = Mathf.Round(foodTarget.transform.position.x);
				pos.y = Mathf.Round(foodTarget.transform.position.y);
				unitScript.targetPos = pos;
				eating = true;
			}
		}
		else
		{
			waiting = false;
		}
		

	}

	void EatFood()
	{
		foodTarget.SendMessage("Kill");
		eating = false;
		hunger = 0;
		statScript.hunger = 0;
		waiting = false;
	}

	private void Sleep()
	{

	}

	private void SleepInBed()
	{

	}
//	void OnDrawGizmosSelected() {
//		Gizmos.color = Color.yellow;
//		Gizmos.DrawWireSphere(transform.position, 5);
//	}

}
