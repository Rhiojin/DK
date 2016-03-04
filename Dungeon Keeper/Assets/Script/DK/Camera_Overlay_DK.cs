using UnityEngine;
using System.Collections;

public class Camera_Overlay_DK : MonoBehaviour {
	
	public Texture2D GBOverlay; 
	public GUIStyle Invsi_Buttons;
	
	// the other script;
	GameControl_DK Main;
	
	// the button click sound....mmm
	public AudioClip ButtonClick;
	
	// MENU BOOLS - what menu are we in anyway?
	public bool InGame = false; //in main game
	public bool InMMenu = false; // in main menu (exit/save/reset)
	public bool InOMenu = false; //In build options menu (build/cast/...build)
	public bool InBMenu = false; // in build menu (mine, destroy rooms, etc.)
	public bool InWMenu = false; // in workshop menu (door, trap, scare)
	public bool InCMenu = false; // in Cast menu (spawn imp, etc.)

	// Use this for initialization
	void Start () {
		Screen.SetResolution (480, 320, true);
		Main = GetComponent("GameControl_DK") as GameControl_DK;
		InGame = true;

	}
	
	// Update is called once per frame
	void Update () {
	}
	void OnGUI () {
		// MAIN OVERLAY---------------------------------------
		GUI.Label (new Rect (0,-30,500,360),GBOverlay);
		//-------------------------------------------------------
		// BUTTONS ----------------------------------------------
		// D PAD ----------------------------------------------------
		if(GUI.Button(new Rect (72,90,40,40),"",Invsi_Buttons)){ // up
			GetComponent<AudioSource>().PlayOneShot(ButtonClick);
			if(InGame){
				Main.MoveUp();
			}
			else if(InMMenu){
				Main.MoveMMup();	
			}
			else if(InOMenu){
				Main.MoveOMup();
			}
			else if(InBMenu){
				Main.MoveBMup();
			}
		}
		if(GUI.Button(new Rect (72,170,40,40),"",Invsi_Buttons)){ // down
			GetComponent<AudioSource>().PlayOneShot(ButtonClick);
			if(InGame){
				Main.MoveDown();
			}
			else if(InMMenu){
				Main.MoveMMdown();	
			}
			else if(InOMenu){
				Main.MoveOMdown();
			}
			else if(InBMenu){
				Main.MoveBMdown();
			}

		}
		if(GUI.Button(new Rect (35,130,40,40),"",Invsi_Buttons)){ // right
			GetComponent<AudioSource>().PlayOneShot(ButtonClick);
			if(InGame){
				Main.MoveRight();
			}
			else if(InBMenu){
				Main.MoveBMright();
			}
			else if(InCMenu){
				Main.MoveCMright();
			}
			else if(InWMenu){
				Main.MoveWMright();
			}

		}
		if(GUI.Button(new Rect (112,130,40,40),"",Invsi_Buttons)){ // left
			GetComponent<AudioSource>().PlayOneShot(ButtonClick);
			if(InGame){
				Main.MoveLeft();
			}
			else if(InBMenu){
				Main.MoveBMleft();
			}
			else if(InCMenu){
				Main.MoveCMleft();
			}
			else if(InWMenu){
				Main.MoveWMleft();
			}

		}
		//-----------------------------------------------------------
		// A Button -----------------------------------------------
		if(GUI.Button(new Rect (343,90,50,50),"",Invsi_Buttons)){ // a
			GetComponent<AudioSource>().PlayOneShot(ButtonClick);
			// IN MENU ACTIONS 
			if(InMMenu){
				Main.SelectMM();	
			}
			else if(InOMenu){
				Main.SelectOM();	
			}
			else if(InBMenu){
				Main.SelectBM();
			}
			else if(InCMenu){
				Main.SelectCM();
			}
			else if(InWMenu){
				Main.SelectWM();
			}
			
			// IN GAME ACTIONS
			else if(Main.Action == "Mine"){
				Main.ActionMine();	
			}
			else if(Main.Action == "Delete"){
				Main.ActionDelete();	
			}
			else if(Main.Action == "Bed"){
				Main.ActionBed();	
			}
			else if(Main.Action == "Chick"){
				Main.ActionChick();	
			}
			else if(Main.Action == "Lib"){
				Main.ActionLib();	
			}
			else if(Main.Action == "Work"){
				Main.ActionWork();	
			}
			else if(Main.Action == "BuildDoor"){
				Main.ActionDoor();	
			}
			else if(Main.Action == "BuildSpike"){
				Main.ActionSpike();	
			}
			else if(Main.Action == "BuildSpoke"){
				Main.ActionSpoke();	
			}
			else if(Main.Action == "CastImp"){
				Main.ActionImp();	
			}
			else if(Main.Action == "CastShock"){
				Main.ActionShock();	
			}
			else if(Main.Action == "CastGold"){
				Main.ActionGold();	
			}


		}
		//--------------------------------------------------------
		// B Button -----------------------------------------------
		if(GUI.Button(new Rect (393,160,50,50),"",Invsi_Buttons)){ // b
			GetComponent<AudioSource>().PlayOneShot(ButtonClick);
			// IN MENU ACTIONS
			if(InMMenu || InOMenu || InWMenu || InBMenu || InCMenu){
				ResetBool();
				InGame = true;
			}
			// IN GAME ACTIONS
			else if(Main.Action != null){
				Main.clearAction();
			}
		}
		//--------------------------------------------------------
		// Start Button -----------------------------------------------
		if(GUI.Button(new Rect (265,250,60,35),"",Invsi_Buttons)){ // start
			GetComponent<AudioSource>().PlayOneShot(ButtonClick);
			Main.OptionsID = 1;
			if(InMMenu){
				ResetBool();
				InGame = true;
			}
			else if(InGame){
				ResetBool();
				InMMenu = true;
			}
		}
		//--------------------------------------------------------
		// Select Button -----------------------------------------------
		if(GUI.Button(new Rect (170,250,60,35),"",Invsi_Buttons)){
			GetComponent<AudioSource>().PlayOneShot(ButtonClick);
			Main.OptionsID = 1;
			if(InGame){
				ResetBool();
				InOMenu = true;
			}
			else if(InOMenu){
				ResetBool();
				InGame = true;			
			}
			
		}// select
		//--------------------------------------------------------
	}
	public void ResetBool(){
	InGame = false;
	InMMenu = false;
	InOMenu = false;
	InBMenu = false;
	InWMenu = false;
	InCMenu = false;
	}
}
