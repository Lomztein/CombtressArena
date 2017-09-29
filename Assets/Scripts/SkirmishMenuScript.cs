using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkirmishMenuScript : MonoBehaviour {

	public int players;
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
	public SkirmishMenuPlayer localPlayer;
	public int[] playerIDs;
	public bool[] occupied;
	public string[] randomBotNames;

	public string[] botNames;
	public string[] botTypes;
	public string[] botTypeDescriptions;
	public bool typeMenuOpen;
	public int selectedIndex;

	public Rect bs;
	public float buttonDistance;
	public string gameType = "Combtress: Arena";
	public bool refreshing = false;
	public HostData[] hostData;
	public int openSubMenu;

	public DataCarrierScript ds;
	public GameObject playerPrefab;

	public static SkirmishMenuScript current;

	void Start () {
		current = this;
		GameObject dc = GameObject.Find ("DataCarrier");
		ds = dc.GetComponent<DataCarrierScript>();
	}

	void UpdateArrays () {
		string[] prevNames = names;
		string[] prevTypes = types;
		bool[] prevOccupied = occupied;

		names = new string[players];
		types = new string[players];
		occupied = new bool[players];

		for (int i=0;i<players;i++) {
			if (i<prevNames.Length) {
				names[i] = prevNames[i];
				types[i] = prevTypes[i];
			}
			if (i<prevOccupied.Length) {
				occupied[i] = prevOccupied[i];
			}
			if (names[i] == null) {
				names[i] = botNames[Random.Range (0,botNames.Length)];
			}
			if (types[i] == null) {
				types[i] = "balanced";
			}
		}
	}

	void OnPlayerConnected (NetworkPlayer player) {
		Debug.Log ("New player joined on IP: " + player.externalIP + ", " + player.ipAddress);
	}

	void OnPlayerDisconnected (NetworkPlayer player) {
		Debug.Log ("Player " + player.externalIP + " left, cleaning up.");
		Network.DestroyPlayerObjects(player);
		Network.RemoveRPCs(player);
	}

	void OnDisconnectedFromServer () {
		Debug.Log ("Lost connection to server");
		Application.LoadLevel ("ca_menu");
	}

	void StartServer () {
		Network.InitializeServer(5,8008,!Network.HavePublicAddress());
		MasterServer.RegisterHost(gameType,"SERVER_" + Random.Range (0,1000).ToString ());
	}
	
	void RefreshHostList () {
		MasterServer.RequestHostList(gameType);
		refreshing = true;
	}
	
	void Update () {
		if (refreshing) {
			if (MasterServer.PollHostList().Length != 0) {
				refreshing = false;
				hostData = MasterServer.PollHostList ();
			}
		}
	}
	
	void OnServerInitialized () {
		Debug.Log ("Server initialized!");
		SpawnPlayer ();
	}
	
	void OnMasterServerEvent (MasterServerEvent mse) {
		if (mse == MasterServerEvent.RegistrationSucceeded) {
			Debug.Log ("Registration succeded!");
		}
	}
	
	void OnConnectedToServer () {
		Debug.Log ("Connected to server!");
		SpawnPlayer ();
	}

	void SpawnPlayer () {
		GameObject newPlayer = (GameObject)Network.Instantiate (playerPrefab,Vector3.zero,Quaternion.identity,0);
		SkirmishMenuPlayer other = newPlayer.GetComponent<SkirmishMenuPlayer>();
		int index = 0;
		other.id = index;
		localPlayer = other;
	}

	void StartGame () {
		Network.RemoveRPCsInGroup (0);
		GetComponent<NetworkView>().RPC ("LoadLevel",RPCMode.AllBuffered,"ca_play_01");
	}

	[RPC] void RequestPosition (string newName,NetworkViewID id, NetworkMessageInfo info) {
		int index = 0;
		while (occupied[index]) {
			index++;
		}
		Debug.Log ("Send position: " + index);
		NetworkView.Find (id).RPC ("GetPosition",info.sender,index);
		occupied[index] = true;
		names[index] = newName;
	}

	[RPC] void RequestPlayerData (NetworkViewID id) {
		NetworkView view = NetworkView.Find (id);
		for (int i=0;i<players;i++) {
			view.RPC ("GetPlayerData",view.owner,i,names[i],types[i],occupied[i]);
		}
	}

	[RPC] void ChangeName (string name, int index) {
		if (index >= 0) {
			names[index] = name;
		}
	}

	[RPC] void ChangePosition (NetworkViewID id, int oldPos, int newPos) {
		SkirmishMenuPlayer player = NetworkView.Find (id).GetComponent<SkirmishMenuPlayer>();
		player.id = newPos;
		if (oldPos >= 0 && oldPos < players) {
			names[oldPos] = names[newPos];
			types[oldPos] = types[oldPos];
			occupied[oldPos] = false;
		}
		names[newPos] = player.playerName;
		types[newPos] = "balanced";
		occupied[newPos] = true;
	}

	[RPC] void RemovePlayer (NetworkViewID id) {
		SkirmishMenuPlayer player = NetworkView.Find (id).GetComponent<SkirmishMenuPlayer>();
		names[player.id] = botNames[Random.Range (0,botNames.Length)];
		types[player.id] = "balanced";
		occupied[player.id] = false;
		player.id = -1;
	}

	[RPC] void ChangeBotType (int bot, string type) {
		types[bot] = type;
	}

	/*void LoadLevel (string level) {
		Network.SetSendingEnabled(0,false);
		Network.isMessageQueueRunning = false;
		Network.SetLevelPrefix(1);
		Application.LoadLevel (level);
		yield;
		yield;
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0,true);
		foreach (GameObject go in Object.FindObjectsOfType(GameObject)) {
			go.SendMessage ("OnNetworkLoadedLevel",SendMessageOptions.DontRequireReceiver);
		}
	}*/

	void OnGUI () {
		GUI.skin = skin;
		if (Network.isServer || Network.isClient) {
			float totSize = slotsDistanceFromTop + ((players+3)*(slotSize+slotDistance));
			if (players != names.Length) {
				UpdateArrays();
			}
			slotLength = Screen.width/2-slotsDistanceFromSide;
			if (players == names.Length) {
				int j=0;
				for (int i=0;i<players;i++) {
					string locName = "";
					if (localPlayer.id == i) {
						locName = localPlayer.playerName.ToUpper () + " - YOU";
						names[i] = localPlayer.playerName.ToUpper ();
					}else{
						if (occupied[i]) {
							locName = names[i].ToUpper ();
						}else{
							locName = names[i].ToUpper () + " - " + types[i].ToUpper () + " - " + "BOT";
						}
					}

                        if (i == 0) {
                            GUI.Box (new Rect (slotsDistanceFromSide, slotsDistanceFromTop + slotsPosOffset + ((i + j) * (slotSize + slotDistance)), slotLength, slotSize), "TEAM 1" + " - " + teamNames [ 0 ].ToUpper ());
                            if (!typeMenuOpen)
                                teamNames [ 0 ] = GUI.TextField (new Rect (slotLength + slotsDistanceFromSide, slotsDistanceFromTop + slotsPosOffset + ((i + j) * (slotSize + slotDistance)), slotLength / 2, slotSize), teamNames [ 0 ].ToUpper ());
                            j++;
                        }
                        if (i == players / 2) {
                            GUI.Box (new Rect (slotsDistanceFromSide, slotsDistanceFromTop + slotsPosOffset + ((i + j) * (slotSize + slotDistance)), slotLength, slotSize), "TEAM 2" + " - " + teamNames [ 1 ].ToUpper ());
                            if (!typeMenuOpen)
                                teamNames [ 1 ] = GUI.TextField (new Rect (slotLength + slotsDistanceFromSide, slotsDistanceFromTop + slotsPosOffset + ((i + j) * (slotSize + slotDistance)), slotLength / 2, slotSize), teamNames [ 1 ].ToUpper ());
                            j++;
                        }

                    GUI.Box (new Rect (slotsDistanceFromSide, slotsDistanceFromTop + slotsPosOffset + ((i + j) * (slotSize + slotDistance)), slotLength, slotSize), (i + 1).ToString () + " -- " + locName, skin.customStyles [ 1 ]);
                    if (typeMenuOpen == false) { // I hate the old GUI system.
                        if (localPlayer.id != i && occupied[i] == false) {
							if (GUI.Button (new Rect(slotLength+slotsDistanceFromSide,slotsDistanceFromTop+slotsPosOffset + ((i+j)*(slotSize+slotDistance)),slotLength/4,slotSize),new GUIContent("TAKE","SWAP SLOTS WITH THIS DUDE"))) {
								GetComponent<NetworkView>().RPC ("ChangePosition",RPCMode.All,localPlayer.GetComponent<NetworkView>().viewID,localPlayer.id,i);
								GetComponent<NetworkView>().RPC ("ChangeName",RPCMode.All,localPlayer.playerName,i);
							}
							if (Network.isServer) {
								if (GUI.Button (new Rect(slotLength+slotsDistanceFromSide+slotLength/4,slotsDistanceFromTop+slotsPosOffset + ((i+j)*(slotSize+slotDistance)),slotLength/4,slotSize),new GUIContent("TYPE","CHANGE THE TYPE OF BOT THIS DUDE IS"))) {
									typeMenuOpen = true;
									selectedIndex = i;
									types[i] = "Changing";
								}
							}
						}else{
							if (localPlayer.id == i) {
								if (GUI.Button (new Rect(slotLength+slotsDistanceFromSide,slotsDistanceFromTop+slotsPosOffset + ((i+j)*(slotSize+slotDistance)),slotLength/4,slotSize),new GUIContent("LEAVE","LEAVE THIS SLOT AND BECOME AN AWESOME GHOST SPECTATOR"))) {
									GetComponent<NetworkView>().RPC ("RemovePlayer",RPCMode.All,localPlayer.GetComponent<NetworkView>().viewID);
								}
								localPlayer.playerName = GUI.TextField (new Rect(slotLength+slotsDistanceFromSide+slotLength/4,slotsDistanceFromTop+slotsPosOffset + ((i+j)*(slotSize+slotDistance)),slotLength/4,slotSize),localPlayer.playerName.ToUpper());
								if (GUI.changed) { 
									GetComponent<NetworkView>().RPC ("ChangeName",RPCMode.All,localPlayer.playerName,localPlayer.id);
								}
							}
						}
					}
				}
				if (typeMenuOpen) {
					for (int i=0;i<botTypes.Length;i++) {
						if (GUI.Button (new Rect(slotLength+slotsDistanceFromSide,slotsDistanceFromTop + ((i)*(slotSize+slotDistance)),slotLength/2,slotSize),new GUIContent(botTypes[i].ToUpper(),botTypeDescriptions[i].ToUpper()))) {
							GetComponent<NetworkView>().RPC ("ChangeBotType",RPCMode.All,selectedIndex,botTypes[i]);
							typeMenuOpen = false;
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
			if (Network.isServer) {
				if (GUI.Button (new Rect(slotsDistanceFromSide+slotLength*1.5f+30,slotsDistanceFromTop+slotSize*2,Screen.width/4-Screen.width/8-10,slotSize*2),new GUIContent("REMOVE ONE","REMOVES A PLAYER FROM THE GAME"))) {
					players--;
				}
				if (GUI.Button (new Rect(slotsDistanceFromSide+slotLength*1.5f+30+Screen.width/4-Screen.width/8-10,slotsDistanceFromTop+slotSize*2,Screen.width/4-Screen.width/8-20,slotSize*2),new GUIContent("ADD ONE","ADD ANOTHER PLAYER, WHICH IS AT THE MOMENT ONLY RETARDED BOTS"))) {
					players++;
				}
			}
			GUI.Box (new Rect(slotsDistanceFromSide+slotLength*1.5f+30,slotsDistanceFromTop+slotSize*5,Screen.width/4-30,slotSize*2),"FORTRESSES PER TEAM: " + fortressNumber.ToString());
			if (Network.isServer) {
				if (GUI.Button (new Rect(slotsDistanceFromSide+slotLength*1.5f+30,slotsDistanceFromTop+slotSize*7,Screen.width/4-Screen.width/8-10,slotSize*2),new GUIContent("REMOVE ONE","REMOVES A FORTRESS FROM THE GAME"))) {
					fortressNumber--;
				}
				if (GUI.Button (new Rect(slotsDistanceFromSide+slotLength*1.5f+30+Screen.width/4-Screen.width/8-10,slotsDistanceFromTop+slotSize*7,Screen.width/4-Screen.width/8-20,slotSize*2),new GUIContent("ADD ONE","ADD ANOTHER FORTRESS TO THE GAME"))) {
					fortressNumber++;
				}
			}
			GUI.Box (new Rect(slotsDistanceFromSide+slotLength*1.5f+30,slotsDistanceFromTop+slotSize*10,Screen.width/4-30,slotSize*2),"MAP WIDTH/HEIGHT: " + ((int)mapWidth).ToString() + "/" + ((int)mapHeight).ToString());
			if (Network.isServer) {
				mapWidth = GUI.HorizontalSlider(new Rect(slotsDistanceFromSide+slotLength*1.5f+30,slotsDistanceFromTop+slotSize*12,Screen.width/4-30,slotSize),mapWidth,20f,200f,skin.customStyles[2],skin.customStyles[3]);
				mapHeight = GUI.HorizontalSlider(new Rect(slotsDistanceFromSide+slotLength*1.5f+30,slotsDistanceFromTop+slotSize*13,Screen.width/4-30,slotSize),mapHeight,10f,50f,skin.customStyles[2],skin.customStyles[3]);
			}
			GUI.Box (new Rect(Screen.width/3,10,Screen.width/3,50),"SKIRMISH");
			if (Network.isServer) {
				if (GUI.Button (new Rect(Screen.width-Screen.width/4+20,Screen.height-50,Screen.width/4-30,40),new GUIContent("START GAME","START THE GAME, DUH"))) {
					Application.LoadLevel ("ca_play_01");
				}
			}
			if (Network.isServer) {
				if (GUI.Button (new Rect(Screen.width-Screen.width/4+20,Screen.height-90,Screen.width/4-30,40),new GUIContent("RANDOMIZE BOTS","RANDOMIZE EVERY SINGLE BOT, JUST FOR A BIT OF VARIANCE, EH?"))) {
					for (int a=0;a<players;a++) {
						GetComponent<NetworkView>().RPC ("ChangeBotType",RPCMode.All,a,botTypes[Random.Range (0,botTypes.Length)]);
					}
				}
			}
			if (GUI.Button (new Rect(10,Screen.height-50,Screen.width/4-30,40),new GUIContent("BACK","BACK TO MAIN MENU"))) {
				Application.LoadLevel ("ca_menu");
				Network.Disconnect();
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
			ds.localID = localPlayer.id;
			ds.localName = localPlayer.playerName;
			ds.teamNames = teamNames;
			ds.fortressNumber = fortressNumber;
			ds.mapWidth = mapWidth;
			ds.mapHeight = mapHeight;
			
			ds.names = names;
			ds.types = types;
		}else{
			if (GUI.Button (new Rect(bs.x,bs.y,bs.width,bs.height),"START SERVER")) {
				StartServer ();
			}
			if (GUI.Button (new Rect(bs.x,bs.y + buttonDistance + bs.height ,bs.width,bs.height),"REFRESH")) {
				RefreshHostList ();
			}
			if (hostData != null) {
				for (int i=0;i<hostData.Length;i++) {
					string text = hostData[i].gameName + " -- " + hostData[i].connectedPlayers + " / " + hostData[i].playerLimit + " PLAYERS";
					if (hostData[i].passwordProtected) {
						text = text + " - PASSWORD PROTECTED";
					}
					if (GUI.Button (new Rect(bs.x + buttonDistance + bs.width,bs.y + ( buttonDistance + bs.height/2 ) * i,Screen.width - bs.width - buttonDistance - bs.x*2,bs.height/2),text)) {
						Network.Connect (hostData [i]);
					}
				}
			}
		}
	}
}
