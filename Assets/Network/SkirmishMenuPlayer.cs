using UnityEngine;
using System.Collections;

public class SkirmishMenuPlayer : MonoBehaviour {

	public SkirmishMenuScript menu;
	public string playerName;
	public int id;

	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info) {

		int nID = 0;

		if (stream.isWriting) {
			nID = id;
			stream.Serialize(ref nID);
		}else{
			stream.Serialize(ref nID);
			id = nID;
		}
	}

	void Start () {
		if (GetComponent<NetworkView>().isMine) {
			SkirmishMenuScript.current.GetComponent<NetworkView>().RPC ("RequestPosition",RPCMode.Server,playerName,GetComponent<NetworkView>().viewID);
			SkirmishMenuScript.current.GetComponent<NetworkView>().RPC ("RequestPlayerData",RPCMode.Server,GetComponent<NetworkView>().viewID);
		}
		Debug.Log ("Player spawned!");
	}

	[RPC]
	void GetPosition (int pos) {
		id = pos;
		Debug.Log ("Recieved starting position: " + pos);
	}

	[RPC]
	void GetPlayerData (int index, string newName, string newType, bool newOccupied) {
		SkirmishMenuScript.current.names[index] = newName;
		SkirmishMenuScript.current.types[index] = newType;
		SkirmishMenuScript.current.occupied[index] = newOccupied;
	}
}
