using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public int id;
	public int teamIndex;
	public string playerName;

	public Vector3 focusPoint;
	public Transform pointer;
	public GameObject selectedPurchaseOption;
	public GlobalManager manager;
	public MapManager map;

	public bool botControlled;
	public bool local;
	public BotInput bot;

	public GameObject[] freindlyFortresses;
	public Transform nearestFortress;

	// Use this for initialization
	void Start () {
		manager = GetComponent<GlobalManager>();
		map = GetComponent<MapManager>();
		if (botControlled) {
			bot = new BotInput();
			bot.input = this;
		}
		freindlyFortresses = new GameObject[map.fortressAmount];
		int fortressIndex = 0;
		for (int i=0;i<map.fortressAmount*2;i++) {
			Unit fu = map.fortresses[i].GetComponent<Unit>();
			if (fu.teamIndex == teamIndex) {
				freindlyFortresses[fortressIndex] = map.fortresses[i];
				fortressIndex++;
			}
		}
	}

	void FixedUpdate () {
		if (local) {
			Transform closest = freindlyFortresses[0].transform;
			float distance = float.MaxValue;
			for (int i=0;i<freindlyFortresses.Length;i++) {
				float cd = Vector3.Distance (focusPoint,freindlyFortresses[i].transform.position);
				if (cd < distance) {
					closest = freindlyFortresses[i].transform;
					distance = cd;
				}
			}
			nearestFortress = closest;
		}
	}
	
	void Update() {
		if (botControlled == false) {
			focusPoint = pointer.position;
			if (Input.GetButtonDown ("Fire1")) {
				PlacePurchase();
			}
		}else{
			Transform f = freindlyFortresses[Random.Range (0,freindlyFortresses.Length)].transform;

		}
	}

	// Update is called once per frame
	void PlacePurchase () {
		Unit purchaseUnit = selectedPurchaseOption.GetComponent<Unit>();
		int cost = purchaseUnit.income*500;
		if (manager.credits[id] <= cost) {
			manager.credits[id] -= cost;
			GameObject purchase = (GameObject)Instantiate(selectedPurchaseOption,focusPoint,Quaternion.identity);
			Unit newU = purchase.GetComponent<Unit>();
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
		}
	}

	void OnDrawGizmos () {
		Gizmos.DrawSphere (focusPoint,0.25f);
	}
}
