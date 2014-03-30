﻿using UnityEngine;
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

	public LayerMask freindlyLayer;

	// Use this for initialization
	void Start () {
		manager = GameObject.FindGameObjectWithTag("Stats").GetComponent<GlobalManager>();
		map = manager.GetComponent<MapManager>();
		pointer = GameObject.Find ("Pointer").transform;
		freindlyFortresses = new GameObject[map.fortressAmount];
		if (botControlled) {
			bot = (BotInput)gameObject.AddComponent("BotInput");
			bot.input = this;
			playerName = manager.botNames[Random.Range (0,manager.botNames.Length)] + " (BOT)";
		}
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
			pointer.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward;
			focusPoint = new Vector3(pointer.position.x,pointer.position.y,0);
			if (Input.GetButtonDown ("Fire1")) {
				PlacePurchase();
			}
		}
	}

	// Update is called once per frame
	public bool PlacePurchase () {
		bool placed = false;
		if (selectedPurchaseOption) {
			if (!Physics.CheckSphere(focusPoint,1,freindlyLayer)) {
				Unit purchaseUnit = selectedPurchaseOption.GetComponent<Unit>();
				int cost = purchaseUnit.cost;
				placed = true;
				if (manager.credits[id] >= cost) {
					manager.credits[id] -= cost;
					GameObject purchase = (GameObject)Instantiate(manager.factory,focusPoint,Quaternion.identity);
					Unit newU = purchase.GetComponent<Unit>();
					ProducingStructure pc = purchase.GetComponent<ProducingStructure>();
					if (pc) {
						pc.time = 10;
						pc.unit = selectedPurchaseOption;
						pc.income = purchaseUnit.income;
					}else{
						Debug.LogError ("No 'ProducingStructure' class was found on player " + id + "'s purchase.");
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
				}
			}
		}
		return placed;
	}

	void OnDrawGizmos () {
		Gizmos.DrawSphere (focusPoint,0.25f);
	}
}
