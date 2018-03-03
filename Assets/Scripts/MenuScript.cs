using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public Vector3 mousePos;
	public GUISkin skin;
	public Transform background;
	public int openMenu;
	public int subMenu;

	void OnGUI () {
		GUI.skin = skin;
		Vector3 mp = Input.mousePosition;
		mousePos = new Vector2 (mp.x,-mp.y+Screen.height);
		GUI.Box (new Rect(0,Screen.height-20,Screen.width,20),"COMBTRESS: ARENA - VERSION A0.09 - BUILD ON UNITY " + Application.unityVersion);
		Rect rect = new Rect(20,20,Screen.width/3,30);
		if (rect.Contains(mousePos)) {
			subMenu = 1;
		}
		GUI.Button(rect,"PLAY");
		rect = new Rect(20,60,Screen.width/3,30);
		GUI.Button (rect,"OPTIONS");
		if (rect.Contains(mousePos)) {
			subMenu = 2;
		}
		rect = new Rect(20,100,Screen.width/3,30);
		GUI.Button (rect,"STUFF");
		if (rect.Contains(mousePos)) {
			subMenu = 3;
		}
		rect = new Rect(20,140,Screen.width/3,30);
        if (GUI.Button (rect, "QUIT")) {
            Application.Quit ();
        }
		if (subMenu == 1) {
			if (GUI.Button (new Rect(Screen.width/3+40,20,Screen.width/3,30),"SKIRMISH")) {
				Application.LoadLevel("ca_menu_skirmish");
			}
			if (GUI.Button (new Rect(Screen.width/3+40,60,Screen.width/3,30),"MULTIPLAYER - WIP")) {
				Application.LoadLevel("ca_menu_skirmish");
			}
		}
	}
}
