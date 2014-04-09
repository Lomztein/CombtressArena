using UnityEngine;
using System.Collections;

public class GlobalManager : MonoBehaviour {

	public LayerMask team0Layer;
	public LayerMask team1Layer;
	public LayerMask selectorLayer;
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
	public Unit localFocusUnit;
	public Light sun;
	public Vector3 mousePos;

	public GUISkin skin;
	public GUISkin insufficientButtonStyle;
	Texture2D[] buttonSprites;
	public Transform selectedUnitSprite;

	public int players;
	public string[] teamNames;
	public int localID;
	public int[] credits;
	public GameObject player;
	public string botTypeOverride;
	public int startingCredits;

	public string[] names;
	public string localName;
	public string[] types;

	public float buttonSize;
	public float buttonDistance;
	public float infoScreenSize;

	public int menuID;
	public int menuOffset;
	public MapManager map;
	public string tooltip;
	public GameObject emptySprite;
	public GameObject selectedSprite;

	Unit locSelUnit;
	WeaponScript locSelWep;
	HealthScript locSelHel;
	BulletScript locSelBul;

	GameObject mouseFocUnit;

	public Vector3[] nearbyVectors;
	public int particleAmount;
	public int maxParticles;

	public int maxPopulation;
	public int[] populations;
	public int creditsMultiplier;

	public bool pauseMenuOpen;
	public bool gamePaused;
	public bool gameEnded;
	public string endReason;

	Vector3 sBoxOrigin;
	Vector3 sBoxEnd;
	public Rect selectionRect;

	public Vector3 sScreenOrigin;
	public Vector3 sSelOrigin;
	public Vector3 sSelEnd;
	public BoxCollider selCol;

	DataCarrierScript ds;

	// Use this for initialization
	void Start () {

		map = GetComponent<MapManager>();
		GameObject dc = GameObject.Find ("DataCarrier");
		if (dc) {
			ds = dc.GetComponent<DataCarrierScript>();
		}
		GetCarrierData();
		map.GenerateMap();
		credits = new int[players];
		populations = new int[players];
		playerControllers = new PlayerController[players];
		selCol = GetComponent<BoxCollider>();

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
				pc.playerName = localName;
			}else{
				pc.botControlled = true;
				BotInput bot = pc.gameObject.AddComponent("BotInput").GetComponent<BotInput>();
				bot.aiType = types[i];
				bot.input = pc;
				pc.playerName = names[i].ToUpper() + " - " + types[i].ToUpper() + " - " + " BOT";
			}
		}

		purchaseUnits = new Unit[purchaseables.Length];
		ArrangeUnits();
	}

	void GetCarrierData () {
		if (ds) {
			players = ds.players;
			localID = ds.localID;
			localName = ds.localName;
			teamNames = ds.teamNames;
			map.fortressAmount = ds.fortressNumber;
			map.mapWidth = ds.mapWidth;
			map.mapHeight = ds.mapHeight;
			
			names = ds.names;
			types = ds.types;
		}else{
			localName = "Commander";
			names = new string[players];
			types = new string[players];

			for (int i=0;i<players;i++) {
				names[i] = "BOT";
				types[i] = "balanced";
			}
		}
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
				infButtons[iIndex] = infantry[iIndex].GetComponent<InfantryController>().purchaseTexture;
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

	void PauseGame () {
		if (gamePaused) {
			gamePaused = false;
			pauseMenuOpen = false;
		}else{
			gamePaused = true;
			pauseMenuOpen = true;
		}
	}

	public void TestFortresses () {
		int t0 = 0;
		int t1 = 0;
		for (int i=0;i<map.fortresses.Length;i++) {
			if (map.fortresses[i]) {
				Unit locUnit = map.fortresses[i].GetComponent<Unit>();
				if (locUnit.teamIndex == 0) {
					t0++;
				}else{
					t1++;
				}
			}
		}
		Debug.Log ("Checking fortresses, " + t0 + ", " + t1);
		if (t0 == 1) {
			PauseGame ();
			gameEnded = true;
			endReason = teamNames[0] + " won!";
		}
		if (t1 == 1) {
			PauseGame ();
			gameEnded = true;
			endReason = teamNames[1] + " won!";
		}
	}

	void Update () {
		Vector3 mp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mousePos = new Vector3 (mp.x,mp.y,0);
		sun.intensity = Mathf.Sin ((Time.time+240)/240)/5+0.3f;
		particleAmount = GameObject.FindGameObjectsWithTag("Particle").Length;
		if (Input.GetButtonDown ("Escape")) {
			PauseGame ();
		}
		if (gamePaused || gameEnded) {
			Time.timeScale = 0;
		}else{
			Time.timeScale = 1;
		}
		if (localPlayer) {
			if (localPlayer.selectedPurchaseOption) {
				Unit locUnit = localPlayer.selectedPurchaseOption.GetComponent<Unit>();
				if (!selectedUnitSprite) {
					GameObject newObject = (GameObject)Instantiate(emptySprite,mousePos,Quaternion.identity);
					selectedUnitSprite = newObject.transform;
					SpriteRenderer locSprite = selectedUnitSprite.GetComponent<SpriteRenderer>();
					if (locUnit.unitType != "structure") {
						Sprite newSprite = factory.transform.FindChild ("Sprite").GetComponent<SpriteRenderer>().sprite;
						locSprite.sprite = newSprite;
						locSprite.transform.localScale = new Vector3 (0.5f,0.5f,1);
					}else{
						Sprite newSprite = null;
						if (locUnit.newWeapon) {
							Transform locWeapon = locUnit.newWeapon.transform;
							newSprite = locWeapon.FindChild("Sprite").GetComponent<SpriteRenderer>().sprite;
						}else{
							newSprite = localPlayer.selectedPurchaseOption.transform.FindChild ("Sprite").GetComponent<SpriteRenderer>().sprite;
						}
						locSprite.sprite = newSprite;
					}
				}else{
					SpriteRenderer locSprite = selectedUnitSprite.GetComponent<SpriteRenderer>();
					Color newColor = Color.green;
					selectedUnitSprite.position = new Vector3 (mousePos.x,mousePos.y,0);
					if (Physics.CheckSphere (new Vector3(mousePos.x,mousePos.y,0),1,localPlayer.freindlyLayer)) {
						newColor = Color.red;
					}
					if (IsInsideBattlefield (mousePos) == false) {
						newColor = Color.red;
					}
					if (localPlayer.population >= maxPopulation) {
						newColor = Color.red;
					}
					if (locUnit.cost >= credits[localID]) {
						newColor = Color.red;
					}
					if (Vector3.Distance (new Vector3(mousePos.x,mousePos.y,0),localPlayer.nearestFortress.position) > map.fRange) {
						newColor = Color.red;
					}
					locSprite.color = newColor;

				}
			}else{
				if (selectedUnitSprite) {
					Destroy(selectedUnitSprite.gameObject);
				}
			}
		}else{
			if (selectedUnitSprite) {
				Destroy(selectedUnitSprite.gameObject);
			}
		}
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

	void OnDrawGizmos () {
		Gizmos.DrawSphere (sSelOrigin,0.25f);
		Gizmos.DrawSphere (sSelEnd,0.25f);
	}

	void OnGUI () {
		GUI.skin = skin;
		if (!gamePaused) {
			if (Input.GetButtonDown("Fire1")) {
				sBoxOrigin = Input.mousePosition;
				sBoxOrigin = new Vector3 (sBoxOrigin.x,-sBoxOrigin.y+Screen.height,0);
				sScreenOrigin = Input.mousePosition-Vector3.back*10;
			}
			sSelOrigin = Camera.main.ScreenToWorldPoint(sScreenOrigin);
			if (Input.GetButton ("Fire1")) {
				Vector3 mp = Input.mousePosition;
				sBoxEnd = new Vector3 (mp.x,-mp.y+Screen.height,0)-sBoxOrigin;
				sSelEnd = Camera.main.ScreenToWorldPoint (Input.mousePosition)-Vector3.back*10;
				selectionRect = new Rect(sBoxOrigin.x,sBoxOrigin.y,sBoxEnd.x,sBoxEnd.y);
				GUI.Box (selectionRect,"",skin.customStyles[0]);
				selCol.center = (sSelOrigin + sSelEnd)/2;
				selCol.size = new Vector3 (sSelEnd.x-sSelOrigin.x,sSelEnd.y-sSelOrigin.y,999);
			}
			if (localPlayer) {
				GUI.Box (new Rect(0,Screen.height-buttonSize-65,(buttonSize+buttonDistance)*8-5,buttonSize+75),"");
				GUI.Box (new Rect(0,Screen.height-buttonSize-25,Screen.width,buttonSize+25),"");
				if ((buttonSize + buttonDistance) * activeButtons.Length + 20 > Screen.width) {
					menuOffset = (int)GUI.HorizontalSlider (new Rect(10,Screen.height-buttonSize-20,Screen.width-20,20),menuOffset,(Mathf.RoundToInt(-buttonSize - buttonDistance) * activeButtons.Length),0);
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
				bool anyContainsMouse = false;
				for (int i=0;i<activeButtons.Length;i++) {
					Unit newU = activeButtons[i].GetComponent<Unit>();
					Rect rect = new Rect ((float)menuOffset + 10 + i * (buttonSize + buttonDistance),Screen.height - buttonSize-10,buttonSize,buttonSize);
					if (newU.cost <= credits[localID]) {
						if (GUI.Button (rect,new GUIContent("",newU.unitName.ToUpper () + ", COST: " + newU.cost))) {
							localPlayer.selectedPurchaseOption = activeButtons[i];
							if (selectedUnitSprite) {
								Destroy(selectedUnitSprite.gameObject);
							}
						}
					}else{
						GUI.Box (rect,new GUIContent("",newU.unitName.ToUpper () + ", COST: " + newU.cost + ", INSUFFICIENT CREDITS"));
					}
					GUI.DrawTexture (rect,activeSprites[i],ScaleMode.ScaleToFit,true,1);
					if (rect.Contains(new Vector2 (Input.mousePosition.x,Screen.height-Input.mousePosition.y))) {
						mouseFocUnit = activeButtons[i];
						anyContainsMouse = true;
						if (locSelUnit == null) {
							locSelUnit = mouseFocUnit.GetComponent<Unit>();
						}else{
							if (locSelUnit.gameObject != mouseFocUnit) {
								locSelUnit = mouseFocUnit.GetComponent<Unit>();
							}
						}
					}
				}
				if (anyContainsMouse == false) {
					mouseFocUnit = null;
				}
				tooltip = GUI.tooltip;
				GUI.Label (new Rect(Input.mousePosition.x,Screen.height - (buttonSize+30),Screen.width,20),GUI.tooltip);
				if (localPlayer.selectedCount > 0 || mouseFocUnit) {
					Rect unitRect = new Rect(Screen.width-infoScreenSize,0,infoScreenSize,Screen.height-buttonSize-25);
					GUI.Box (unitRect,"");
					for (int a=0;a<localPlayer.selectedCount;a++) {
						GUI.Box (new Rect(Screen.width-infoScreenSize+10,10+(a*buttonSize),infoScreenSize-20,buttonSize),"");
						GUI.DrawTexture(new Rect(Screen.width-infoScreenSize+10,10+(a*buttonSize),buttonSize,buttonSize),localPlayer.selectedSprites[a]);
						GUI.Label (new Rect(Screen.width+buttonSize-infoScreenSize+10,12+(a*buttonSize),infoScreenSize-25-buttonSize,buttonSize-10),localPlayer.selectedUnits[a].unitName.ToUpper());
					}
					if ((localPlayer.selectedCount == 1 && localPlayer.selectedUnits[0]) || mouseFocUnit) {
						if (localPlayer.selectedCount > 0) { if (localPlayer.selectedUnits[0]) { locSelUnit = localPlayer.selectedUnits[0]; }}
						string weaponInfo;
						if (locSelHel == null) {
							locSelHel = locSelUnit.GetComponent<HealthScript>();
							if (locSelUnit.newWeapon) {
								locSelWep = locSelUnit.newWeapon.GetComponent<WeaponScript>();
								locSelBul = locSelWep.bulletType.GetComponent<BulletScript>();
							}
						}
						if (locSelWep) {
							weaponInfo = "ANTI-"+locSelBul.damageType.ToUpper ()+", "+locSelHel.armorType.ToUpper();
								if (locSelUnit.unitDisc.Length > 0) {
								weaponInfo = weaponInfo + "\n\n"+locSelUnit.unitDisc;
							}else{
								weaponInfo = weaponInfo + "\n\nTHIS UNIT LACKS A DESCRIPTION, GET ON IT YOU LAZY DEVS!";
							}
								weaponInfo = weaponInfo +"\n\nDAMAGE: "+locSelWep.damage*locSelUnit.bDamage+" * "+locSelWep.amount
								+"\nFIRERATE: "+(1/((Mathf.Max (1,locSelWep.transform.childCount))*(locSelWep.reloadTime*locSelUnit.bFirerate))).ToString ()+" / SEC"
									+"\nDPS: "+(locSelWep.damage*locSelWep.amount)/((Mathf.Max (1,locSelWep.transform.childCount))*(locSelWep.reloadTime*locSelUnit.bFirerate))
									+"\nRANGE: "+locSelWep.range*locSelUnit.bRange+"\n";
							if (mouseFocUnit == null) {
								weaponInfo = weaponInfo
									+"\nHULL: "+locSelHel.health+" / "+locSelHel.maxHealth
									+"\nEXP: "+locSelUnit.experience+" / "+locSelUnit.expNeeded
									+"\nLEVEL: "+locSelUnit.level+"\n";
							}

							if (locSelUnit.unitPros.Length > 0 || locSelBul.homing || locSelBul.piercing || locSelBul.applyEffect) {
								weaponInfo = weaponInfo+"\nPROS:";
								for (int a=0;a<locSelUnit.unitPros.Length;a++) {
									weaponInfo = weaponInfo + "\n"+locSelUnit.unitPros[a].ToUpper();
								}
								if (locSelBul.homing) {
									weaponInfo = weaponInfo + "\nHOMING BULLETS";
								}
								if (locSelBul.piercing) {
									weaponInfo = weaponInfo + "\nPIERCING BULLETS";
								}
								if (locSelBul.applyEffect) {
									weaponInfo = weaponInfo + "\n"+locSelBul.effect.name.ToUpper ();
								}
							}
							if (locSelUnit.unitCons.Length > 0) {
								weaponInfo = weaponInfo+"\n\nCONS";
								for (int a=0;a<locSelUnit.unitCons.Length;a++) {
									weaponInfo = weaponInfo + "\n"+locSelUnit.unitCons[a].ToUpper();
								}
							}
						}else{
							weaponInfo = "PASSIVE, "+locSelHel.armorType.ToUpper()+"\n\n"+locSelUnit.unitDisc.ToUpper();
						}
						GUI.Box (new Rect (Screen.width-infoScreenSize+10,buttonSize+20,infoScreenSize-20,Screen.height-buttonSize-105),"");
						GUI.Label (new Rect(Screen.width-infoScreenSize+15,buttonSize+22,infoScreenSize-25,Screen.height-buttonSize-100),weaponInfo);
					}
				}
				if (localPlayer.selectedCount != 1) {
					locSelWep = null;
					locSelHel = null;
					locSelBul = null;
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
					GUI.Label (new Rect(10,(i+1)*20,Screen.width,20),playerControllers[i].playerName + ", CREDITS: " + credits[i].ToString () + ", STRUCTURES: " + populations[i].ToString() + "/" + maxPopulation.ToString());
				}else{
					GUI.Label (new Rect(10,(i+2)*20,Screen.width,20),playerControllers[i].playerName + ", CREDITS: " + credits[i].ToString () + ", STRUCTURES: " + populations[i].ToString() + "/" + maxPopulation.ToString());
				}
			}
		}
		if (gameEnded) {
			if (endReason.Length == 0) {
				endReason = "Unknown reason";
			}
			float stringLength = ("GAME OVER: ".Length + endReason.Length) * 9 + 20;
			GUI.Box (new Rect(Screen.width/2-stringLength/2,Screen.height/9,stringLength,40),"GAME OVER: " + endReason.ToUpper ());
		}
		if (pauseMenuOpen) {
			int buttons = 5;
			int bd = 10;
			float sw = Screen.width/3;
			float sh = Screen.height/buttons;
			GUI.Box (new Rect(Screen.width/2 - sw/2,0,sw,Screen.height),"");
			if (GUI.Button (new Rect(Screen.width/2-sw/2+10,bd,sw-20,sh-bd-bd/buttons),"RESUME")) {
				PauseGame ();
			}
			GUI.Box (new Rect(Screen.width/2-sw/2+10,sh+bd-bd/buttons,sw-20,sh-bd-bd/buttons),"SAVE");
			GUI.Box (new Rect(Screen.width/2-sw/2+10,sh*2+bd-bd/buttons*2,sw-20,sh-bd-bd/buttons),"OPTIONS");
			if (GUI.Button (new Rect(Screen.width/2-sw/2+10,sh*3+bd-bd/buttons*3,sw-20,sh-bd-bd/buttons),"RESTART")) {
				Application.LoadLevel (Application.loadedLevel);
			}
			if (GUI.Button (new Rect(Screen.width/2-sw/2+10,sh*4-bd/buttons*4+bd,sw-20,sh-bd-bd/buttons),"QUIT")) {
				Application.LoadLevel ("ca_menu");
			}
		}
	}
}