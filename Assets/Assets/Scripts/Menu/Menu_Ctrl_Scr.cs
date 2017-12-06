using UnityEngine;
using System.Collections;

//CONTROLS THE TRAVERSAL OF MENUS
	//OBJECT IS GIVEN DIFF ATTRIBS IN EACH SCENE FROM INSPECTOR
public class Menu_Ctrl_Scr : MonoBehaviour {

	//The menu above this one (when players hit 'back' button
		//Set in Inspector
	public string previous;
	//Holding the menu 'buttons'
	public GameObject[] menu_Obj;
	//Holds the gamestate object
	public GameObject GameState_Obj;
	//Holds the max values for our Grid (x,y)
		//Ex. if there are 6 vertical 'buttons', y= 6, x = 0
		//Used when creating an array of 'buttons'
	public int max_X;
	public int max_Y;
	//Holds the gameState's script (what we access the states from)
		//private Game_State_Scr gState_Script;
	//Holds the x/y indicies of the 'button' that is currenlt selected
	public int curr_x;
	public int curr_y;

	//What stores the 'buttons' and their placements
	private GameObject[,] the_Grid;

	//The thing that's gonna delay the moving of the 'cursor'
	private float timer;
	private float timerMain;


	public bool isStopped;
	public bool islessthanTwo;
	public float waitTime;

	void Start () {
		//Simple var initializations:
			//gState_Script = GameState_Obj.GetComponent<Game_State_Scr> ();
		//Every menu screen starts at the top left corner
		curr_x = 0;
		curr_y = 0;

		timer = 0.0f;
		isStopped = true;
		islessthanTwo = true;

		//Creating the 'Grid' using all of the inputed buttons and max values:
		the_Grid = new GameObject[max_X , max_Y];
		for (int i = 0; i < menu_Obj.Length; ++i) {
			Menu_Obj_Scr button_Scr = menu_Obj [i].GetComponent<Menu_Obj_Scr> ();
			the_Grid [button_Scr.x_Index, button_Scr.y_Index] = menu_Obj [i];
		}
		print ("ready");
	}

	void Update () {
		//The timer holds out until a certian time, when it's okay to move the cursor
		//if (timer > 0.1f) {
		//	//Gets...the inputs
		//	getInputs ();
		//	timer = 0.0f;
		//} else {
		//	timer += Time.deltaTime;
		//}

		//My attempt at doing the whole "move down once, wait a sec, move down as much as you want" thing:
		//If the cursor is at rest, we can move it
		if (isStopped) {
			timer = 0.0f;
			islessthanTwo = true;
			getInputs ();
		} else {
			//We can also move it if we already waited for the first 'wait' to pass
			if (!islessthanTwo || ((Input.GetAxis ("Horizontal") > -0.8f && Input.GetAxis ("Horizontal") < 0.8 && Input.GetAxis ("Vertical") > -0.8f && Input.GetAxis ("Vertical") < 0.8f) &&
				(Input.GetAxis ("Horizontal_Dpad") > -0.8f && Input.GetAxis ("Horizontal_Dpad") < 0.8 && Input.GetAxis ("Vertical_Dpad") > -0.8f && Input.GetAxis ("Vertical_Dpad") < 0.8f))) {
				//The following "if" is used as a delay so that we dont traverse things waaaay too fast
				if (timerMain > 0.05f) {
					timerMain = 0.0f;
					getInputs ();
				} else {
					timerMain += Time.deltaTime;
				}
			} else {	//This is where we wait a bit
				//If we've already waited, we can break out of the waiting
				if (timer > waitTime) {
					islessthanTwo = false;
				} else {	//The process of waiting
					timer += Time.deltaTime;
				}
			}
		}

	}


	IEnumerator SceneDelay(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		Game_State_Scr.goto_Scene ();
	}

	void getInputs(){
		//If we are on the title:
		if (Game_State_Scr.state == "title_Menu") {
			//If any Key is pressed on the title, goto main menu
			if (Input.anyKey) {
				Game_State_Scr.state = "main_Menu";
				//GetComponent<TitleScreenSoundScript>().EnterMenu();
				StartCoroutine("SceneDelay", 1.0f);
			}
			//If we are on any of the other menus:
		} else {
			//MOVEMENT ALONG MENUS:
			//wraps vertically/horizontally, and skips over any 'blank' spaces

			//Not moving at all
			if ((Input.GetAxis ("Horizontal") > -0.8f && Input.GetAxis ("Horizontal") < 0.8 && Input.GetAxis ("Vertical") > -0.8f && Input.GetAxis ("Vertical") < 0.8f) &&
				(Input.GetAxis ("Horizontal_Dpad") > -0.8f && Input.GetAxis ("Horizontal_Dpad") < 0.8 && Input.GetAxis ("Vertical_Dpad") > -0.8f && Input.GetAxis ("Vertical_Dpad") < 0.8f)) {
				isStopped = true;

				//Going 'up' a menu (when 'back' button is pushed)
				if (Input.GetAxis ("Cancel") > 0.0) {
					Game_State_Scr.state = previous;
					//GetComponent<MenuSoundScript>().ExitMenu();
					StartCoroutine("SceneDelay", 1.0f);
				}
				//If the 'next' button is pushed:
				if (Input.GetAxis ("Submit") > 0.0) {
					Menu_Obj_Scr selected_Obj = the_Grid [curr_x, curr_y].GetComponent<Menu_Obj_Scr> ();
					Game_State_Scr.state = selected_Obj.goto_Val;
					
					//GetComponent<MenuSoundScript>().EnterMenu();
					StartCoroutine("SceneDelay", 1.0f);
				}

				return;
			}

			//Moving Up
			if ((Input.GetAxisRaw ("Vertical") > 0.0f) || (Input.GetAxisRaw ("Vertical_Dpad") > 0.0f)) {
				the_Grid [curr_x, curr_y].SetActive (false);
				do {
					if (curr_y != 0)
						--curr_y;
					else
						curr_y = max_Y - 1;
				} while (the_Grid [curr_x, curr_y] == null);
				//GetComponent<MenuSoundScript>().NavigateMenu();
				the_Grid [curr_x, curr_y].SetActive (true);
				isStopped = false;
			}
			//Moving down
			if ((Input.GetAxis ("Vertical") < 0.0f) || (Input.GetAxis ("Vertical_Dpad") < 0.0f)) {
				the_Grid [curr_x, curr_y].SetActive (false);
				do {
					if (curr_y != max_Y - 1)
						++curr_y;
					else
						curr_y = 0;
				} while (the_Grid [curr_x, curr_y] == null);
				//GetComponent<MenuSoundScript>().NavigateMenu();
				the_Grid [curr_x, curr_y].SetActive (true);
				isStopped = false;
			}
			//Moving right
			if ((Input.GetAxis ("Horizontal") > 0.0f) || (Input.GetAxis ("Horizontal_Dpad") > 0.0f)) {
				the_Grid [curr_x, curr_y].SetActive (false);
				do {
					if (curr_x != max_X - 1)
						++curr_x;
					else
						curr_x = 0;
				} while (the_Grid [curr_x, curr_y] == null);
				//GetComponent<MenuSoundScript>().NavigateMenu();
				the_Grid [curr_x, curr_y].SetActive (true);
				isStopped = false;
			}
			//Moving left
			if ((Input.GetAxis ("Horizontal") < 0.0f) || (Input.GetAxis ("Horizontal_Dpad") < 0.0f)) {
				the_Grid [curr_x, curr_y].SetActive (false);
				do {
					if (curr_x != 0)
						--curr_x;
					else
						curr_x = max_X - 1;
				} while (the_Grid [curr_x, curr_y] == null);
				//GetComponent<MenuSoundScript>().NavigateMenu();
				the_Grid [curr_x, curr_y].SetActive (true);
				isStopped = false;
			}



			//Going 'up' a menu (when 'back' button is pushed)
			if (Input.GetAxis ("Cancel") > 0.0) {
				Game_State_Scr.state = previous;
				Game_State_Scr.goto_Scene ();
			}
			//If the 'next' button is pushed:
			if (Input.GetAxis ("Submit") > 0.0) {
				Menu_Obj_Scr selected_Obj = the_Grid [curr_x, curr_y].GetComponent<Menu_Obj_Scr> ();
				Game_State_Scr.state = selected_Obj.goto_Val;
				Game_State_Scr.goto_Scene ();
			}
		}
	}

}
