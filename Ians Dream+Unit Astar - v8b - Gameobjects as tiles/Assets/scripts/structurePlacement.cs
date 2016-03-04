using UnityEngine;
using System.Collections;

public class structurePlacement : MonoBehaviour {


	public GameObject placeHolder;
	public GameObject buildingBlock;
	public GameObject hut;
	public GameObject storage;
	public GameObject factory;

	//------------------------------
	public GameObject mainBase;
	public GameObject bed;
	public GameObject chickenCoop;
	//----
	public GameObject currentBuilding;
	private levelManager2 levelScript;




	void Start () 
	{
		levelScript = GameObject.Find("Level Manager").GetComponent<levelManager2>();
	}
	
	// Update is called once per frame
//	void Update () 
//	{
//	
//	}

	public bool AddBuilding(string buildName, int xPos, int yPos)
	{
		//print (buildName);
		int[,] building;
		bool valid = true;
		 //=================================
		if(buildName == "hut")
		{
			print ("hut");
			currentBuilding = hut;
			building = new int[3,3]
			{ 
				{1,1,1},
				{1,1,1},
				{1,1,1} 
			};

			int rowSize = building.GetLength(0);
			int colSize = building.GetLength(1);

			rowSize = 3;
			colSize = 3;

			for(int row = 0; row < rowSize; row++)
			{
				for(int col = 0; col < colSize; col++)
				{
					if(building[row,col] == 1)
					{
						int newX = xPos+col;
						int newY = yPos+row;

						//check if playarea array position is empty
						//if(levelScript.playArea[newX,newY] != null) 
						if(levelScript.playArea[newX,newY].tag != "unoccupied")
						{
							//break out if not empty
							print (levelScript.playArea[newX,newY].tag);
							valid = false;
							break;
						}
					}
				}
				if(!valid)
				{
					//print ("!");
					return false;
				}
			}

			if(valid)
			{
				for(int row = 0; row < rowSize; row++)
				{
					for(int col = 0; col < colSize; col++)
					{
						if(building[row,col] == 1)
						{
							int newX = xPos+col;
							int newY = yPos+row;
							//placeholder is just for filling the array
							levelScript.playArea[newX,newY] = Instantiate(placeHolder,new Vector3(newX,newY,0),transform.rotation) as GameObject;
						}
					}
				}

				levelScript.buildings.Add(Instantiate(hut, new Vector3(xPos, yPos,0.5f), hut.transform.rotation) as GameObject);
			}
		}
		//===========================================================

		if(buildName == "factory")
		{
			print ("fac");
			currentBuilding = factory;
			building = new int[7,10]
			{ 
				{0,0,0,1,1,1,1,1,0,0},
				{0,0,0,1,1,1,1,1,0,0},
				{1,1,1,1,1,1,1,1,1,1},
				{1,1,1,1,1,1,1,1,1,1},
				{1,1,1,1,1,1,1,1,0,0},
				{0,0,0,0,0,0,1,1,0,0},
				{0,0,0,0,0,0,1,1,0,0},
			};
			
			int rowSize = building.GetLength(0);
			int colSize = building.GetLength(1);
			
			for(int row = 0; row < rowSize; row++)
			{
				for(int col = 0; col < colSize; col++)
				{
					if(building[row,col] == 1)
					{
						int newX = xPos+col;
						int newY = yPos+row;
						
						//check if playarea array position is empty
						if(levelScript.playArea[newX,newY] != null) 
						{
							//break out if not empty
							valid = false;
							break;
						}
					}
				}
				if(!valid)
				{
					return false;
				}
			}
			
			if(valid)
			{
				for(int row = 0; row < rowSize; row++)
				{
					for(int col = 0; col < colSize; col++)
					{
						if(building[row,col] == 1)
						{
							int newX = xPos+col;
							int newY = yPos+row;
							//placeholder is just or filling the a
							levelScript.playArea[newX,newY] = Instantiate(placeHolder,new Vector3(newX,newY,0),transform.rotation) as GameObject;
						}
					}
				}
				
				levelScript.buildings.Add(Instantiate(factory, new Vector3(xPos, yPos,0.5f), factory.transform.rotation) as GameObject);
			}
		}


		//=====================ACTUAL BUILDINGS

		if(buildName == "mainBase")
		{
			//print ("mainb");
			currentBuilding = mainBase;
			building = new int[5,5]
			{ 
				{1,1,1,1,1},
				{1,1,1,1,1},
				{1,2,3,4,1},
				{1,1,1,1,1},
				{1,1,1,1,1}
			};
			
			int rowSize = building.GetLength(0);
			int colSize = building.GetLength(1);
			
			rowSize = 5;
			colSize = 5;
			
			for(int row = 0; row < rowSize; row++)
			{
				for(int col = 0; col < colSize; col++)
				{
					if(building[row,col] == 1)
					{
						int newX = xPos+col;
						int newY = yPos+row;
						
						//check if playarea array position is empty
						//print (newX + "  " + newY);
						if(levelScript.playArea[newX,newY] != null) 
						{
							//break out if not empty
							valid = false;
							break;
						}
					}
				}
				if(!valid)
				{
					return false;
				}
			}
			
			if(valid)
			{
				for(int row = 0; row < rowSize; row++)
				{
					for(int col = 0; col < colSize; col++)
					{
						if(building[row,col] == 1)
						{
							int newX = xPos+col;
							int newY = yPos+row;
							//placeholder is just for filling the array
							//levelScript.playArea[newX,newY] = Instantiate(buildingBlock,new Vector3(newX,newY,0),transform.rotation) as GameObject;
							levelScript.playArea[newX,newY] = Instantiate(levelScript.claimedFloor,new Vector3(newX,newY,0),transform.rotation) as GameObject;

						}

						if(building[row,col] == 2)
						{
							int newX = xPos+col;
							int newY = yPos+row;

							levelScript.playArea[newX,newY] = Instantiate(levelScript.claimedFloor,new Vector3(newX,newY,0),transform.rotation) as GameObject;
							levelScript.playArea[newX,newY] = Instantiate(levelScript.altarL,new Vector3(newX,newY,0),transform.rotation) as GameObject;

						}

						if(building[row,col] == 3)
						{
							int newX = xPos+col;
							int newY = yPos+row;

							levelScript.playArea[newX,newY] = Instantiate(levelScript.claimedFloor,new Vector3(newX,newY,0),transform.rotation) as GameObject;

							levelScript.theAtlar = Instantiate(levelScript.altarM,new Vector3(newX,newY,0),transform.rotation) as GameObject;
							levelScript.playArea[newX,newY] = levelScript.theAtlar;
							//levelScript.theAtlar.transform.position = new Vector3(newX,newY,0);

						}

						if(building[row,col] == 4)
						{
							int newX = xPos+col;
							int newY = yPos+row;

							levelScript.playArea[newX,newY] = Instantiate(levelScript.claimedFloor,new Vector3(newX,newY,0),transform.rotation) as GameObject;
							levelScript.playArea[newX,newY] = Instantiate(levelScript.altarR,new Vector3(newX,newY,0),transform.rotation) as GameObject;
							
						}
					}
				}
				
				//levelScript.buildings.Add(Instantiate(mainBase, new Vector3(xPos, yPos,0.5f), mainBase.transform.rotation) as GameObject);
			}
		}
		//============================================
		if(buildName == "BuildingBed")
		{
			//print ("bBed");
			currentBuilding = bed;
			building = new int[1,1]
			{ 
				{1}
			};
			
			int rowSize = building.GetLength(0);
			int colSize = building.GetLength(1);
			
			rowSize = 1;
			colSize = 1;
			
			for(int row = 0; row < rowSize; row++)
			{
				for(int col = 0; col < colSize; col++)
				{
					if(building[row,col] == 1)
					{
						int newX = xPos+col;
						int newY = yPos+row;
						
						//check if playarea array position is empty
						//if(levelScript.playArea[newX,newY] != null) 
						if(levelScript.playArea[newX,newY].tag != "unoccupied")
						{
							//break out if not empty
							print (levelScript.playArea[newX,newY].tag);
							valid = false;
							break;
						}
					}
				}
				if(!valid)
				{
					//print ("!");
					return false;
				}
			}
			
			if(valid)
			{
				for(int row = 0; row < rowSize; row++)
				{
					for(int col = 0; col < colSize; col++)
					{
						if(building[row,col] == 1)
						{
							int newX = xPos+col;
							int newY = yPos+row;

							//levelScript.playArea[newX,newY] = Instantiate(levelScript.bed,new Vector3(newX,newY,0),transform.rotation) as GameObject;
							levelScript.playArea[newX,newY] = levelScript.bed;
							levelScript.bedLocations.Add(new Vector2( newX, newY));
						}
					}
				}
				
				//levelScript.buildings.Add(Instantiate(levelScript.bed, new Vector3(xPos, yPos,0.5f), levelScript.bed.transform.rotation) as GameObject);
			}
		}
		//===========================================================

		if(buildName == "BuildingChickenCoop")
		{
			//print ("Chikcoop");
			currentBuilding = chickenCoop;
			building = new int[1,1]
			{ 
				{1}
			};
			
			int rowSize = building.GetLength(0);
			int colSize = building.GetLength(1);
			
			rowSize = 1;
			colSize = 1;
			
			for(int row = 0; row < rowSize; row++)
			{
				for(int col = 0; col < colSize; col++)
				{
					if(building[row,col] == 1)
					{
						int newX = xPos+col;
						int newY = yPos+row;
						
						//check if playarea array position is empty
						//if(levelScript.playArea[newX,newY] != null) 
						if(levelScript.playArea[newX,newY].tag != "unoccupied")
						{
							//break out if not empty
							print (levelScript.playArea[newX,newY].tag);
							valid = false;
							break;
						}
					}
				}
				if(!valid)
				{
					//print ("!");
					return false;
				}
			}
			
			if(valid)
			{
				for(int row = 0; row < rowSize; row++)
				{
					for(int col = 0; col < colSize; col++)
					{
						if(building[row,col] == 1)
						{
							int newX = xPos+col;
							int newY = yPos+row;
							
							//levelScript.playArea[newX,newY] = Instantiate(levelScript.chickenCoop,new Vector3(newX,newY,0),transform.rotation) as GameObject;
							levelScript.playArea[newX,newY] = levelScript.chickenCoop;

						}
					}
				}
				
				//levelScript.buildings.Add(Instantiate(levelScript.bed, new Vector3(xPos, yPos,0.5f), levelScript.bed.transform.rotation) as GameObject);
			}
		}
		//===========================================================
		return valid;
	}

//	public bool CheckValidPosition(string buildName, int xPos, int yPos)
//	{
//		int[,] building;
//		bool valid = true;
//		
//		if(buildName == "hut")
//		{
//			currentBuilding = hut;
//			building = new int[3,3]
//			{ 
//				{1,1,1},
//				{1,1,1},
//				{1,1,1} 
//			};
//			
//			int rowSize = building.GetLength(0);
//			int colSize = building.GetLength(1);
//
//			rowSize = 3;
//			colSize = 3;
//			
//			for(int row = 0; row < rowSize; row++)
//			{
//				for(int col = 0; col < colSize; col++)
//				{
//					if(building[row,col] == 1)
//					{
//						int newX = xPos+col;
//						int newY = yPos+row;
//						
//						//check if playarea array position is empty
//						if(levelScript.playArea[newX,newY] != null) 
//						{
//							//break out if not empty
//							valid = false;
//							break;
//						}
//					}
//				}
//				if(!valid)
//				{
//					return false;
//				}
//			}
//
//		}
//
//		if(buildName == "factory")
//		{
//			currentBuilding = factory;
//			building = new int[7,10]
//			{ 
//				{0,0,0,1,1,1,1,1,0,0},
//				{0,0,0,1,1,1,1,1,0,0},
//				{1,1,1,1,1,1,1,1,1,1},
//				{1,1,1,1,1,1,1,1,1,1},
//				{1,1,1,1,1,1,1,1,0,0},
//				{0,0,0,0,0,0,1,1,0,0},
//				{0,0,0,0,0,0,1,1,0,0},
//			};
//			
//			int rowSize = building.GetLength(0);
//			int colSize = building.GetLength(1);
//			
//			for(int row = 0; row < rowSize; row++)
//			{
//				for(int col = 0; col < colSize; col++)
//				{
//					if(building[row,col] == 1)
//					{
//						int newX = xPos+col;
//						int newY = yPos+row;
//						
//						//check if playarea array position is empty
//						if(levelScript.playArea[newX,newY] != null) 
//						{
//							//break out if not empty
//							valid = false;
//							break;
//						}
//					}
//				}
//				if(!valid)
//				{
//					return false;
//				}
//			}
//		}
//			
//		return true;
//	}

}
