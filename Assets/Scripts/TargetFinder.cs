using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Unit))]

public class TargetFinder : MonoBehaviour {

	public Unit unit;
	public MapManager map;
	public bool ignoreFortress;
	public float maxHeight = 1;
	public float minHeight = -2;

	// Use this for initialization
	void Start () {
		map = GameObject.FindGameObjectWithTag("Stats").GetComponent<MapManager>();
		unit = GetComponent<Unit>();
		unit.GetLayers ();
		FindTarget();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!unit.target) {
			FindTarget();
		}else{
			if (unit.target.tag == "Fortress") {
				if (Physics.CheckSphere (transform.position,unit.weaponRange,unit.enemyLayer)) {
					FindTarget ();
				}
			}
		}
		if (unit.distanceToTarget >= unit.weaponRange && unit.targetOverride == null) {
			FindTarget ();
		}
		if (unit.targetUnit) {
			if (unit.targetUnit.tag != "Fortress") {
				if (unit.targetUnit.height >= maxHeight || unit.targetUnit.height <= minHeight) {
					Debug.Log ("Target was too high up",unit.targetUnit.gameObject);
					unit.target = null;
					unit.targetOverride = null;
				}
			}
		}
	}

	public void FindTarget () {
		Collider[] near = Physics.OverlapSphere(transform.position,unit.weaponRange,unit.enemyLayer);
		GameObject closest = null;
		float shortest = float.MaxValue;
		if (near.Length > 0) {
			for (int i=0;i<near.Length;i++) {
				GameObject other = near[i].gameObject;
				Unit otherU = other.GetComponent<Unit>();
				float distance = Vector3.Distance (transform.position,other.transform.position);
				if (otherU) {
					if (distance < shortest && otherU.teamIndex != unit.teamIndex && (otherU.height <= maxHeight && otherU.height >= minHeight)) {
						shortest = distance;
						closest = other;
					}
				}
			}
		}
		if (closest == null && ignoreFortress == false) {
			for (int i=0;i<map.fortresses.Length;i++) {
				if (map.fortresses[i]) {
					GameObject other = map.fortresses[i];
					Unit otherU = other.GetComponent<Unit>();
					float distance = Vector3.Distance (transform.position,other.transform.position);
					if (distance < shortest && otherU.teamIndex != unit.teamIndex) {
						shortest = distance;
						closest = other;
					}
				}
			}
		}
		if (closest) {
			unit.target = closest;
			unit.targetUnit = closest.GetComponent<Unit>();
		}
	}

	void OnDrawGizmos () {
		if (unit) {
			Gizmos.DrawWireSphere(transform.position,unit.weaponRange);
		}
	}
}