using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class levelManager : MonoBehaviour {


	public GameObject pc;
	public GameObject crate;
	public GameObject[,] playArea;
	public GameObject instancedPc;


	public GameObject marker;
	public GameObject markerHolder;
	public Material mat_valid;
	public Material mat_invalid;
	public GameObject checkedMarker;

	public bool pause;
	public bool invalidPath = false;
	public bool computingPath = false;
	public bool busy = false;
	private Vector3 nullVector = new Vector3(-1,-1,-1);
	public bool targetFound = false;
	public bool showDFSMarkers = false;

	//--A* pathfinding Options
	private bool noDiagonal = true;
	private bool euclidian = false;

	private Vector3 clickPoint;

	//---------
	public List<GameObject> buildings;
	private bool placingBuilding;
	public GameObject buildingToBuild;

	//---------Script Grabs
	public unitAI unitScript;
	private structurePlacement buildingScript;

	public int pcXpos;
	public int pcYpos;
	private int xBounds = 0;
	private int yBounds = 0;

	//---
	private bool guiClicked = false;
	public GUITexture buildButton;
	public GameObject unit1;

	public List<Vector3> workOrders = new List<Vector3>();

	void Awake()
	{
		playArea = new GameObject[100,100];
		//FillWorkOrders();

	}

	void Start () 
	{
		//create array and instantiate playable character

		instancedPc = Instantiate(pc, new Vector3(20,20,0), transform.rotation) as GameObject;
		//-place pc in array
		playArea[20,20] = instancedPc;

		//-----Script Grabs
		unitScript = instancedPc.GetComponent<unitAI>();
		buildingScript = transform.GetComponent<structurePlacement>();

		//--set the camera script to the pc as a target
		Camera.main.GetComponent<cameraScript>().target = instancedPc.transform;
		//--unpause game
		pause = false;

		xBounds = playArea.GetLength(0)-1;
		yBounds = playArea.GetLength(1)-1;

		//----Populate Grid --
		AddObjects(990,999);

	}
	
	// Update is called once per frame
	void Update () 
	{
		GetGUIClick();

		if(!guiClicked)GetMouseClick();


		if(computingPath != true)
		{
			UpdatePCPositionFromList();
		}
		else
		{
			//ComputePCPath();
			//ComputePCPathWithBFS(unitScript.currentPos, unitScript.targetPos);
			ComputePCPathWithAStar(unitScript.currentPos, unitScript.targetPos);
		}

		//--toggle pause
		if(Input.GetKeyDown(KeyCode.P)) pause = !pause;
		if(Input.GetKeyDown(KeyCode.M)) showDFSMarkers = !showDFSMarkers;
		if(Input.GetKeyDown(KeyCode.D)) noDiagonal = !noDiagonal;
		if(Input.GetKeyDown(KeyCode.E)) euclidian = !euclidian;

		//--easy access to building functionality
		StartBuilding();
	}

	void GetMouseClick()
	{
		if(!placingBuilding)
		{
			if(Input.GetMouseButtonDown(0) && !computingPath && !unitScript.moving)
			{
				computingPath = true;
				invalidPath = false;
				unitScript.clearPath.Clear();
				unitScript.SetListPos(0);
				unitScript.prevState = unitScript.GetState();
				unitScript.SetState("Searching");

				//--Get input
				clickPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);

				//Round clickpoint
				clickPoint = new Vector3(Mathf.Round(clickPoint.x), Mathf.Round(clickPoint.y), 0);

				//clickpoint clamp
				if(clickPoint.x < 0)
				{
					clickPoint.x = 0;
				}
				if(clickPoint.x > xBounds)
				{
					clickPoint.x = xBounds;
				}
				if(clickPoint.y < 0)
				{
					clickPoint.y = 0;
				}
				if(clickPoint.y > yBounds)
				{
					clickPoint.y = yBounds;
				}

				if(GetPlayAreaObject(clickPoint.x,clickPoint.y) == null)
				{
					unitScript.currentVector = (clickPoint-instancedPc.transform.position).normalized;
					unitScript.targetPos = clickPoint;
				}

				if(markerHolder == null)
				{
					markerHolder = Instantiate(marker, clickPoint, marker.transform.rotation) as GameObject;
				}
				else
				{
					markerHolder.transform.position = clickPoint;
				}
			}
		}

		else if(Input.GetMouseButtonDown(0) && placingBuilding)
		{
			Vector3 _clickPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			//Round clickpoint
			_clickPoint = new Vector3(Mathf.Round(_clickPoint.x), Mathf.Round(_clickPoint.y), 0);

			if(buildingScript.AddBuilding(buildingToBuild.tag,(int)_clickPoint.x, (int)_clickPoint.y))
			{
				placingBuilding = false;
				Destroy(buildingToBuild,0.3f);
				pause = false;
			}
		}

		if(placingBuilding)
		{
			Vector3 _clickPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			//Round clickpoint
			_clickPoint = new Vector3(Mathf.Round(_clickPoint.x), Mathf.Round(_clickPoint.y), 0);

			if(buildingToBuild == null)
			{
				buildingToBuild = Instantiate(buildingScript.currentBuilding, _clickPoint, buildingScript.currentBuilding.transform.rotation) as GameObject;
			}
			else
			{
				buildingToBuild.transform.position = _clickPoint;
			}
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

	void UpdateUnitPosition()
	{
		if(unitScript.targetPos.x > unitScript.currentPos.x) unitScript.nextPos.x = unitScript.currentPos.x+1;
		if(unitScript.targetPos.x < unitScript.currentPos.x) unitScript.nextPos.x = unitScript.currentPos.x-1;
		if(unitScript.targetPos.y > unitScript.currentPos.y) unitScript.nextPos.y = unitScript.currentPos.y+1;
		if(unitScript.targetPos.y < unitScript.currentPos.y) unitScript.nextPos.y = unitScript.currentPos.y-1;

		unitScript.CheckForCollision();

		if(playArea[(int)unitScript.nextPos.x, (int)unitScript.nextPos.y] == null && !unitScript.moving)
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
			//print(unitScript.CheckListPos()-1);
			//int tempListPos = unitScript.CheckListPos()-1;
			//if(tempListPos < 0) tempListPos = 0;
			//unitScript.nextPos.x = unitScript.clearPath[tempListPos].x; 
			//unitScript.nextPos.x = unitScript.clearPath[unitScript.CheckListPos()-1].x; 

			unitScript.nextPos.x = unitScript.clearPath[unitScript.CheckListPos()+1].x;
			unitScript.nextPos.y = unitScript.clearPath[unitScript.CheckListPos()+1].y;


		}

		if(Mathf.Abs(dx) < 0.1f && Mathf.Abs(dy) < 0.1f)
		{
			foreach(GameObject n in unitScript.pathMarksList)
				GameObject.Destroy(n, 0.3f);
			if(showDFSMarkers && !pause)
			{
				foreach(GameObject n2 in unitScript.markCheckedList)
					GameObject.Destroy(n2, 3f);
			}

			unitScript.pathMarksList.Clear();
			unitScript.markCheckedList.Clear();
		}

//		if(Mathf.Abs(dy) > 0.1f)
//		{
//			int tempListPos = unitScript.CheckListPos()-1;
//			if(tempListPos < 0) tempListPos = 0;
//			unitScript.nextPos.y = unitScript.clearPath[tempListPos].y; 
//			//unitScript.nextPos.y = unitScript.clearPath[unitScript.CheckListPos()-1].y; 
//		}

		//--Check if intended next spot is valid
		if(playArea[(int)unitScript.nextPos.x, (int)unitScript.nextPos.y] == null && !unitScript.moving)
		{
			playArea[(int)unitScript.currentPos.x, (int)unitScript.currentPos.y] = null;
			playArea[(int)unitScript.nextPos.x, (int)unitScript.nextPos.y] = instancedPc;
			unitScript.currentPos = unitScript.nextPos;

		}
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
			markerHolder.GetComponent<Renderer>().material = mat_invalid;
		}
		else
		{
			markerHolder.GetComponent<Renderer>().material = mat_valid;
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

					//--IF OUT OF PLAY AREA BOUNDS. CHANGE FOR SMALLER OR LARGER PLAYAREA[]
					if(i < 0 || i > 99)
					{
						continue;
					}
					else if(j < 0 || j > 99)
					{
						continue;
					}

					if(!CheckNode(neighbour))
					{
						continue;
					}

					unitScript.parentsList.Add(n);
					unitScript.openList.Add(neighbour);

					if(showDFSMarkers)
					{
						unitScript.markCheckedList.Add(Instantiate(checkedMarker, new Vector3(neighbour.x,neighbour.y,0), checkedMarker.transform.rotation) as GameObject);
					}
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
		if(playArea[(int)_node.x, (int)_node.y] == null)
		{
			unitScript.checkedList.Add(_node);
			return true;
		}
		else
		{
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

		//--loop to visualize path
		foreach(Vector3 _n in unitScript.clearPath)
		{
			unitScript.pathMarksList.Add(Instantiate(checkedMarker, new Vector3(_n.x, _n.y,0), checkedMarker.transform.rotation)as GameObject);
		}

		unitScript.clearPath = _temp;
		computingPath = false;
		unitScript.SetState(unitScript.prevState);
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
			if(AStarPath(_startPos, _targetPos))
			{
				Debug.Log("No. Nodes checked with A* = " + unitScript.checkedList.Count);
			}
			else
			{
				Debug.Log("target not found!!");
			}
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
					if(i<0 || i > 99) //--Playarea max size
					{
						continue;
					}
					else if(j < 0 || j > 99)
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
					if(showDFSMarkers)
					{
						unitScript.markCheckedList.Add(Instantiate(checkedMarker, new Vector3(_neighbor.x, _neighbor.y,0), checkedMarker.transform.rotation)as GameObject);
					}
				}
			}
		}

		busy = false;
		unitScript.targetPos = unitScript.currentPos;
		unitScript.clearPath.Add (unitScript.targetPos);
		return false;
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
		return playArea[pcXpos,pcYpos];
	}

	public GameObject GetPlayAreaObject(float x, float y)
	{

		if(playArea[(int)x,(int)y] == null)
		   return null;
		else return playArea[(int)x,(int)y];
	}

	public void SetPlayAreaObject(GameObject go)
	{
		playArea[pcXpos,pcYpos] = go;
	}
	//----END G+S-------------------


	void AddObjects(int min, int max)
	{
		GameObject[] crateCount = new GameObject[Random.Range(min,max)];
		for(int i = 0; i< crateCount.Length; i++)
		{
			int randX = Random.Range(0,playArea.GetLength(0));
			int randY = Random.Range(0,playArea.GetLength(1));
			
			if(playArea[randX,randY] == null)
			{
				playArea[randX,randY] = Instantiate (crate, new Vector3((float)randX,(float)randY,0), transform.rotation) as GameObject;
			}

		}

	}

	void StartBuilding()
	{
		if(Input.GetKeyDown(KeyCode.B))
		{
			placingBuilding = true;
			buildingScript.currentBuilding = buildingScript.hut;
			//pause = true;
		}

		if(Input.GetKeyDown(KeyCode.U))
		{
			Vector3 unitPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			
			//Round clickpoint
			unitPoint = new Vector3(Mathf.Round(unitPoint.x), Mathf.Round(unitPoint.y), 0);
			
			//----PlayArea Clamp
			if(unitPoint.x < 0)
			{
				unitPoint.x = 0;
			}
			if(unitPoint.x > xBounds)
			{
				unitPoint.x = xBounds;
			}
			if(unitPoint.y < 0)
			{
				unitPoint.y = 0;
			}
			if(unitPoint.y > yBounds)
			{
				unitPoint.y = yBounds;
			}

			playArea[(int)unitPoint.x,(int)unitPoint.y] = Instantiate (unit1, unitPoint, unit1.transform.rotation) as GameObject;
		}

		if(Input.GetKeyDown(KeyCode.F))
		{
			FillWorkOrders();
		}

	}

	void GetGUIClick()
	{
		if(buildButton.HitTest(Input.mousePosition))
		{
			guiClicked = true;
			placingBuilding = true;
			buildingScript.currentBuilding = buildingScript.hut;
			pause = true;
		}
		else
		{
			guiClicked = false;
		}
	}

	void FillWorkOrders()
	{
		int _randX = Random.Range(0,playArea.GetLength(0)-1);
		int _randY = Random.Range(0,playArea.GetLength(1)-1);
		Vector3 workOrder = new Vector3((float)_randX,(float)_randY,0);
		workOrders.Add(workOrder);
	}
}
