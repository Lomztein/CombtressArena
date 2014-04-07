using UnityEngine;
using System.Collections;

public class SkirmishMenuScript : MonoBehaviour {

	public int players;
	public int localID;
	public string localName;
	public string[] teamNames;
	public int fortressNumber;
	public float mapWidth;
	public float mapHeight;

	public float slotLength;
	public float slotSize;
	public float slotDistance;
	public float slotsDistanceFromTop;
	public float slotsDistanceFromSide;
	public float slotsPosOffset;

	public GUISkin skin;

	public string[] names;
	public string[] types;
	public string[] randomBotNames;

	public string[] botNames;
	public string[] botTypes;
	public string[] botTypeDescriptions;
	public bool typeMenuOpen;
	public int selectedIndex;

	public DataCarrierScript ds;

	void Start () {
		GameObject dc = GameObject.Find ("DataCarrier");
		ds = dc.GetComponent<DataCarrierScript>();
	}

	void UpdateArrays () {
		string[] prevNames = names;
		string[] prevTypes = types;

		names = new string[players];
		types = new string[players];

		for (int i=0;i<players;i++) {
			if (i<prevNames.Length) {
				names[i] = prevNames[i];
				types[i] = prevTypes[i];
			}
			if (names[i] == null) {
				names[i] = botNames[Random.Range (0,botNames.Length)];
			}
			if (types[i] == null) {
				types[i] = "balanced";
			}
		}
	}

	void OnGUI () {
		GUI.skin = skin;
		float totSize = slotsDistanceFromTop + ((players+3)*(slotSize+slotDistance));
		if (players != names.Length) {
			UpdateArrays();
		}
		slotLength = Screen.width/2-slotsDistanceFromSide;
		if (players == names.Length) {
			int j=0;
			for (int i=0;i<players;i++) {
				string locName = "";
				if (i == localID) {
					locName = localName.ToUpper () + " - YOU";
				}else{
					locName = names[i].ToUpper () + " - " + types[i].ToUpper () + " - " + "BOT";
				}
				if (i == 0) {
					GUI.Box (new Rect(slotsDistanceFromSide,slotsDistanceFromTop+slotsPosOffset + ((i+j)*(slotSize+slotDistance)),slotLength,slotSize),"TEAM 1" + " - " + teamNames[0].ToUpper ());
					teamNames[0] = GUI.TextField (new Rect(slotLength+slotsDistanceFromSide,slotsDistanceFromTop+slotsPosOffset + ((i+j)*(slotSize+slotDistance)),slotLength/2,slotSize),teamNames[0].ToUpper ());
					j++;
				}
				if (i == players/2) {
					GUI.Box (new Rect(slotsDistanceFromSide,slotsDistanceFromTop+slotsPosOffset + ((i+j)*(slotSize+slotDistance)),slotLength,slotSize),"TEAM 2" + " - " + teamNames[1].ToUpper());
					teamNames[1] = GUI.TextField (new Rect(slotLength+slotsDistanceFromSide,slotsDistanceFromTop+slotsPosOffset + ((i+j)*(slotSize+slotDistance)),slotLength/2,slotSize),teamNames[1].ToUpper());
					j++;
				}
				GUI.Box(new Rect(slotsDistanceFromSide,slotsDistanceFromTop+slotsPosOffset + ((i+j)*(slotSize+slotDistance)),slotLength,slotSize),(i+1).ToString () + " -- " + locName,skin.customStyles[1]);
				if (typeMenuOpen == false) {
					if (localID != i) {
						if (GUI.Button (new Rect(slotLength+slotsDistanceFromSide,slotsDistanceFromTop+slotsPosOffset + ((i+j)*(slotSize+slotDistance)),slotLength/4,slotSize),new GUIContent("TAKE","SWAP SLOTS WITH THIS DUDE"))) {
							if (localID >= 0) {
								names[localID] = names[i];
								types[localID] = types[i];
							}
							localID = i;
						}
						if (GUI.Button (new Rect(slotLength+slotsDistanceFromSide+slotLength/4,slotsDistanceFromTop+slotsPosOffset + ((i+j)*(slotSize+slotDistance)),slotLength/4,slotSize),new GUIContent("TYPE","CHANGE THE TYPE OF BOT THIS DUDE IS"))) {
							typeMenuOpen = true;
							selectedIndex = i;
							types[i] = "Changing";
						}
					}else{
						if (GUI.Button (new Rect(slotLength+slotsDistanceFromSide,slotsDistanceFromTop+slotsPosOffset + ((i+j)*(slotSize+slotDistance)),slotLength/4,slotSize),new GUIContent("LEAVE","LEAVE THIS SLOT AND BECOME AN AWESOME GHOST SPECTATOR"))) {
							localID = -1;
						}
						localName = GUI.TextField (new Rect(slotLength+slotsDistanceFromSide+slotLength/4,slotsDistanceFromTop+slotsPosOffset + ((i+j)*(slotSize+slotDistance)),slotLength/4,slotSize),localName.ToUpper());
					}
				}
			}
			if (typeMenuOpen) {
				for (int i=0;i<botTypes.Length;i++) {
					if (GUI.Button (new Rect(slotLength+slotsDistanceFromSide,slotsDistanceFromTop + ((i)*(slotSize+slotDistance)),slotLength/2,slotSize),new GUIContent(botTypes[i].ToUpper(),botTypeDescriptions[i].ToUpper()))) {
						types[j+selectedIndex] = botTypes[i];
						typeMenuOpen = false;
						selectedIndex = -1;
					}
				}
			}
		}
		if (totSize >= Screen.height) {
			slotsPosOffset = GUI.VerticalSlider (new Rect(slotsDistanceFromSide+slotLength*1.5f+10,slotsDistanceFromTop,20,Screen.height-slotsDistanceFromTop-60),slotsPosOffset,-totSize,0);
		}else{
			slotsPosOffset = 0;
		}
		GUI.Box (new Rect(slotsDistanceFromSide+slotLength*1.5f+30,slotsDistanceFromTop,Screen.width/4-30,slotSize*2),"PLAYERS: " + players.ToString());
		if (GUI.Button (new Rect(slotsDistanceFromSide+slotLength*1.5f+30,slotsDistanceFromTop+slotSize*2,Screen.width/4-Screen.width/8-10,slotSize*2),new GUIContent("REMOVE ONE","REMOVES A PLAYER FROM THE GAME"))) {
			players--;
		}
		if (GUI.Button (new Rect(slotsDistanceFromSide+slotLength*1.5f+30+Screen.width/4-Screen.width/8-10,slotsDistanceFromTop+slotSize*2,Screen.width/4-Screen.width/8-20,slotSize*2),new GUIContent("ADD ONE","ADD ANOTHER PLAYER, WHICH IS AT THE MOMENT ONLY RETARDED BOTS"))) {
			players++;
		}
		GUI.Box (new Rect(slotsDistanceFromSide+slotLength*1.5f+30,slotsDistanceFromTop+slotSize*5,Screen.width/4-30,slotSize*2),"FORTRESSES PER TEAM: " + fortressNumber.ToString());
		if (GUI.Button (new Rect(slotsDistanceFromSide+slotLength*1.5f+30,slotsDistanceFromTop+slotSize*7,Screen.width/4-Screen.width/8-10,slotSize*2),new GUIContent("REMOVE ONE","REMOVES A FORTRESS FROM THE GAME"))) {
			fortressNumber--;
		}
		if (GUI.Button (new Rect(slotsDistanceFromSide+slotLength*1.5f+30+Screen.width/4-Screen.width/8-10,slotsDistanceFromTop+slotSize*7,Screen.width/4-Screen.width/8-20,slotSize*2),new GUIContent("ADD ONE","ADD ANOTHER FORTRESS TO THE GAME"))) {
			fortressNumber++;
		}
		GUI.Box (new Rect(slotsDistanceFromSide+slotLength*1.5f+30,slotsDistanceFromTop+slotSize*10,Screen.width/4-30,slotSize*2),"MAP WIDTH/HEIGHT: " + ((int)mapWidth).ToString() + "/" + ((int)mapHeight).ToString());
		mapWidth = GUI.HorizontalSlider(new Rect(slotsDistanceFromSide+slotLength*1.5f+30,slotsDistanceFromTop+slotSize*12,Screen.width/4-30,slotSize),mapWidth,20f,200f,skin.customStyles[2],skin.customStyles[3]);
		mapHeight = GUI.HorizontalSlider(new Rect(slotsDistanceFromSide+slotLength*1.5f+30,slotsDistanceFromTop+slotSize*13,Screen.width/4-30,slotSize),mapHeight,10f,50f,skin.customStyles[2],skin.customStyles[3]);
		GUI.Box (new Rect(Screen.width/3,10,Screen.width/3,50),"SKIRMISH");
		if (GUI.Button (new Rect(Screen.width-Screen.width/4+20,Screen.height-50,Screen.width/4-30,40),new GUIContent("START GAME","START THE GAME, DUH"))) {
			Application.LoadLevel ("ca_play_01");
		}
		if (GUI.Button (new Rect(Screen.width-Screen.width/4+20,Screen.height-90,Screen.width/4-30,40),new GUIContent("RANDOMIZE BOTS","RANDOMIZE EVERY SINGLE BOT, JUST FOR A BIT OF VARIANCE, EH?"))) {
			for (int a=0;a<players;a++) {
				types[a] = botTypes[Random.Range (0,botTypes.Length)];
			}
		}
		if (GUI.Button (new Rect(10,Screen.height-50,Screen.width/4-30,40),new GUIContent("BACK","BACK TO MAIN MENU"))) {
			Application.LoadLevel ("ca_menu");
		}
		GUI.Box (new Rect(slotsDistanceFromSide+Screen.width/4-30,Screen.height-50,Screen.width/2+20,40),"");
		GUI.Label (new Rect(slotsDistanceFromSide+Screen.width/4-25,Screen.height-48,Screen.width/2+15,40),GUI.tooltip);
		if (fortressNumber < 1) {
			fortressNumber = 1;
		}
		if (players < 1) {
			players = 1;
		}

		ds.players = players;
		ds.localID = localID;
		ds.localName = localName;
		ds.teamNames = teamNames;
		ds.fortressNumber = fortressNumber;
		ds.mapWidth = mapWidth;
		ds.mapHeight = mapHeight;
		
		ds.names = names;
		ds.types = types;
	}
}
