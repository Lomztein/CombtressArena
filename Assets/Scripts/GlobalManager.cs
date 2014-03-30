using UnityEngine;
using System.Collections;

public class GlobalManager : MonoBehaviour {

	public LayerMask team0Layer;
	public LayerMask team1Layer;
	public GameObject[] purchaseables;
	public GameObject[] offensive;
	public GameObject[] defensive;
	public GameObject factory;
	public PlayerController[] playerControllers;
	public PlayerController localPlayer;
	public Light sun;

	public GUIStyle style;
	Texture2D[] buttonSprites;

	public int players;
	public string[] teamNames;
	public int localID;
	public int[] credits;
	public GameObject player;
	public string[] botNames;

	public float buttonSize;
	public float buttonDistance;

	public int menuID;
	public int menuOffset;
	public MapManager map;

	public Vector3[] nearbyVectors;

	// Use this for initialization
	void Start () {
		map = GetComponent<MapManager>();
		map.GenerateMap();
		teamNames = new string[2];
		credits = new int[players];
		buttonSprites = new Texture2D[purchaseables.Length];
		playerControllers = new PlayerController[players];

		for (int i=0;i<players;i++) {
			GameObject newP = (GameObject)Instantiate(player,transform.position,Quaternion.identity);
			newP.transform.parent = transform;
			PlayerController pc = newP.GetComponent<PlayerController>();
			playerControllers[i] = pc;
			pc.id = i;
			credits[i] = 500;
			if (i<players/2) {
				pc.teamIndex = 0;
				pc.freindlyLayer = team0Layer;
			}else{
				pc.teamIndex = 1;
				pc.freindlyLayer = team1Layer;
			}
			if (i == localID) {
				pc.local = true;
				localPlayer = pc;
			}else{
				pc.botControlled = true;
			}
		}

		for (int i=0;i<purchaseables.Length;i++) {
			buttonSprites[i] = purchaseables[i].transform.FindChild ("Sprite").GetComponent<SpriteRenderer>().sprite.texture;
		}
	}

	void Update () {
		sun.intensity = Mathf.Sin ((Time.time+240)/240)/5+0.3f;
	}

	void OnGUI () {
		if (localPlayer) {
			if ((buttonSize + buttonDistance) * purchaseables.Length + 20 > Screen.width) {
				menuOffset = (int)GUI.HorizontalSlider (new Rect(10,Screen.height-buttonSize-30,Screen.width-20,20),menuOffset,(Mathf.RoundToInt(-buttonSize - buttonDistance) * purchaseables.Length),0);
			}
			if (menuID == 1) {
				for (int i=0;i<purchaseables.Length;i++) {
					Unit newU = purchaseables[i].GetComponent<Unit>();
					Rect rect = new Rect ((float)menuOffset + 10 + i * (buttonSize + buttonDistance),Screen.height - buttonSize-10,buttonSize,buttonSize);
					//GUI.Label (rect,buttonBackground);
					if (GUI.Button (rect,new GUIContent(buttonSprites[i],newU.unitName + ", COST: " + newU.cost))) {
						localPlayer.selectedPurchaseOption = purchaseables[i];
					}
				}
				GUI.Label (new Rect(Input.mousePosition.x,Screen.height - (buttonSize+30),Screen.width,20),GUI.tooltip);
			}
		}
		for (int i=0;i<players;i++) {
			if (i==0) {
				GUI.Label (new Rect(10,i*20,Screen.width,20),"Team 1");
			}
			if (i==players/2) {
				GUI.Label (new Rect(10,(i+1)*20,Screen.width,20),"Team 2");
			}
			if (i<players/2) {
				GUI.Label (new Rect(10,(i+1)*20,Screen.width,20),playerControllers[i].playerName + ", " + credits[i].ToString ());
			}else{
				GUI.Label (new Rect(10,(i+2)*20,Screen.width,20),playerControllers[i].playerName + ", " + credits[i].ToString ());
			}
		}
	}
}