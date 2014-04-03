using UnityEngine;
using System.Collections;

public class GlobalManager : MonoBehaviour {

	public LayerMask team0Layer;
	public LayerMask team1Layer;
	public GameObject[] purchaseables;
	public Unit[] purchaseUnits;

	public GameObject[] infantry;
	public GameObject[] vehicles;
	public GameObject[] turrets;
	public GameObject[] structures;

	public Texture2D[] infButtons;
	public Texture2D[] vehButtons;
	public Texture2D[] turButtons;
	public Texture2D[] strButtons;

	public GameObject[] activeButtons;
	public Texture2D[] activeSprites;
	
	public GameObject factory;
	public PlayerController[] playerControllers;
	public PlayerController localPlayer;
	public Light sun;

	public GUISkin skin;
	Texture2D[] buttonSprites;

	public int players;
	public string[] teamNames;
	public int localID;
	public int[] credits;
	public GameObject player;
	public string[] botNames;
	public string[] botTypes;
	public string botTypeOverride;
	public int startingCredits;

	public float buttonSize;
	public float buttonDistance;

	public int menuID;
	public int menuOffset;
	public MapManager map;
	public string tooltip;

	public Vector3[] nearbyVectors;
	public int particleAmount;
	public int maxParticles;

	public int maxPopulation;
	public int[] populations;

	// Use this for initialization
	void Start () {
		map = GetComponent<MapManager>();
		map.GenerateMap();
		teamNames = new string[2];
		credits = new int[players];
		populations = new int[players];
		playerControllers = new PlayerController[players];

		for (int i=0;i<players;i++) {
			GameObject newP = (GameObject)Instantiate(player,transform.position,Quaternion.identity);
			newP.transform.parent = transform;
			PlayerController pc = newP.GetComponent<PlayerController>();
			playerControllers[i] = pc;
			pc.id = i;
			credits[i] = startingCredits;
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

		purchaseUnits = new Unit[purchaseables.Length];
		ArrangeUnits();
	}

	void ArrangeUnits () {
		GameObject[] objects = new GameObject[purchaseables.Length];
		string[] namesInArray = new string[purchaseables.Length];
		for (int i=0;i<purchaseables.Length;i++) { //Testing every turret in array
			int smallestCost = int.MaxValue;
			GameObject smallestCostObject = purchaseables[i];
			for (int j=0;j<purchaseables.Length;j++) { //Comparing every turret to every other turret
				bool allGood = true;
				for (int a=0;a<purchaseables.Length;a++) { //Testing if the turret being tested against is in the list already
					if (namesInArray[a] == purchaseables[j].name) {
						allGood = false;
					}
				}
				if (allGood == true) {
					int uCost = purchaseables[j].GetComponent<Unit>().cost;
					if (uCost <= smallestCost) {
						smallestCost = uCost;
						smallestCostObject = purchaseables[j];
					}
				}
			}
			namesInArray[i] = smallestCostObject.name;
			objects[i] = smallestCostObject;
		}
		purchaseables = objects;

		for (int i=0;i<purchaseables.Length;i++) {
			purchaseUnits[i] = purchaseables[i].GetComponent<Unit>();
		}

		int iNum = 0;
		int vNum = 0;
		int tNum = 0;
		int sNum = 0;

		for (int i=0;i<purchaseables.Length;i++) {
			if (purchaseUnits[i].unitType == "infantry") {
				iNum++;
			}
			if (purchaseUnits[i].unitType == "vehicle") {
				vNum++;
			}
			if (purchaseUnits[i].unitType == "structure") {
				TurretController tur = purchaseUnits[i].GetComponent<TurretController>();
				if (tur) {
					tNum++;
				}else{
					sNum++;
				}
			}
		}

		infantry = new GameObject[iNum];
		infButtons = new Texture2D[iNum];
		vehicles = new GameObject[vNum];
		vehButtons = new Texture2D[vNum];
		turrets = new GameObject[tNum];
		turButtons = new Texture2D[tNum];
		structures = new GameObject[sNum];
		strButtons = new Texture2D[sNum];
		
		int iIndex = 0;
		int vIndex = 0;
		int tIndex = 0;
		int sIndex = 0;

		for (int i=0;i<purchaseables.Length;i++) {
			if (purchaseUnits[i].unitType == "infantry") {
				infantry[iIndex] = purchaseables[i];
				infButtons[iIndex] = infantry[iIndex].transform.FindChild ("Sprite").GetComponent<SpriteRenderer>().sprite.texture;
				iIndex++;
			}
			if (purchaseUnits[i].unitType == "vehicle") {
				vehicles[vIndex] = purchaseables[i];
				vehButtons[vIndex] = vehicles[vIndex].transform.FindChild ("Sprite").GetComponent<SpriteRenderer>().sprite.texture;
				vIndex++;
			}
			if (purchaseUnits[i].unitType == "structure") {
				TurretController tur = purchaseUnits[i].GetComponent<TurretController>();
				if (tur) {
					turrets[tIndex] = purchaseables[i];
					turButtons[tIndex] = turrets[tIndex].GetComponent<Unit>().newWeapon.transform.FindChild("Sprite").GetComponent<SpriteRenderer>().sprite.texture;
					tIndex++;
				}else{
					structures[sIndex] = purchaseables[i];
					strButtons[sIndex] = structures[sIndex].transform.FindChild ("Sprite").GetComponent<SpriteRenderer>().sprite.texture;
					sIndex++;
				}
			}
		}

		UpdateMenu (infantry,infButtons);
	}

	void UpdateMenu (GameObject[] newMenu, Texture2D[] newTextures) {
		activeButtons = newMenu;
		activeSprites = newTextures;
	}

	void Update () {
		sun.intensity = Mathf.Sin ((Time.time+240)/240)/5+0.3f;
		particleAmount = GameObject.FindGameObjectsWithTag("Particle").Length;
	}

	public bool IsInsideBattlefield (Vector2 pos) {
		bool inside = true;
		if ( pos.x < -map.mapWidth ) {
			inside = false;
		}
		if ( pos.x > map.mapWidth ) {
			inside = false;
		}
		if ( pos.y < -map.mapHeight ) {
			inside = false;
		}
		if ( pos.y > map.mapHeight ) {
			inside = false;
		}
		return inside;
	}

	void OnGUI () {
		GUI.skin = skin;
		if (localPlayer) {
			GUI.Box (new Rect(0,Screen.height-buttonSize-70,Screen.width,buttonSize+70),"");
			if ((buttonSize + buttonDistance) * purchaseables.Length + 20 > Screen.width) {
				menuOffset = (int)GUI.HorizontalSlider (new Rect(10,Screen.height-buttonSize-30,Screen.width-20,20),menuOffset,(Mathf.RoundToInt(-buttonSize - buttonDistance) * activeButtons.Length),0);
			}
			if (GUI.Button (new Rect(10,Screen.height - buttonSize - 55,buttonSize*2,buttonSize/2),"INFANTRY")) {
				UpdateMenu (infantry,infButtons);
			}
			if (GUI.Button (new Rect(10+buttonSize*2+buttonDistance,Screen.height - buttonSize - 55,buttonSize*2,buttonSize/2),"VEHICLES")) {
				UpdateMenu (vehicles,vehButtons);
			}
			if (GUI.Button (new Rect(10+(buttonSize*2+buttonDistance)*2,Screen.height - buttonSize - 55,buttonSize*2,buttonSize/2),"TURRETS")) {
				UpdateMenu (turrets,turButtons);
			}
			if (GUI.Button (new Rect(10+(buttonSize*2+buttonDistance)*3,Screen.height - buttonSize - 55,buttonSize*2,buttonSize/2),"STRUCTURES")) {
				UpdateMenu (structures,strButtons);
			}
			for (int i=0;i<activeButtons.Length;i++) {
				Unit newU = activeButtons[i].GetComponent<Unit>();
				Rect rect = new Rect ((float)menuOffset + 10 + i * (buttonSize + buttonDistance),Screen.height - buttonSize-10,buttonSize,buttonSize);
				if (newU.cost <= credits[localID]) {
					if (GUI.Button (rect,new GUIContent(activeSprites[i],newU.unitName + ", COST: " + newU.cost))) {
						localPlayer.selectedPurchaseOption = activeButtons[i];
					}
				}
			}
			tooltip = GUI.tooltip;
			GUI.Label (new Rect(Input.mousePosition.x,Screen.height - (buttonSize+30),Screen.width,20),GUI.tooltip);
		}
		for (int i=0;i<players;i++) {
			if (i==0) {
				GUI.Label (new Rect(10,i*20,Screen.width,20),"Team 1");
			}
			if (i==players/2) {
				GUI.Label (new Rect(10,(i+1)*20,Screen.width,20),"Team 2");
			}
			if (i<players/2) {
				GUI.Label (new Rect(10,(i+1)*20,Screen.width,20),playerControllers[i].playerName + ", CREDITS: " + credits[i].ToString ());
			}else{
				GUI.Label (new Rect(10,(i+2)*20,Screen.width,20),playerControllers[i].playerName + ", CREDITS: " + credits[i].ToString ());
			}
		}
	}
}