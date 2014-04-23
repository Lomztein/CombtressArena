using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public int id;
	public int teamIndex;
	public string playerName;

	public Vector3 focusPoint;
	public Unit focusUnit;
	public GameObject selectedPurchaseOption;
	public Vector3 mousePos;
	public GlobalManager manager;
	public MapManager map;

	public bool botControlled;
	public bool local;
	public BotInput bot;

	public GameObject[] freindlyFortresses;
	public Transform nearestFortress;

	public int selectedCount;
	public Unit[] selectedUnits;
	public Texture2D[] selectedSprites;

	public LayerMask freindlyLayer;
	public int population;

	public GameObject radar;

	// Use this for initialization
	void Start () {
		manager = GameObject.FindGameObjectWithTag("Stats").GetComponent<GlobalManager>();
		map = manager.GetComponent<MapManager>();
		freindlyFortresses = new GameObject[map.fortressAmount];
		int fortressIndex = 0;
		for (int i=0;i<map.fortressAmount*2;i++) {
			Unit fu = map.fortresses[i].GetComponent<Unit>();
			if (fu.teamIndex == teamIndex) {
				freindlyFortresses[fortressIndex] = map.fortresses[i];
				fortressIndex++;
			}
		}
		name = playerName;
	}

	public void UpdateSelectedUnits () {
		Debug.Log("Updating selected units");
		GameObject[] sprs = GameObject.FindGameObjectsWithTag("SelectorSprite");
		selectedUnits = new Unit[sprs.Length];
		selectedSprites = new Texture2D[sprs.Length];
		selectedCount = sprs.Length;
		for (int i=0;i<sprs.Length;i++) {
			selectedUnits[i] = sprs[i].transform.parent.GetComponent<Unit>();
			selectedSprites[i] = GetSprite (selectedUnits[i]);
		}
	}

	Texture2D GetSprite (Unit u) {
		if (u.unitType == "infantry") {
			return u.GetComponent<InfantryController>().purchaseTexture;
		}
		if (u.unitType == "structure") {
			if (u.newWeapon) {
				return u.newWeapon.transform.FindChild ("Sprite").GetComponent<SpriteRenderer>().sprite.texture;
			}
		}
		return u.transform.FindChild ("Sprite").GetComponent<SpriteRenderer>().sprite.texture;
	}
	
	void FixedUpdate () {
		if (local) {
			GetNearestFortress();
		}
		manager.populations[id] = population;
	}
	void GetNearestFortress () {
		Transform closest = null;
		float distance = float.MaxValue;
		for (int i=0;i<freindlyFortresses.Length;i++) {
			if (freindlyFortresses[i]) {
				float cd = Vector3.Distance (focusPoint,freindlyFortresses[i].transform.position);
				if (cd < distance) {
					closest = freindlyFortresses[i].transform;
					distance = cd;
				}
			}
		}
		nearestFortress = closest;
	}
	
	void Update() {
		if (botControlled == false) {
			focusPoint = new Vector3(mousePos.x,mousePos.y,0);
			if (Input.GetButtonDown ("Fire1") && manager.tooltip.Length == 0) {
				PlacePurchase();
			}
		}
		Vector3 mp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mousePos = new Vector3 (mp.x,mp.y,0);
	}

	public bool CanPlace (Unit u, Vector3 pos) {
		Collider[] nearby = Physics.OverlapSphere(pos,1,freindlyLayer);
		if (nearby.Length > 0) {
			if (nearby.Length == 1) {
				if (nearby[0].gameObject.tag == "Shield") {
					return true;
				}
			}else{
				return false;
			}
		}else{
			return true;
		}
		return false;
	}

	// Update is called once per frame
	public int PlacePurchase () {
		int error = 0;
		GetNearestFortress();
		if (selectedPurchaseOption) {
			Unit purchaseUnit = selectedPurchaseOption.GetComponent<Unit>();
			if (population < manager.maxPopulation) {
				if (CanPlace (purchaseUnit,focusPoint) && nearestFortress) {
					if (manager.IsInsideBattlefield (focusPoint) && Vector3.Distance (focusPoint,nearestFortress.position) < map.fRange) {
						int cost = purchaseUnit.cost;
						if (manager.credits[id] >= cost) {
							manager.credits[id] -= cost;
							GameObject newPurchasePrefab;
							if (purchaseUnit.unitType != "structure") {
								newPurchasePrefab = manager.factory;
							}else{
								newPurchasePrefab = selectedPurchaseOption;
							}
							GameObject purchase = (GameObject)Instantiate(newPurchasePrefab,focusPoint,Quaternion.identity);
							if (newPurchasePrefab.name == "Radar") {
								radar = purchase;
							}
							population++;
							Unit newU = purchase.GetComponent<Unit>();
							if (purchaseUnit.unitType != "structure") {
								ProducingStructure pc = purchase.GetComponent<ProducingStructure>();
								if (pc) {
									pc.unit = selectedPurchaseOption;
									pc.income = purchaseUnit.income;
								}else{
									Debug.LogError ("No 'ProducingStructure' class was found on player " + id + "'s purchase.");
								}
							}
							if (newU) {
								newU.teamIndex = teamIndex;
								newU.playerIndex = id;
								newU.playerName = playerName;
								newU.teamName = manager.teamNames[teamIndex];
								if (teamIndex == 0) {
									newU.freindlyLayer = manager.team0Layer;
									newU.enemyLayer = manager.team1Layer;
								}else{
									newU.freindlyLayer = manager.team1Layer;
									newU.enemyLayer = manager.team0Layer;
								}
							}else{
								Debug.LogError ("No 'Unit' class was found on player " + id + "'s purchase.");
							}
						}else{
							error = 1;
						}
					}else{
						error = 2;
					}
				}else{
					error = 3;
				}
			}else{
				error = 4;
			}
		}else{
			error = 5;
		}
		if (local && botControlled == false) {
			if (!Input.GetButton ("Shift")) {
				selectedPurchaseOption = null;
			}
		}
		return error;
	}

	void OnDrawGizmos () {
		Gizmos.DrawSphere (focusPoint,0.25f);
	}
}
