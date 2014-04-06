using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public Vector3 mousePos;
	public GUISkin skin;
	public Transform background;
	public int subMenu;

	// Update is called once per frame
	void Update () {
		background.localScale = new Vector3 (camera.orthographicSize/5.4f,camera.orthographicSize/5.4f,1);
	}

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
		if (subMenu == 1) {
			if (GUI.Button (new Rect(Screen.width/3+40,20,Screen.width/3,30),"SKIRMISH")) {
				Application.LoadLevel("ca_menu_skirmish");
			}
			GUI.Box (new Rect(Screen.width/3+40,60,Screen.width/3,30),"MULTIPLAYER - NYI");
		}
		rect = new Rect(20,60,Screen.width/3,30);
		if (GUI.Button (rect,"QUIT")) {
			Application.Quit();
		}
		if (rect.Contains(mousePos)) {
			subMenu = 0;
		}
	}
}
