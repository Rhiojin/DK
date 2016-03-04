using UnityEngine;
using System.Collections;

public class GameControl_DK : MonoBehaviour {
	// map AI
	MapAI MapAI;
	
	
	// GUI Vars
	public int OptionsID = 1; // to see what menu we are on;
	
	// ACTIONS
	public string Action; //to see what action we are doing
	GameObject SoilOBJ; // the soil object we are picking
	public bool OverSoil; // are we over some soil?
	public bool OverBuild; // are we over a building?
	GameObject BuildOBJ; // the building object, perhaps to destroy
	
	// build rooms
	public GameObject BedRoom; // prefab for bed
	public GameObject ChickRoom;// prefab for coop
	public GameObject WorkRoom;// prefab for workshop
	public GameObject LibRoom;// prefab for library
	public GameObject DoorWork;// prefab for door
	public GameObject SpikeWork;// prefab for spike trap
	public GameObject SpokeWork;// prefab for ghost skeleton fucking SCRAY TRAP
	GameObject TempRoom;// holding var
	
	//PREFABS FOR spells
	public GameObject ImpOBJ;
	public GameObject ShockOBJ;
	public GameObject GoldOBJ;
	
	// Action reskins
	GameObject ActionPannel;
	public Material NormalAction;// the normal action covered thing
	public Material PixAction; // for pick minning acctionn
	public Material DeleteAction; // for destroying shit
	public Material BedAction; // for placing beds
	public Material ChickAction;// for placing coop, resize to 4x4
	public Material WorkAction; // for placing workshop, resize to 4x4
	public Material LibAction; // for placing library, resize 4x4
	
	// GUUI------------------------------------------------ XD
	//Menu
	public GUIStyle BuildMenu; // the build menu
	public GUIStyle CastMenu; // cast menu
	public GUIStyle WorkMenu; // workshop menu
	public GUIStyle SelectionBox; // the wee selection box
	public GUIStyle BlankBox; // to hide spells you haven't researched
	public GUIStyle BlankForText;  // no BAD GUI BOXES
	public GUIStyle Menu; // the main menu
	
	
	// casting
	public Material SumImp; // SUMMON IMP
	public Material Shock; // SHOCK
	public Material SumCoin; // summon coins
	
	// workshop items
	public Material BDoor; // build door
	public Material BSpike; // build spike
	public Material BSpoke; // build spookeyy
	
	// for the soil to be skinned
	public Material FlashMine;
	public Material NormalSoil;
	
	
	// IN GAME VARS 
	
	//Money and MANANAAAA
	public int Money; // the gold stuff XD
	public float Mana; // mana which regens
	
	//Research 
	public float Research = 0; //once this gets to 100 then unlock new spell, 200 etc. 300 etc.
	public bool WorkshopBuilt = false;
	
	// Camera scipt VV
	Camera_Overlay_DK CM_Main;
	
	void Start(){
		CM_Main = GetComponent<Camera_Overlay_DK>();
		ActionPannel = GameObject.Find("PC/Action");
		MapAI = GameObject.Find("MapAI").GetComponent<MapAI>();
	}
	void Update(){
		if(Mana < 101){ //regen mana init
			Mana+=Time.deltaTime/2;
		}
	}
	// CONTROLS ------------------------------------------------//
	//IN GAME -----------------------
	public void MoveUp(){
		if(transform.position.z <= 50){
			transform.Translate(0,0,-1);
		}
	}
	public void MoveDown(){
		if(transform.position.z >= -50){
			transform.Translate(0,0,1);
		}
	}
	public void MoveLeft(){
		if(transform.position.x >= -50){
			transform.Translate(-1,0,0);
		}
	}
	public void MoveRight(){
		if(transform.position.x <= 50){
			transform.Translate(1,0,0);
		}	
	}
	//------------------------
	
	//IN MAIN MENU ----- -----------------
	public void MoveMMup(){
		if(OptionsID >1){
			OptionsID--;
		}
	}
	public void MoveMMdown(){
		if(OptionsID >0 && OptionsID<3){
			OptionsID++;
		}
	}
	public void SelectMM(){
		if(OptionsID == 1){
			//SAVE GAME? :D
		}
		if(OptionsID == 2){
			Application.LoadLevel(1);
			//clear data from save;
		}
		if(OptionsID == 3){
			Application.LoadLevel(0);
		}
		OptionsID = 1;
	}
	// ----------------------
	
	//IN OPTIONS MENU -------
	public void MoveOMup(){
		if(OptionsID >1){
			OptionsID--;
		}
	}
	public void MoveOMdown(){
		if(OptionsID >0 && OptionsID<3){
			OptionsID++;
		}
	}
	public void SelectOM(){
		CM_Main.ResetBool();
		if(OptionsID == 1){ // BUILD
			CM_Main.InBMenu = true;
		}
		if(OptionsID == 2){ // cAST
			CM_Main.InCMenu = true;
		}
		if(OptionsID == 3){ // make
			CM_Main.InWMenu = true;
		}
		OptionsID = 1;
	}
	//------------------------
	
	//IN BUILD x6 Menu ----------
	public void MoveBMup(){
		if(OptionsID == 4){
			OptionsID = 1;
		}
		if(OptionsID == 5){
			OptionsID = 2;
		}
		if(OptionsID == 6){
			OptionsID = 3;
		}
	}
	public void MoveBMdown(){
		if(OptionsID == 1){
			OptionsID = 4;
		}
		if(OptionsID == 2){
			OptionsID = 5;
		}
		if(OptionsID == 3){
			OptionsID = 6;
		}
	}
	public void MoveBMleft(){
		if(OptionsID>=1 && OptionsID<3){
			OptionsID++;
		}
		if(OptionsID>=4 && OptionsID<6){
			OptionsID++;
		}
	}
	public void MoveBMright(){
		if(OptionsID<=3 && OptionsID>1){
			OptionsID--;
		}
		if(OptionsID<=6 && OptionsID>4){
			OptionsID--;
		}
	}	
	public void SelectBM(){
		if(OptionsID == 1){
			Action = "Mine";
			ActionPannel.GetComponent<Renderer>().material = PixAction;
		}
		if(OptionsID == 2){
			Action = "Bed";
			ActionPannel.GetComponent<Renderer>().material = BedAction;
		}
		if(OptionsID == 3){
			Action = "Lib";
			gameObject.transform.localScale = new Vector3(2,1,2);
			gameObject.transform.Translate(new Vector3(1.5f,0,0.5f));
			ActionPannel.GetComponent<Renderer>().material = LibAction;
		}
		if(OptionsID == 4){
			Action = "Delete";
			ActionPannel.GetComponent<Renderer>().material = DeleteAction;
			//Destroy 	
		}
		if(OptionsID == 5){
			Action = "Chick";
			gameObject.transform.localScale = new Vector3(2,1,2);
			gameObject.transform.Translate(new Vector3(1.5f,0,0.5f));
			ActionPannel.GetComponent<Renderer>().material = ChickAction;
		}
		if(OptionsID == 6){
			Action = "Work";
			gameObject.transform.localScale = new Vector3(2,1,2);
			gameObject.transform.Translate(new Vector3(1.5f,0,0.5f));
			ActionPannel.GetComponent<Renderer>().material = WorkAction;
		}
		CM_Main.ResetBool();
		CM_Main.InGame = true;
	}
	//----------------------------
	
	//IN Cast x3 Menu ---------
	public void MoveCMleft(){
		if(OptionsID>=1 && OptionsID<3){
			OptionsID++;
		}	
	}
	public void MoveCMright(){
		if(OptionsID<=3 && OptionsID>1){
			OptionsID--;
		}
	}
	public void SelectCM(){
		// need to add in researchy bit ;)
		if(OptionsID == 1){ // IMP
			if(Research >=1){
				Action = "CastImp";
				ActionPannel.GetComponent<Renderer>().material = SumImp;
				CM_Main.ResetBool();
				CM_Main.InGame = true;
			}
		}
		if(OptionsID == 2){ // SHOCK
			if(Research >=2){
				Action = "CastShock";
				ActionPannel.GetComponent<Renderer>().material = Shock;
				CM_Main.ResetBool();
				CM_Main.InGame = true;
			}
		}
		if(OptionsID == 3){ // Gold
			if(Research >=3){
				Action = "CastGold";
				ActionPannel.GetComponent<Renderer>().material = SumCoin;
				CM_Main.ResetBool();
				CM_Main.InGame = true;
			}
		}
		OptionsID = 1;
	}
	//-------------------------
	
	//IN Workshop x3 Menu --------
	public void MoveWMleft(){
		if(OptionsID>=1 && OptionsID<3){
			OptionsID++;
		}	
	}
	public void MoveWMright(){
		if(OptionsID<=3 && OptionsID>1){
			OptionsID--;
		}
	}
	public void SelectWM(){
		if(WorkshopBuilt){
			CM_Main.ResetBool();
			// need to add in researchy bit ;)
			if(OptionsID == 1){ // Door
				CM_Main.InGame = true;
				Action = "BuildDoor";
				ActionPannel.GetComponent<Renderer>().material = BDoor;
			}
			if(OptionsID == 2){ // Trap
				CM_Main.InGame = true;
				Action = "BuildSpike";
				ActionPannel.GetComponent<Renderer>().material = BSpike;
			}
			if(OptionsID == 3){ // Scare
				CM_Main.InGame = true;
				Action = "BuildSpoke";
				ActionPannel.GetComponent<Renderer>().material = BSpoke;
			}
			OptionsID = 1;
		}
	}
	//-----------------------------
	
	
	// IN GAME OPTIONS ===============================================================================================**

	public void clearAction(){
		if(Action == "Lib"|| Action == "Chick" || Action == "Work"){
			ActionPannel.GetComponent<Renderer>().material = NormalAction;
			gameObject.transform.localScale = new Vector3(0.9f,1,0.9f);
			gameObject.transform.Translate(new Vector3(-1.5f,0,-0.5f));
			Action = null;
		}
		else Action = null; ActionPannel.GetComponent<Renderer>().material = NormalAction;
	}
	// MINING ! :D XXX!! :DAWYGTKWA fuck this is going to be fun.
	public void ActionMine(){	
		if(OverSoil && SoilOBJ.tag == "Soil"){
			SoilOBJ.tag = "Mined";
			SoilOBJ.GetComponent<Renderer>().material = FlashMine;
			MapAI.MineBlock(SoilOBJ);
		}
		else if(OverSoil && SoilOBJ.tag == "Mined"){
			SoilOBJ.tag = "Soil";
			SoilOBJ.GetComponent<Renderer>().material = NormalSoil;
			MapAI.RemoveMineBlock(SoilOBJ);
		}
	}
	
	// DELETETETETE
	public void ActionDelete(){
		if(!OverSoil && OverBuild && BuildOBJ.name !="Dungeon Heart"){
			Destroy(BuildOBJ);	
			clearAction();
		}
	}
	// Putting down Bed 
	public void ActionBed(){
		if(!OverSoil && !OverBuild){
			if(Money >= 100){
				TempRoom = Instantiate(BedRoom,transform.position,transform.rotation) as GameObject;
				Money-=100;
				clearAction();
			}
		}
	}
	
	
	// putting down chicken coop
	public void ActionChick(){
		if(!OverSoil && !OverBuild){
			if(Money >= 200){
				TempRoom = Instantiate(ChickRoom,transform.position,transform.rotation) as GameObject;
				Money -=200;
				clearAction();
			}
		}
	}	
	
	
	// putting down library 
	public void ActionLib(){
		if(!OverSoil && !OverBuild){
			if(Money >= 400){
				TempRoom = Instantiate(LibRoom,transform.position,transform.rotation) as GameObject;
				Money -= 400;
				clearAction();
			}
		}
	}	
	
	
	// putting down workshop
	public void ActionWork(){
		if(!OverSoil && !OverBuild){
			if(Money > 400){
				TempRoom = Instantiate(WorkRoom,transform.position,transform.rotation) as GameObject;
				Money -= 400;
				clearAction();
			}
		}
	}	
	
	
	// casting Imp
	public void ActionImp(){
		if(!OverSoil && !OverBuild){
			if(Mana >=10){
				TempRoom = Instantiate(ImpOBJ, transform.position,transform.rotation) as GameObject;
				Mana -= 10;
				clearAction();	
			}
		}
	}	
		
	// casting shock
	public void ActionShock(){
		if(!OverSoil && !OverBuild){
			if(Mana >= 15){
				TempRoom = Instantiate(ShockOBJ, transform.position,transform.rotation) as GameObject;
				Mana -=15;
				clearAction();
			}
		}
	}	
		
	
	// casting coindrop
	public void ActionGold(){
		if(!OverSoil && !OverBuild){
			if(Mana >=20){
				TempRoom = Instantiate(GoldOBJ, transform.position,transform.rotation) as GameObject;
				Mana -=20;
				clearAction();
			}
		}
	}	
		
	
	// build door
	public void ActionDoor(){
		if(!OverSoil && !OverBuild){
			// if money over certain amount
			// instansiate
			clearAction();
		}
	}		
	
	// build spike trap
	public void ActionSpike(){
		if(!OverSoil && !OverBuild){
			// if money over certain amount
			// instansiate
			clearAction();
		}
	}	
	
	// build spoke trap
	public void ActionSpoke(){
		if(!OverSoil && !OverBuild){
			// if money over certain amount
			// instansiate
			clearAction();
		}
	}	
	void OnTriggerStay(Collider col){
		if(col.gameObject.name == "Soil"){
			OverSoil = true;
			SoilOBJ = col.gameObject; 
		}
		else OverSoil = false;
		
		if(col.gameObject.tag == "PBuild"){
			OverBuild = true;
			BuildOBJ = col.gameObject;
		}
		else OverBuild = false;
	}
	
	
 //--------------------------------------------------------------------------//
	// GUI _---------------------------------------------------------------------
	void OnGUI(){
	// GUI FOR MAIN MENU
		if(CM_Main.InMMenu){
				GUI.Box(new Rect (200,85,100,130),"",Menu);
				GUI.Box(new Rect (235,100,100,40),"Save",BlankForText); // save
				GUI.Box(new Rect (235,140,100,40),"Reset",BlankForText); // resety
				GUI.Box(new Rect (235,180,100,40),"Quit",BlankForText); // quit
				if(OptionsID == 1){
					GUI.Box(new Rect (227,95,45,25),"",SelectionBox); // x marks the??
				}
				if(OptionsID == 2){
					GUI.Box(new Rect (227,135,45,25),"",SelectionBox); // x marks the??
				}
				if(OptionsID == 3){
					GUI.Box(new Rect (227,175,45,25),"",SelectionBox); // x marks the??
				}
		}
	// ------------------------------------------------------------
	// GUI for Options Menu
		if(CM_Main.InOMenu){
				GUI.Box(new Rect (200,85,100,130),"",Menu);
				GUI.Box(new Rect (235,100,100,40),"Build",BlankForText); // build
				GUI.Box(new Rect (235,140,100,40),"Cast",BlankForText); // cast
				GUI.Box(new Rect (222,180,100,40),"Workshop",BlankForText); // workshop
				if(OptionsID == 1){
					GUI.Box(new Rect (227,95,45,25),"",SelectionBox); // x marks the??
				}
				if(OptionsID == 2){
					GUI.Box(new Rect (227,135,45,25),"",SelectionBox); // x marks the??
				}
				if(OptionsID == 3){
					GUI.Box(new Rect (220,175,65,25),"",SelectionBox); // x marks the??
				}
		}
		
	// GUI for Build menu
		if(CM_Main.InBMenu){
				GUI.Box(new Rect (165,78,160,144),"",BuildMenu);
				GUI.Box(new Rect (240,90,60,30),Money+"gp",BlankForText);
			if(OptionsID == 1){
				GUI.Box(new Rect (183,110,40,40),"",SelectionBox); // x marks the??
			}
			if(OptionsID == 2){
				GUI.Box(new Rect (226,110,40,40),"",SelectionBox); // x marks the??
			}
			if(OptionsID == 3){
				GUI.Box(new Rect (269,110,40,40),"",SelectionBox); // x marks the??
			}
			if(OptionsID == 4){
				GUI.Box(new Rect (183,155,40,40),"",SelectionBox); // x marks the??
			}
			if(OptionsID == 5){
				GUI.Box(new Rect (224,155,40,40),"",SelectionBox); // x marks the??
			}
			if(OptionsID == 6){
				GUI.Box(new Rect (272,155,40,40),"",SelectionBox); // x marks the??
			}
		}
	// GUI for cast menu
		if(CM_Main.InCMenu){
				GUI.Box(new Rect (165,78,160,144),"",CastMenu);
				GUI.Box(new Rect (225,95,60,30),(Mathf.Round(Mana))+" Mana",BlankForText);
			if(Research < 1){
				GUI.Box(new Rect (183,122,40,40),"",BlankBox); // x marks the??
			}
			if(Research <2){
			 GUI.Box(new Rect (224,122,40,40),"",BlankBox); // x marks the?	
			}
			if(Research <3){
			GUI.Box(new Rect (263,122,40,40),"",BlankBox); // x marks the?	
			}
			
			
			if(OptionsID == 1){
					GUI.Box(new Rect (183,122,40,40),"",SelectionBox); // x marks the??
			}
			if(OptionsID == 2){
					GUI.Box(new Rect (224,122,40,40),"",SelectionBox); // x marks the?
			}
			if(OptionsID == 3){
					GUI.Box(new Rect (263,122,40,40),"",SelectionBox); // x marks the?
			}
		}	
		
	// GUI for workshop menu
		if(CM_Main.InWMenu){
				GUI.Box(new Rect (165,78,160,144),"",WorkMenu);
				GUI.Box(new Rect (240,90,60,30),Money+"gp",BlankForText);
			
			if(!WorkshopBuilt){
				GUI.Box(new Rect (183,122,40,40),"",BlankBox); // x marks the??
				GUI.Box(new Rect (224,122,40,40),"",BlankBox); // x marks the?	
				GUI.Box(new Rect (263,122,40,40),"",BlankBox); // x marks the?	
			}
			
			if(OptionsID == 1){
				GUI.Box(new Rect (183,125,40,40),"",SelectionBox); // x marks the??
			}
			if(OptionsID == 2){
				GUI.Box(new Rect (226,125,40,40),"",SelectionBox); // x marks the?
			}
			if(OptionsID == 3){
				GUI.Box(new Rect (267,125,40,40),"",SelectionBox); // x marks the?
			}
		}
	}
}


