using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class levelManager2 : MonoBehaviour {
	
	public GameObject[,] playArea;
	public Sprite[] goldSoils = new Sprite[9];
	public Sprite[] soils = new Sprite[18];
	public Sprite[] waters = new Sprite[9];
	public Sprite[] rocks = new Sprite[9];


	//----
	private int totalGold = 50;
	public Text goldCounter;
	public Text skullCounter;
	public GameObject miningTimer;
	public GameObject theAtlar;
	public Sprite altarFired;
	public Sprite altarNormal;

	//--Tile Prefabs 
	public GameObject highlighter;
	private GameObject h_lHolder;

	public GameObject water;
	public GameObject rock;
	public GameObject crate;
	public GameObject goldSoil;
	public GameObject soil;
	public GameObject freshSoil;
	public GameObject claimedFloor;
	public GameObject claimedFloorB;
	public GameObject altarL;
	public GameObject altarM;
	public GameObject altarR;

	private GameObject tileHolder;
	

	public bool pause;

	public bool busy = false;

	
	private Vector3 clickPoint;
	
	//---------
	public List<GameObject> buildings;
	private bool placingBuilding = false;
	public GameObject buildingToBuild;
	public GameObject unitToBuild;
	private bool placingUnit = false;
	private bool destroying = false;
	
	//---------Script Grabs

	private structurePlacement buildingScript;
	

	private int xBounds = 0;
	private int yBounds = 0;
	
	//---BUTTONS-----------------
	public bool guiClicked = false;
	public Color buttonHighlight;
	private GUITexture currentButton;
	public GUITexture toggleMineButton;
	public GUITexture buildImpButton;
	public GUITexture destroyButton;


	//------------
	public GUITexture buildBedButton;
	public GUITexture buildCoopButton;
	//-------------------------

	public RectTransform buildConfirmMenu;
	public GUITexture confirmButton;
	public GUITexture cancelButton;
	private Vector3 b_c_menuOnscreen = new Vector3(0,145,0);
	private Vector3 b_c_menuOffscreen = new Vector3(-500,-0.2F,0);

	public RectTransform toggleBuildActions;
	public RectTransform toggleSpellActions;
	public RectTransform toggleTrapActions;
	private bool toggle_build_actionOnscreen = false;
	private bool toggle_spell_actionOnscreen = false;
	private bool toggle_trap_actionOnscreen = false;

	private Vector3 build_actionOnscreen = new Vector3(-221,140,0);
	private Vector3 spell_actionOnscreen = new Vector3(-221,32,0);
	private Vector3 trap_actionOnscreen = new Vector3(1,1,0);
	private Vector3 offscreen = new Vector3(-520,1,0);
	//----

	//-------UNITS-----------
	public GameObject unitObj;
	public GameObject imp;
	public GameObject goblin;
	public GameObject mage;

	public GameObject knight;

	public GameObject bed;
	public GameObject chickenCoop;

	//----------------------=

	//----MAX UNIT COUNTS
	public int impCurrentCount = 0;
	public int impMaxCount = 5;
	public int chickenCurrentCount = 0;
	public int chickenMaxCount = 10;

	//-------------------

	public List<Vector2> bedLocations = new List<Vector2>();

	public List<Vector3> workOrders = new List<Vector3>();
	public List<Vector3> minedBlocks = new List<Vector3>();
	public List<Vector3> delayedWorkOrders = new List<Vector3>();
	public List<GameObject> workers = new List<GameObject>();

	public Vector3 workFromPoint = Vector3.zero;
	private bool checkingDelays = false;
	private bool checkingOrder = false;
	private bool validWorkOrder = false;

	private bool validDelayedWorkOrder = false;
	private Vector3 delayedWorkFromPoint = Vector3.zero;

	private int mainBaseBuildPoint = 20; //Play area / 2

	public bool mining = false;

	public float miningTime = 6f;

	private Vector3 buildPoint = Vector3.zero;

//	public int currentTargetHp;
//	public int currentTargetHunger;
//	public int currentTargetFatigue;
	public GameObject currentTargetUnit;
	private stats statScript;

	public Text toolTipHp;
	public Text toolTipHunger;
	public Text toolTipFatigue;
	private Vector3 toolTipOnscreen = new Vector3(-86,58,0);
	private Vector3 toolTipOffscreen = new Vector3(95,58,0);
	public RectTransform toolTip;

	//------------------
	private RaycastHit2D hit;
	private Vector3 ray;
	public GameObject FCT;
	private GameObject FCTobj;

	void Awake()
	{
		playArea = new GameObject[40,40];

		
	}
	
	void Start () 
	{
		//--Script grab
		buildingScript = transform.GetComponent<structurePlacement>();
		
		//--unpause game
		pause = false;

		//----Populate Grid --build main base --> gold blocks --> fill rest of map

		StartCoroutine( BuildMainBase() ); //also fills array with fluff soil

		//--cache play area bounds
		xBounds = playArea.GetLength(0)-1;
		yBounds = playArea.GetLength(1)-1;

		goldCounter.text = totalGold.ToString();
		//currentButton = toggleMineButton;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		//GetGUIClick();
		if(placingBuilding)
		{
			buildConfirmMenu.anchoredPosition = b_c_menuOnscreen;
		}
		else buildConfirmMenu.anchoredPosition = b_c_menuOffscreen;


		if(!EventSystem.current.IsPointerOverGameObject())GetMouseClick();
		

		
		//--toggle pause
		if(Input.GetKeyDown(KeyCode.P)) pause = !pause;
		
		//continuously attempt to dish out orders to slacking imps
		GiveOrders();
		//ShowUnitStats();
	}

//	void GetGUIClick()
//	{
//		if(Input.GetMouseButtonDown(0))
//		{
//			if(guiClicked) guiClicked = false;
//
//
//			//---------Buttons-----------------
//			if(toggleMineButton.HitTest(Input.mousePosition))
//			{
//
//				//if(mining) toggleMineButton.color = buttonHighlight;
//				//else toggleMineButton.color = Color.grey;
//				ButtonHighlighter(toggleMineButton);
//				mining = !mining;
//
//				guiClicked = true;
//			}
//			
//			if(buildImpButton.HitTest(Input.mousePosition))
//			{
//				ButtonHighlighter(buildImpButton);
//				SpawnUnit("imp");
//				//unitObj = imp;
//				//placingUnit = true;
//				//pause = true;
//			}
//
//			if(destroyButton.HitTest(Input.mousePosition))
//			{
//				ButtonHighlighter(destroyButton);
//				destroying = !destroying;
//				guiClicked = true;
//			}
//
//
//			//-----------------Buildings
//
//
//
//			if(buildBedButton.HitTest(Input.mousePosition))
//			{
//				ButtonHighlighter(buildBedButton);
//
//				guiClicked = true;
//
//				buildingScript.currentBuilding = bed;
//				buildingToBuild = Instantiate(buildingScript.currentBuilding, buildPoint, buildingScript.currentBuilding.transform.rotation) as GameObject;
//				buildingToBuild.name = "BuildingBed";
//				if(!placingBuilding) placingBuilding = true;
//
//
//				pause = true;
//			}
//
//			if(buildCoopButton.HitTest(Input.mousePosition))
//			{
//				ButtonHighlighter(buildCoopButton);
//
//				guiClicked = true;
//
//				buildingScript.currentBuilding = chickenCoop;
//				buildingToBuild = Instantiate(buildingScript.currentBuilding, buildPoint, buildingScript.currentBuilding.transform.rotation) as GameObject;
//				buildingToBuild.name = "BuildingChickenCoop";
//				if(!placingBuilding) placingBuilding = true;
//				pause = true;
//			}
//
//
//			//------------------menu------------
//			if(confirmButton.HitTest(Input.mousePosition))
//			{
//
//				if(buildingScript.AddBuilding(buildingToBuild.name,(int)buildPoint.x, (int)buildPoint.y))
//				{
//					placingBuilding = false;
//					//Destroy(buildingToBuild,0.3f);
//					buildingToBuild = null;
//					pause = false;
//					ButtonHighlighter(null);
//				}
//			}
//
//			if(cancelButton.HitTest(Input.mousePosition))
//			{
//				placingBuilding = false;
//				Destroy(buildingToBuild);
//				buildingToBuild = null;
//				pause = false;
//				ButtonHighlighter(null);
//
//			}
//
//		}
//		if(placingBuilding)
//		{
//			buildConfirmMenu.anchoredPosition = b_c_menuOnscreen;
//		}
//		else buildConfirmMenu.anchoredPosition = b_c_menuOffscreen;
//		
//
//		
//	}

	// -=--------------Button functions

	public void ToggleBuildActions()
	{
		if(!toggle_build_actionOnscreen)
		{
			toggleBuildActions.anchoredPosition = build_actionOnscreen;
			toggle_build_actionOnscreen = true;
		}
		else 
		{
			toggleBuildActions.anchoredPosition = offscreen;
			toggle_build_actionOnscreen = false;
		}
	}

	public void ToggleSpellActions()
	{
		if(!toggle_spell_actionOnscreen)
		{
			toggleSpellActions.anchoredPosition = spell_actionOnscreen;
			toggle_spell_actionOnscreen = true;
		}
		else
		{
			toggleSpellActions.anchoredPosition = offscreen;
			toggle_spell_actionOnscreen = false;
		}
	}


	public void ToggleMiningButton()
	{
		//ButtonHighlighter(toggleMineButton);
		mining = !mining;
	}

	public void BuildImpButton()
	{
		//ButtonHighlighter(buildImpButton);
		SpawnUnit("imp");
	}

	public void DestroyButton()
	{
		//ButtonHighlighter(destroyButton);
		destroying = !destroying;
	}

	public void BuildBedButton()
	{
		//ButtonHighlighter(buildBedButton);
		buildingScript.currentBuilding = bed;
		buildingToBuild = Instantiate(buildingScript.currentBuilding, buildPoint, buildingScript.currentBuilding.transform.rotation) as GameObject;
		buildingToBuild.name = "BuildingBed";
		if(!placingBuilding) placingBuilding = true;
		
		
		pause = true;
	}

	public void BuildCoopButton()
	{
		//ButtonHighlighter(buildCoopButton);
		
		buildingScript.currentBuilding = chickenCoop;
		buildingToBuild = Instantiate(buildingScript.currentBuilding, buildPoint, buildingScript.currentBuilding.transform.rotation) as GameObject;
		buildingToBuild.name = "BuildingChickenCoop";
		if(!placingBuilding) placingBuilding = true;
		pause = true;
	}

	public void ConfirmBuildButton()
	{
		if(buildingScript.AddBuilding(buildingToBuild.name,(int)buildPoint.x, (int)buildPoint.y))
		{
			placingBuilding = false;
			//Destroy(buildingToBuild,0.3f);
			buildingToBuild = null;
			pause = false;
			//ButtonHighlighter(null);
		}
	}

	public void CancelBuildButton()
	{
		placingBuilding = false;
		Destroy(buildingToBuild);
		buildingToBuild = null;
		pause = false;
		//ButtonHighlighter(null);
	}




	//------------------End
	void ButtonHighlighter(GUITexture button)
	{
		if(currentButton)currentButton.color = Color.gray;
		//if(mining) mining = false;

		if(button && button != currentButton)
		{
			if(button.color != buttonHighlight)
			{
				button.color = buttonHighlight;
			}
			else button.color = Color.gray;

			currentButton = button;
		}
	}

	void BringObjectToWorld()
	{
		if(!placingBuilding)
		{
			buildingToBuild = Instantiate(buildingScript.currentBuilding, buildPoint, buildingScript.currentBuilding.transform.rotation) as GameObject;
			placingBuilding = true;
		}
	}

	void GetMouseClick()
	{
		//--Turn this into build on button click
		if(Input.GetMouseButtonDown(0))
		{
			CancelInvoke();
			currentTargetUnit = null;
			statScript = null;
			toolTip.anchoredPosition = toolTipOffscreen;

			//check placing first to avoid both placing a uniot and issuing an order
			if(!placingBuilding && !placingUnit)
			{
				
				//--Get input
				clickPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				
				//Round clickpoint
				clickPoint = new Vector3(Mathf.Round(clickPoint.x), Mathf.Round(clickPoint.y), 0);
				
				//----PlayArea Clamp
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
				
				if(destroying)
				{
					ray = Camera.main.ScreenToWorldPoint (Input.mousePosition);
					
					hit = Physics2D.Raycast(ray,-Vector2.up/2,0.1f);
					if(hit.collider)
					{
						
						if(hit.collider.name.Contains("Building")) 
						{
							if(hit.collider.name.Contains("Bed"))
							{
								bedLocations.Remove(new Vector2(hit.collider.transform.position.x,hit.collider.transform.position.y));
							}
							Destroy(hit.collider.gameObject);
							playArea[(int)clickPoint.x,(int)clickPoint.y] = claimedFloorB;
						}
						
						
					}
				}


				//Vector3 workOrder = new Vector3((float)_randX,(float)_randY,0);
				//workOrders.Add(workOrder);

				//Add clickpoint as a work issue order if mininf flag is set
				if(mining && !destroying)
				{
					print ("mining");
					if(!checkingOrder) StartCoroutine( CheckWorkorder(clickPoint) );
					
					if(validWorkOrder)
					{
						validWorkOrder = false;
						validDelayedWorkOrder = false;
						workFromPoint = new Vector3(Mathf.Round(workFromPoint.x), Mathf.Round(workFromPoint.y), 0);
						workOrders.Add(workFromPoint);
						minedBlocks.Add(clickPoint);

						h_lHolder = Instantiate(highlighter,new Vector3(clickPoint.x,clickPoint.y,-0),highlighter.transform.rotation) as GameObject;
						h_lHolder.GetComponent<Renderer>().sortingLayerName = "characters";
						//Instantiate(crate,workFromPoint,crate.transform.rotation);
					}

					//--Need to fully update delayed mining functionality with a delayed minedblocks list

	//				else
	//				{
	//					clickPoint = new Vector3(Mathf.Round(clickPoint.x), Mathf.Round(clickPoint.y), 0);
	//					delayedWorkOrders.Add(clickPoint);
	//
	//				}

//					ray = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//					
//					hit = Physics2D.Raycast(ray,-Vector2.up/2,0.1f);
//					if(hit.collider)
//					{
//
//						
//						if(hit.collider.name.Contains("h_l")) 
//						{
//							Destroy (hit.collider.gameObject);
//							RemoveFromList(new Vector3(Mathf.Round(ray.x), Mathf.Round(ray.y), 0));
//						}
//					}
				}
				
			}

			if(placingBuilding && !destroying)
			{
				buildPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				//Round clickpoint
				buildPoint = new Vector3(Mathf.Round(buildPoint.x), Mathf.Round(buildPoint.y), 0);

				if(buildingToBuild)
				{
					buildingToBuild.transform.position = buildPoint;
					//print ("!build move");
				}


			}

			if(placingUnit && !destroying)
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

				if(playArea[(int)unitPoint.x,(int)unitPoint.y].tag == "unoccupied")
				{
					playArea[(int)unitPoint.x,(int)unitPoint.y] = Instantiate (unitObj, unitPoint, unitObj.transform.rotation) as GameObject;
					placingUnit = false;
					pause = false;
					ButtonHighlighter(null);
				}
			}


			ray = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			hit = Physics2D.Raycast(ray,-Vector2.up/2,0.1f);
			if(hit.collider)
			{
				//print ("!");
				//print (hit.collider.name);
				if(hit.collider.name.Contains("UnitChicken")) 
				{
					hit.collider.GetComponent<chickenManager>().Kill();
				}

				else if(hit.collider.name.Contains("Unit"))
				{

					//hit.collider.gameObject.SendMessage("ReportStats",SendMessageOptions.DontRequireReceiver);
					currentTargetUnit = hit.collider.gameObject;
					if(hit.collider.transform.GetComponent<stats>())
					{
						toolTip.anchoredPosition = toolTipOnscreen;
						statScript = hit.collider.transform.GetComponent<stats>();
						InvokeRepeating("ShowUnitStats",0.01f, 0.2f);
					}
				}

			}


		}


		//Bring object into world for visual placement
//		if(placingBuilding && !destroying)
//		{
//			Vector3 _clickPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
//			//Round clickpoint
//			_clickPoint = new Vector3(Mathf.Round(_clickPoint.x), Mathf.Round(_clickPoint.y), 0);
//			
//			if(buildingToBuild == null)
//			{
//				buildingToBuild = Instantiate(buildingScript.currentBuilding, _clickPoint, buildingScript.currentBuilding.transform.rotation) as GameObject;
//				print ("!build make");
//			}
////			else
////			{
////				buildingToBuild.transform.position = _clickPoint;
////			}
//		}
	}
	

	
	//GETTERS AND SETTERS FOR PLAY AREA
//	public GameObject GetPlayAreaObject()
//	{
//		return playArea[pcXpos,pcYpos];
//	}
	
	public GameObject GetPlayAreaObject(float x, float y)
	{
		
		if(playArea[(int)x,(int)y] == null)
			return null;
		else return playArea[(int)x,(int)y];
	}
	
//	public void SetPlayAreaObject(GameObject go)
//	{
//		playArea[pcXpos,pcYpos] = go;
//	}
	//----END G+S-------------------
	
	
	void AddObjects(int min, int max)
	{
		//Gold
		int crateCount = Random.Range(min,max);
		for(int i = 0; i< crateCount; i++)
		{
			int randX = Random.Range(0,playArea.GetLength(0));
			int randY = Random.Range(0,playArea.GetLength(1));
			int randG = Random.Range(0, goldSoils.Length);
			
			if(playArea[randX,randY] == null)
			{
				tileHolder = Instantiate (goldSoil, new Vector3((float)randX,(float)randY,0), goldSoil.transform.rotation) as GameObject;
				tileHolder.GetComponent<SpriteRenderer>().sprite = goldSoils[randG];
				playArea[randX,randY] = tileHolder;
			}
			
		}
		//water
		crateCount = Random.Range(15,30);
		for(int i = 0; i< crateCount; i++)
		{
			int randX = Random.Range(0,playArea.GetLength(0));
			int randY = Random.Range(0,playArea.GetLength(1));
			int randG = Random.Range(0, goldSoils.Length);
			
			if(playArea[randX,randY] == null)
			{
				tileHolder = Instantiate (water, new Vector3((float)randX,(float)randY,0), water.transform.rotation) as GameObject;
				tileHolder.GetComponent<SpriteRenderer>().sprite = waters[randG];
				playArea[randX,randY] = tileHolder;
			}
			
		}

		//rock
		crateCount = Random.Range(15,30);
		for(int i = 0; i< crateCount; i++)
		{
			int randX = Random.Range(0,playArea.GetLength(0));
			int randY = Random.Range(0,playArea.GetLength(1));
			int randG = Random.Range(0, rocks.Length);
			
			if(playArea[randX,randY] == null)
			{
				tileHolder = Instantiate (rock, new Vector3((float)randX,(float)randY,0), rock.transform.rotation) as GameObject;
				tileHolder.GetComponent<SpriteRenderer>().sprite = rocks[randG];
				playArea[randX,randY] = tileHolder;
			}
			
		}
		
	}
	


	IEnumerator BuildMainBase()
	{
		yield return new WaitForSeconds(0.3f);
		//buildingScript.currentBuilding = buildingScript.mainBase;
		//buildingToBuild = Instantiate(buildingScript.currentBuilding, Vector3.zero, buildingScript.currentBuilding.transform.rotation) as GameObject;
		//buildingScript.AddBuilding("mainBase",(int)playArea.GetLength(0)/2, (int)playArea.GetLength(1)/2);
		buildingScript.AddBuilding("mainBase",mainBaseBuildPoint, mainBaseBuildPoint);
		//Destroy(buildingToBuild,0.3f);
		buildingToBuild = null;
		AddObjects(20,50);

		BuildMap();
	}

	void BuildMap()
	{
		for(int x = 0; x < playArea.GetLength(0); x++)
		{
			for(int y = 0; y < playArea.GetLength(1); y++)
			{
				if(playArea[x,y] == null)
				{
					//int s = Random.Range(0,18);
					//tileHolder = Instantiate (soil, new Vector3((float)x,(float)y,0), soil.transform.rotation) as GameObject;
					//tileHolder.GetComponent<SpriteRenderer>().sprite = soils[s];
					//playArea[x,y] = tileHolder;
					playArea[x,y] = soil;
				}
			}
		}

		StartCoroutine( SpawnStuff() );
	}




	void GiveOrders()
	{
		if(workers.Count > 0 && workOrders.Count > 0 && minedBlocks.Count > 0)
		{
			//int r = Random.Range(0, workers.Count);
			//print (r);
			workers[0].GetComponent<UnitManager>().GetWorkOrder((int)workOrders[0].x, (int)workOrders[0].y, (int)minedBlocks[0].x, (int)minedBlocks[0].y);


			workers.RemoveAt(0);
			workOrders.RemoveAt(0);
			minedBlocks.RemoveAt(0);
		}
	}

	IEnumerator CheckWorkorder(Vector3 order)
	{
		checkingOrder = true;

		//print (CheckTheList(order));
		if(CheckTheList(order) == false)
		{
			GameObject obj = GetPlayAreaObject(order.x,order.y);

			//if(!obj.name.Contains("Floor") || "Imp")
			if(obj.name.Contains("Soil") || obj.name.Contains("soil"))
			{
				//print(obj.name);
				for(int i = (int)order.x - 1; i<(int)order.x+2; i++)
				{
					for(int j = (int)order.y - 1; j<(int)order.y+2; j++)
					{

						if(i < 0 || i > playArea.GetLength(0)-1 || j < 0 || j > playArea.GetLength(1)-1)
						{
							//out of bounds
							continue;
						}
						else
						{
							if(playArea[i,j] == null)
							{
								continue;
							}
							else if(playArea[i,j].CompareTag("occupied"))
							{
								continue;
							}


							else if(playArea[i,j].transform.position == order)
							{
								continue;
							}

							//diagonalss 
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

							else if(playArea[i,j].CompareTag("unoccupied"))
							{
								//print ("succesful return");
								validWorkOrder = true;
								validDelayedWorkOrder = true;
								workFromPoint = playArea[i,j].transform.position;
								delayedWorkFromPoint = playArea[i,j].transform.position;

								//workOrders.Add(workFromPoint);
								break;
							}
						}

						yield return new WaitForFixedUpdate();
						
					}
				}
			}

			if(validWorkOrder != true)
			{
				//print ("it probably broke");
				validWorkOrder = false;
				validDelayedWorkOrder = false;
				workFromPoint = Vector3.zero;
				delayedWorkFromPoint = Vector3.zero;
			}

			checkingOrder = false;
		}

		else
		{

			validWorkOrder = false;
			validDelayedWorkOrder = false;
			workFromPoint = Vector3.zero;
			delayedWorkFromPoint = Vector3.zero;
			checkingOrder = false;
			RemoveFromList(order);
		}
	}

	void CheckDelayedOrders()
	{
		checkingDelays = true;

		CheckWorkorder(delayedWorkOrders[0]);
		if(validDelayedWorkOrder)
		{
			workOrders.Add(delayedWorkOrders[0]);
			delayedWorkOrders.RemoveAt(0);
		}

		checkingDelays = false;
	}

	public void RevealAdjacentTiles(int x, int y)
	{
		int s;
		for(int i = x - 1; i<x+2; i++)
		{
			for(int j = y - 1; j<y+2; j++)
			{
				
				if(i < 0 || i > playArea.GetLength(0)-1 || j < 0 || j > playArea.GetLength(1)-1)
				{
					continue;
				}
				else
				{
					if(playArea[i,j] == null)
					{
						continue;
					}
//					else if(playArea[i,j].CompareTag("occupied"))
//					{
//						continue;
//					}
//					else if(i == x && j == y)
//					{
//						continue;
//					}
					

					
					else if(playArea[i,j].name == "soil")
					{
						playArea[i,j] = Instantiate(freshSoil,new Vector3((float)i,(float)j,0), freshSoil.transform.rotation) as GameObject;
						s = Random.Range(0, soils.Length);
						playArea[i,j].GetComponent<SpriteRenderer>().sprite = soils[s];
						//Instantiate(crate,new Vector3((float)i,(float)j,0), crate.transform.rotation);
						
					}
				}
				
			}
		}
	}

	public void UpdateGold(bool add, int gold)
	{
		if(add)
		{
			totalGold += Random.Range(10, 30);
			goldCounter.text = totalGold.ToString();

		}
		else
		{
			totalGold -= Random.Range(10, 30);
			goldCounter.text = totalGold.ToString();
		}
	}

	private void RemoveFromList(Vector3 tap)
	{
		foreach(Vector3 point in minedBlocks)
		{
			if(point == tap)
			{
				int i = minedBlocks.IndexOf(point);
				//print (i);
				workOrders.RemoveAt(i);
				minedBlocks.RemoveAt(i);

				ray = Camera.main.ScreenToWorldPoint (Input.mousePosition);
									
				hit = Physics2D.Raycast(tap,-Vector2.up/2,0.1f);
				if(hit.collider)
				{
				
										
					if(hit.collider.name.Contains("h_l")) 
					{
						Destroy (hit.collider.gameObject);
						//RemoveFromList(new Vector3(Mathf.Round(ray.x), Mathf.Round(ray.y), 0));
					}
				}

				//print (i);
				break;
			}
		}
	}

	private bool CheckTheList(Vector3 tap)
	{
		foreach(Vector3 point in minedBlocks)
		{
			//print (tap);
			//print (point);
			if(point == tap)
			{
				return true;
			}
		}
		return false;
	}

	private void SpawnUnit(string unitToSpawn)
	{
		switch(unitToSpawn)
		{
		case("imp"):
		{
			Instantiate(imp,theAtlar.transform.position,imp.transform.rotation);
			ButtonHighlighter(null);
			StopCoroutine("FlashAltar");
			StartCoroutine("FlashAltar");
		}
			break;
		}
	}

	private IEnumerator FlashAltar()
	{
		theAtlar.GetComponent<SpriteRenderer>().sprite = altarFired;
		yield return new WaitForSeconds(1);
		theAtlar.GetComponent<SpriteRenderer>().sprite = altarNormal;
	}

	public void SpawnFCT(Vector3 pos, int dmg)
	{
		FCTobj = Instantiate(FCT, pos,FCT.transform.rotation) as GameObject;
		FCTobj.GetComponent<TextMesh>().text = dmg.ToString();
		FCT.GetComponent<Renderer>().sortingLayerName = "text";
	}

	IEnumerator SpawnStuff()
	{
		yield return new WaitForSeconds(5);
		Instantiate(goblin, new Vector3(22,23,0), goblin.transform.rotation);
		yield return new WaitForSeconds(3);
		Instantiate(knight, new Vector3(22,21,0), knight.transform.rotation);
	}

	public void ShowUnitStats()
	{
		if(currentTargetUnit)
		{
			toolTipHp.text = "Health: " +statScript.hp.ToString();
			toolTipHunger.text = "Hunger: " +statScript.hunger.ToString();
			toolTipFatigue.text = "Fatigue: " +statScript.fatigue.ToString();
			//InvokeRepeating("DisplayTargetStats",0.01f,0.2f);
		}
	}

//	public void DisplayTargetStats()
//	{
//		currentTargetUnit.SendMessage("ReportStats",SendMessageOptions.DontRequireReceiver);
//		toolTipHp.text = "Health: " +currentTargetHp.ToString();
//		toolTipHunger.text = "Hunger: " +currentTargetHunger.ToString();
//		toolTipFatigue.text = "Fatigue: " +currentTargetFatigue.ToString();
//	}
}
