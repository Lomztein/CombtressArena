using UnityEngine;
using System.Collections;

public class SkirmishMenuNetworkSync : MonoBehaviour {

	public SkirmishMenuScript s;

	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info) {
		
		int nPlayers = 0;
		float nWidth = 0;
		float nHeight = 0;
		int nFortresses = 0;
		
		if (stream.isWriting) {
			nPlayers = s.players;
			nWidth = s.mapWidth;
			nHeight = s.mapHeight;
			nFortresses = s.fortressNumber;
			stream.Serialize(ref nPlayers);
			stream.Serialize(ref nWidth);
			stream.Serialize(ref nHeight);
			stream.Serialize(ref nFortresses);
		}else{
			stream.Serialize(ref nPlayers);
			stream.Serialize(ref nWidth);
			stream.Serialize(ref nHeight);
			stream.Serialize(ref nFortresses);
			s.players = nPlayers;
			s.mapWidth = nWidth;
			s.mapHeight = nHeight;
			s.fortressNumber = nFortresses;
		}
	}
}
