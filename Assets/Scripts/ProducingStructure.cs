﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProducingStructure : MonoBehaviour {

	public float time;
	public GameObject unit;
	public float size;
	public float income;
	public bool giveAllPlayers;
	public bool overrideTime;
	public Unit u;
	float locIncome;

    public static List<Unit>[] placementNodes;

	// Use this for initialization
	void Start () {
		if (unit) {
			HealthScript h = unit.GetComponent<HealthScript>();
			if (h && overrideTime == false) {
				if (h.armorType == "light") {
					time = 20;
				}
				if (h.armorType == "medium") {
					time = 30;
				}
				if (h.armorType == "heavy") {
					time = 40;
				}
			}
		}
		InvokeRepeating("CreateUnit",time,time);
		u = GetComponent<Unit>();
        placementNodes[u.teamIndex].Add (u);
	}

    void OnDestroy () {
        placementNodes[u.teamIndex].Remove (u);
    }

    public static void InitPlacementNodesList () {
        if (placementNodes == null) {
            placementNodes = new List<Unit>[2];
            for (int i = 0; i < placementNodes.Length; i++) {
                placementNodes[i] = new List<Unit> ();
            }
        }
    }

    public static void SetFurthestFactory (int team) {

        float depth = MapManager.cur.mapWidth;

        for (int i = 0; i < placementNodes[team].Count; i++) {
            float x = placementNodes[team][i].transform.position.x;
            if (placementNodes[team][i].teamIndex == 1) {
                x = -x;
            }

            if (x < depth) {
                depth = x;
            }
        }

        PlayerController.maxFactoryPlacementX[team] = depth;
    }

	void Update () {
		locIncome += (float)income * Time.deltaTime;
		if (locIncome >= 1 && u.manager) {
			if (giveAllPlayers) {
				for (int i=0;i<u.manager.players;i++) {
					if (u.manager.playerControllers[i].teamIndex == u.teamIndex) {
						u.manager.credits[i] += 1;
						locIncome = 0;
					}
				}
			}else{
				u.manager.credits[u.playerIndex] += 1;
				locIncome = 0;
			}
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
			if (u.targetOverride) {
				newU.targetOverride = u.targetOverride;
			}
		}
	}
}
