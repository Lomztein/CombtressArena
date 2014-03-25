using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour {

	public float mapWidth;
	public float mapHeight;
	public int fortressAmount;
	public float fDistance;
	public float fDistanceFromEnd;
	public float fRange;

	public GameObject fortress;
	public GameObject[] fortresses;

	// Use this for initialization
	void Start () {
		float fortressOffsetY = ((float)fortressAmount-1)*fDistance;
		fortresses = new GameObject[fortressAmount*2];
		for (int i=0;i<fortressAmount*2;i++) {
			Vector3 newPos = new Vector3 (0,0,0);
			int newTeam = -1;
			if (i < fortressAmount) {
				newPos = new Vector3 (mapWidth-fDistanceFromEnd,fortressOffsetY - (i * fDistance*2),0);
				newTeam = 0;
			}else{
				newPos = new Vector3 (-mapWidth+fDistanceFromEnd,fortressOffsetY - ((i-fortressAmount) * fDistance*2),0);
				newTeam = 1;
			}
			GameObject nf = (GameObject)Instantiate(fortress,newPos,Quaternion.identity);
			fortresses[i] = nf;
			nf.GetComponent<Unit>().teamIndex = newTeam;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
