using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Unit))]

public class TurretController : MonoBehaviour {

	Transform stand;
	Unit unit;

	// Use this for initialization
	void Start () {
		stand = transform.FindChild ("Stand");
		unit = GetComponent<Unit>();
		if (unit.teamIndex == 0) {
			transform.rotation = Quaternion.Euler (0f,0f,180f);
		}else{
			transform.rotation = Quaternion.Euler (0f,0f,0f);
		}
		if (unit.health.armorType == "light") {
			stand.localScale = Vector3.one * 0.5f;
		}
		if (unit.health.armorType == "medium") {
			stand.localScale = Vector3.one * 0.75f;
		}
		if (unit.health.armorType == "heavy") {
			stand.localScale = Vector3.one * 1f;
		}
	}

	void Update () {
		if (unit.target) {
			if (unit.distanceToTarget < unit.weaponRange) {
				unit.weaponScript.Fire ();
			}
		}
	}
}
