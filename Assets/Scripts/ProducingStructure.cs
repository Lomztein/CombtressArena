using UnityEngine;
using System.Collections;

public class ProducingStructure : MonoBehaviour {

	public float time;
	public GameObject unit;
	public float size;
	public float income;
	public Unit u;
	float locIncome;

	// Use this for initialization
	void Start () {
		InvokeRepeating("CreateUnit",time,time);
		u = GetComponent<Unit>();
		income = u.income;
	}

	void Update () {
		locIncome += (float)income * Time.deltaTime;
		if (locIncome >= 1 && u.manager) {
			u.manager.credits[u.playerIndex] += 1;
			locIncome = 0;
		}
	}
	
	// Update is called once per frame
	void CreateUnit () {
		Vector3 ranPos = Random.onUnitSphere * size;
		Vector3 newPos = new Vector3 (ranPos.x,ranPos.y,0) + transform.position;
		Vector3 newDir = new Vector3 (0,0,0);
		if (u.teamIndex == 0) {
			newDir = new Vector3 (0,0,180);
		}
		if (unit) {
			GameObject newUnit = (GameObject)Instantiate(unit,newPos,Quaternion.Euler(newDir));
			Unit newU = newUnit.GetComponent<Unit>();
			newU.teamIndex = u.teamIndex;
			newU.playerName = u.playerName;
			newU.teamName = u.teamName;
			newU.playerIndex = u.playerIndex;
		}
	}
}
