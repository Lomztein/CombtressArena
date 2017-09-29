using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Unit))]

public class TurretController : MonoBehaviour, IUnitController, IBalanceItem {

	Transform stand;
	Unit unit;

    public float GetDamageValue() {
        return unit.newWeapon.GetComponent<WeaponScript> ().GetDPS ();
    }

    public float GetHealthValue() {
        return GetComponent<HealthScript> ().maxHealth;
    }

    public float GetSpeed() {
        return 0f;
    }

    public string GetDamageType() {
        return unit.newWeapon.GetComponent<WeaponScript> ().bulletType.GetComponent<BulletScript>().damageType;
    }

    public string GetHealthType() {
        return GetComponent<HealthScript> ().armorType;
    }

    // Use this for initialization
    void Start() {
		unit = GetComponent<Unit>();
        BotInput.allBalanceItems [ unit.teamIndex ].Add (this);

        stand = transform.Find ("Sprite");
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

    private void OnDestroy() {
        BotInput.allBalanceItems [ unit.teamIndex ].Remove (this);
    }
}
