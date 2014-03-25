using UnityEngine;
using System.Collections;

public class UnitManager : MonoBehaviour {

	public GameObject[] team0;
	public GameObject[] team1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		team0 = GameObject.FindGameObjectsWithTag("Team0");	
		team1 = GameObject.FindGameObjectsWithTag("Team1");	
	}
}
