using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public int id;
	public int teamIndex;
	public string playerName;

	public Vector3 focusPoint;
	public Transform pointer;
	public GameObject selectedPurchaseOption;
	public Vector3 mousePos;
	public GlobalManager manager;
	public MapManager map;

	public bool botControlled;
	public bool local;
	public BotInput bot;

	public GameObject[] freindlyFortresses;
	public Transform nearestFortress;

	public LayerMask freindlyLayer;
	public int population;

	// Use this for initialization
	void Start () {
		manager = GameObject.FindGameObjectWithTag("Stats").GetComponent<GlobalManager>();
		map = manager.GetComponent<MapManager>();
		pointer = GameObject.Find ("Pointer").transform;
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
			pointer.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward;
			focusPoint = new Vector3(pointer.position.x,pointer.position.y,0);
			if (Input.GetButtonDown ("Fire1") && manager.tooltip.Length == 0) {
				PlacePurchase();
			}
		}
		Vector3 mp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mousePos = new Vector3 (mp.x,mp.y,0);
	}

	// Update is called once per frame
	public int PlacePurchase () {
		int error = 0;
		GetNearestFortress();
		if (selectedPurchaseOption) {
			if (population < manager.maxPopulation) {
				if (!Physics.CheckSphere(focusPoint,1,freindlyLayer) && nearestFortress) {
					if (manager.IsInsideBattlefield (focusPoint) && Vector3.Distance (mousePos,nearestFortress.position) < map.fRange) {
						Unit purchaseUnit = selectedPurchaseOption.GetComponent<Unit>();
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
