using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

//HOLDS OVERAL GAME INFO
public class Game_State_Scr : MonoBehaviour {

	//The game state. What scene we are/will be on
	public static string state;
	//Stores the info on how many player type
		//1 = 1 player
		//2 = 2 player, coop
		//3 = 2 player, vs
	public static int players;
	//Stores what character(s) 1 or both players are using
		//int's will eventually match specific game objects
	public static int character_P1;
	public static int character_P2;

	//Stores the "Look" of the chosen player
	public static int look_P1;
	public static int look_P2;

	//For the calculations of the scores after levels:
	//Score: number of souls collected
	public static int score;
	//levelTime: total time in level
	public static float levelTime;
	//reapTime: Points attributed based on how fast players can reap each soul
	public static float reapTime;
	//targetReap: How many Target souls were reaped
	public static int targetReap;
	//missedTargets: How many Target souls were missed (Souls 'withered' away)
	public static int missedTargets;
	//Level: Every 'x' seconds in-game, a new level will be reached.
	public static int Level;
	//topCombo: Thighest combo attained in the round
	public static int topCombo;
	//lastSec: Counting how many "Last Sec"s were attained in the round
	public static int lastSec;

	//levelIntro: True if doing the whole "3,2,1,Go" thing
	public static bool levelIntro;

	//isPaused: True if the game is paused
	public static bool isPaused;


	void Start ()
    {
		//Sets defaults:
			//First scene is the title, 1 player is set, characters are who cares
		state = "title_Menu";
		players = 1;
		character_P1 = 0;
		character_P2 = 0;
		score = 0;
		reapTime = 0.0f;
		missedTargets = 0;
		topCombo = 0;
		lastSec = 0;
		isPaused = false;
		Destroy (this.gameObject);
	}


	public static void goto_Scene()
    {
		//Load scene saved from 'state'
		SceneManager.LoadScene(state);
	}

	public static void reset_Defaults()
    {
		//Load scene saved from 'state'
		//<PUT CODE HERE>

	}
}
