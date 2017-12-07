using UnityEngine;
using System.Collections;

//GIVEN TO EACH 'BUTTON' IN THE MENUS
	//STORES WHAT MENU EACH BUTTON WILL GO TO, AND THEIR POSITIONS ON SCREEN
public class Menu_Obj_Scr : MonoBehaviour {

	//The object's relative potition wrt to the other 'buttons'
	public int x_Index;
	public int y_Index;
	//When we 'press' this 'button', this is where we go:
	public string goto_Val;

	//Marked 'true' if it's a popup, and not something that actually goes to a different scene
	public bool isPopup;
	//If the thingy is a popup, below marks the object(s) that popup
	public GameObject[] popups;

}
