using UnityEngine;
using System.Collections;

public class BotInput : MonoBehaviour {

	public GlobalManager manager;
	public PlayerController input;
	public bool placing;
	public string aiType;
	public Vector3 lastPlace;

	public float credits;
	public GameObject refinary;

	//public float enemyThreat;
	//public bool attacking;
	//public bool defending;

	/* AI Types
	 * balanced - purchases random units within credits range
	 * rusher - lot of cheap units
	 * steamroller - focus on best units possible
	 * defender - focus on defending alone
	 * attacker - focus on attacking alone
	*/

	void Start () {
		GameObject stats = GameObject.FindGameObjectWithTag("Stats");
		manager = stats.GetComponent<GlobalManager>();
		for (int i=0;i<manager.purchaseables.Length;i++) {
			if (manager.purchaseables[i].name == "Refinary") {
				refinary = manager.purchaseables[i];
			}
		}
	}

	void Update () {
		credits = manager.credits[input.id];
	}

	GameObject PickUnit () {
		GameObject locUnit = null;
		if (aiType == "balanced") {
			locUnit = manager.purchaseables[Random.Range (0,manager.purchaseables.Length)];
		}
		if (aiType == "defender") {
			if (input.population < 1) {
				locUnit = refinary;
			}else{
				locUnit = manager.turrets[Random.Range (0,manager.turrets.Length)];
			}
		}
		if (aiType == "attacker") {
			locUnit = manager.purchaseables[Random.Range (0,manager.turrets.Length)];
			Unit locU = locUnit.GetComponent<Unit>();
			while (locU.unitType == "structure" ) {
				locUnit = manager.purchaseables[Random.Range (0,manager.purchaseables.Length)];
				locU = locUnit.GetComponent<Unit>();
			}
		}
		if (aiType == "steamroller") {
			locUnit = FindStrongestUnit (manager.purchaseables);
		}
		if (aiType == "rusher") {
			locUnit = manager.purchaseables[Random.Range (0,manager.purchaseables.Length)];
			Unit locU = locUnit.GetComponent<Unit>();
			while (locU.cost > Mathf.Max (100,input.population*50)) {
				locUnit = manager.purchaseables[Random.Range (0,manager.purchaseables.Length)];
				locU = locUnit.GetComponent<Unit>();
			}
		}
		return locUnit;
	}

	GameObject FindStrongestUnit (GameObject[] units) {
		GameObject strongest = units[0];
		float strength = 0;
		for (int i=0;i<units.Length;i++) {
			Unit u = units[i].GetComponent<Unit>();
			if (credits >= u.cost) {
				if (u.newWeapon) {
					WeaponScript uw = u.newWeapon.GetComponent<WeaponScript>();
					float nStrength = uw.damage / uw.reloadTime;
					if (nStrength > strength) {
						strongest = units[i];
					}
				}
			}else{
				return strongest;
			}
		}
		return strongest;
	}
	
	bool DoCreateUnit () {
		bool create = false;
		if (aiType == "balanced" || aiType == "defender" || aiType == "attacker") {
			if (Random.Range (0,100) == 1) {
				create = true;
			}
		}
		if (aiType == "rusher") {
			if (credits > Mathf.Max (100,input.population*10)) {
				create = true;
			}
		}
		if (aiType == "steamroller") {
			if (credits >= Mathf.Max (500,input.population * 100)) {
				create = true;
			}
		}
		return create;
	}

	Unit FindWeakestStructure (Unit tryingUnit) {
		GameObject[] units = new GameObject[0];
		UnitManager um = manager.GetComponent<UnitManager>();
		if (input.teamIndex == 0) {
			units = um.team0;
		}else{
			units = um.team1;
		}
		GameObject weakest = null;
		int cheapestCost = int.MaxValue;
		if (tryingUnit.cost <= credits) {
			Debug.Log ("Trying to sell something so I can place a " + tryingUnit.name,gameObject);
			for (int i=0;i<units.Length;i++) {
				if (units[i]) {
					Unit locUnit = units[i].GetComponent<Unit>();
					if (locUnit.unitType == "structure" && locUnit.teamIndex == input.teamIndex) {
						if (tryingUnit.cost > locUnit.cost) {
							if (cheapestCost > locUnit.cost) {
								cheapestCost = locUnit.cost;
								weakest = locUnit.gameObject;
							}
						}
					}
				}
			}
		}
		if (weakest) {
			return weakest.GetComponent<Unit>();
		}
		return null;
	}

	void FixedUpdate () {
		if (DoCreateUnit () && placing == false) {
			GameObject newUnit = null;
			newUnit = PickUnit ();
			input.selectedPurchaseOption = newUnit;
			GameObject newF = input.freindlyFortresses[Random.Range (0,input.freindlyFortresses.Length)];
			if (newF) {
				input.focusPoint = newF.transform.position;
				placing = true;
			}
		}
		if (placing) {
			int value = input.PlacePurchase();
			if (value == 0) {
				lastPlace = input.focusPoint;
				placing = false;
			}
			if (value == 1) {
				GameObject newUnit = PickUnit ();
				input.selectedPurchaseOption = newUnit;
			}
			if (value == 2) {
				GameObject newF = input.freindlyFortresses[Random.Range (0,input.freindlyFortresses.Length)];
				if (newF) {
					input.focusPoint = newF.transform.position;
				}
			}
			if (value == 3) {
				input.focusPoint += manager.nearbyVectors[Random.Range (0,manager.nearbyVectors.Length)];
			}
			if (value == 4) {
				Unit weakest = FindWeakestStructure (input.selectedPurchaseOption.GetComponent<Unit>());
				if (weakest) {
					weakest.Sell ();
				}
			}
		}
	}
}