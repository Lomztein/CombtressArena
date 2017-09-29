using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProducingStructure : MonoBehaviour, IBalanceItem {

	public float time;
	public GameObject unit;
	public float size;
	public float income;
	public bool giveAllPlayers;
	public bool overrideTime;
	public Unit u;
	float locIncome;

    public HealthScript spawnHealth;

    public static List<Unit>[] placementNodes;
    public static Dictionary<int, List<ProducingStructure>> teamProducingStructures = new Dictionary<int, List<ProducingStructure>> ();

	// Use this for initialization
	void Start () {
        u = GetComponent<Unit>();
        BotInput.allBalanceItems[u.teamIndex].Add (this);
		if (unit) {
            spawnHealth = unit.GetComponent<HealthScript>();
        }
        placementNodes[u.teamIndex].Add (u);

        if (!teamProducingStructures.ContainsKey (u.teamIndex)) {
            teamProducingStructures.Add (u.teamIndex, new List<ProducingStructure> ());
        }
        teamProducingStructures [ u.teamIndex ].Add (this);
	}

    void OnDestroy () {
        placementNodes[u.teamIndex].Remove (u);
        teamProducingStructures [ u.teamIndex ].Remove (this);
        BotInput.allBalanceItems [ u.teamIndex ].Remove (this);
    }

    public static void InitPlacementNodesList () {
        if (placementNodes == null) {
            placementNodes = new List<Unit>[2];
            for (int i = 0; i < placementNodes.Length; i++) {
                placementNodes[i] = new List<Unit> ();
            }
        }
    }

    public static void SpawnOfType(string type) {
        SpawnOfType (0, type);
        SpawnOfType (1, type);
    }

    public static void SpawnOfType(int team, string type) {
        foreach (ProducingStructure str in teamProducingStructures [ team ]) {
            if (str.spawnHealth && str.spawnHealth.armorType == type) {
                str.CreateUnit ();
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
    public void CreateUnit() {
		if (unit) {
            for (int i = 0; i < unit.GetComponent<Unit> ().spawnAmount; i++) {
                Vector3 ranPos = Random.onUnitSphere * size;
                    Vector3 newPos = new Vector3 (ranPos.x, ranPos.y, 0) + transform.position;
                Vector3 newDir = new Vector3 (0, 0, 0);
                if (u.teamIndex == 0) {
                    newDir = new Vector3 (0, 0, 180);
                }

			    GameObject newUnit = Instantiate (unit, newPos, Quaternion.Euler (newDir));
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

    public float GetDamageValue() {
        if (unit) {
            Unit locUnit = unit.GetComponent<Unit> ();
            if (!locUnit.newWeapon)
                return 0f;
            else
                return locUnit.newWeapon.GetComponent<WeaponScript> ().GetDPS () * locUnit.spawnAmount;
        } else {
            return 0f;
        }
    }

    public float GetHealthValue() {
        if (unit) {
            Unit locUnit = unit.GetComponent<Unit> ();
            return locUnit.GetComponent<HealthScript>().maxHealth * locUnit.spawnAmount;
        } else {
            return u.health.maxHealth;
        }
    }

    public string GetDamageType() {
        if (unit) {
            Unit locUnit = unit.GetComponent<Unit> ();
            if (!locUnit.newWeapon)
                return "";
            else
                return locUnit.newWeapon.GetComponent<WeaponScript> ().bulletType.GetComponent<BulletScript>().damageType;
        } else {
            return "";
        }
    }

    public string GetHealthType() {
        if (unit) {
            Unit locUnit = unit.GetComponent<Unit> ();
            return locUnit.GetComponent<HealthScript>().armorType;
        } else {
            return u.health.armorType;
        }
    }
}
