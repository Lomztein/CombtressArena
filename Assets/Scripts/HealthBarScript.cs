using UnityEngine;
using System.Collections;

public class HealthBarScript : MonoBehaviour {

	public HealthScript health;
	public Unit unit;
	public GlobalManager manager;

	public bool visible;

	// Use this for initialization
	void Start () {
		health = transform.parent.GetComponent<HealthScript>();
		unit = health.unit;
		manager = unit.manager;
	}
	
	// Update is called once per frame
	void Update () {
		if ((Vector3.Distance (manager.mousePos,transform.position) < 5) || (unit.playerIndex == unit.manager.localID && health.health <= health.maxHealth/2) || unit.selected) {
			visible = true;
		}else{
			visible = false;
		}
	}
}
