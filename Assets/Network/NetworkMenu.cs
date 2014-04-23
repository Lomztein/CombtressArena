using UnityEngine;
using System.Collections;

public class NetworkMenu : MonoBehaviour {

	public Rect bs;
	public float buttonDistance;
	public string gameType = "Combtress: Arena";
	public bool refreshing = false;
	public HostData[] hostData;

	public GUISkin skin;
	public int openSubMenu;
	
	void StartServer () {
		Network.InitializeServer(32,8008,!Network.HavePublicAddress());
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

		if (!(!Network.isClient && !Network.isServer)) {
			Application.LoadLevel ("ca_menu_skirmish");
		}
	}
	
	void OnServerInitialized () {
		Debug.Log ("Server initialized!");
	}

	void OnMasterServerEvent (MasterServerEvent mse) {
		if (mse == MasterServerEvent.RegistrationSucceeded) {
			Debug.Log ("Registration succeded!");
		}
	}

	void OnConnectedToServer () {
		Debug.Log ("Connected to server!");
	}

	void OnGUI () {
		GUI.skin = skin;
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
