using UnityEngine;
using System.Collections;

public class DataCarrierScript : MonoBehaviour {

	public int players;
	public int localID;
	public string localName;
	public string[] teamNames;
	public int fortressNumber;
	public float mapWidth;
	public float mapHeight;

	public string[] names;
	public string[] types;

	// Use this for initialization
	void Start () {
		GameObject[] dcs = GameObject.FindGameObjectsWithTag("DataCarrier");
		if (dcs.Length > 1) {
			Destroy(gameObject);
		}
		Transform.DontDestroyOnLoad(gameObject);
	}
}
