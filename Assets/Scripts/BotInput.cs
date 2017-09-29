using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class BotInput : MonoBehaviour {

    public GlobalManager manager;
    public PlayerController input;
    public bool placing;
    public string aiType;
    public Vector3 lastPlace;

    public float credits;
    public GameObject refinary;

    public static Dictionary<int, List<IBalanceItem>> allBalanceItems = new Dictionary<int, List<IBalanceItem>> ();

    private static float [ , ] teamDamageBalance = new float [ 2, 3 ];
    private static float [ , ] teamArmorBalance = new float [ 2, 3 ];

    //public float enemyThreat;
    //public bool attacking;
    //public bool defending;

    /* AI Types
	 * balanced - purchases random units within credits range
	 * rusher - lot of cheap units
	 * steamroller - purchases high-end units in large groups.
	 * defender - focus on defending alone
	 * attacker - focus on attacking alone
     * techie - sacrifices early rushes for early higher end units.
     * areal - prefers to stick to air units if possible.
	*/

    void Start() {
        GameObject stats = GameObject.FindGameObjectWithTag ("Stats");
        manager = stats.GetComponent<GlobalManager> ();
        for (int i = 0; i < manager.purchaseables.Length; i++) {
            if (manager.purchaseables [ i ].name == "Refinary") {
                refinary = manager.purchaseables [ i ];
            }
        }
    }

    void Update() {
        credits = manager.credits [ input.id ];
    }

    GameObject PickUnit() {
        string armorType;
        List<GameObject> counteringUnits = GetCounteringUnits (out armorType);

        GameObject locUnit = null;
        if (aiType == "balanced") {
            locUnit = FindStrongestUnit (counteringUnits, armorType, new Func<Unit, HealthScript, WeaponScript, float> ((unit, hs, ws) => 1f));

        }
        if (aiType == "defender") {
            if (input.population < 1) {
                locUnit = refinary;
            } else {
                locUnit = FindStrongestUnit (counteringUnits, armorType, new Func<Unit, HealthScript, WeaponScript, float> ((unit, hs, ws) => 1f), new Predicate<Unit> (x => x.unitType == "structure"));
            }
        }
        if (aiType == "attacker") {
            locUnit = FindStrongestUnit (counteringUnits, armorType, new Func<Unit, HealthScript, WeaponScript, float> ((unit, hs, ws) => 1f), new Predicate<Unit> (x => x.unitType != "structure"));
        }
        if (aiType == "steamroller") {
            if (UnityEngine.Random.Range (0, input.population * 2) == 0) {
                locUnit = refinary;
            } else {
                locUnit = FindStrongestUnit (counteringUnits, armorType, new Func<Unit, HealthScript, WeaponScript, float> ((unit, hs, ws) => ws.GetDPS () * unit.spawnAmount + hs.maxHealth));
            }
        }
        if (aiType == "ranger") {
            locUnit = FindStrongestUnit (counteringUnits, armorType, new Func<Unit, HealthScript, WeaponScript, float> ((unit, hs, ws) => ws.range), new Predicate<Unit> (x => x.unitType != "structure"));
        }
        if (aiType == "tanker") {
            locUnit = FindStrongestUnit (counteringUnits, armorType, new Func<Unit, HealthScript, WeaponScript, float> ((unit, hs, ws) => hs.health * unit.spawnAmount));
        }
        if (aiType == "rusher") {
            locUnit = FindStrongestUnit (counteringUnits, armorType, new Func<Unit, HealthScript, WeaponScript, float> ((unit, hs, ws) => -unit.cost / unit.spawnAmount), new Predicate<Unit> (x => (x.cost / x.spawnAmount) > input.population * 5f));
        }
        if (aiType == "techie") {
            locUnit = FindStrongestUnit (counteringUnits, armorType, new Func<Unit, HealthScript, WeaponScript, float> ((unit, hs, ws) => unit.cost + (hs.armorType == "light" ? 1f : hs.armorType == "medium" ? 2f : 6f)));
        }
        if (aiType == "areal") {
            locUnit = FindStrongestUnit (counteringUnits, armorType, new Func<Unit, HealthScript, WeaponScript, float> ((unit, hs, ws) => unit.height));
        }
        return locUnit;
    }

    private List<GameObject> GetCounteringUnits(out string ignoredArmor) {
        int enemyTeam = input.teamIndex == 0 ? 1 : 0;
        // To figure out what kind of damage to purchase, check the balance between enemy armor types and friendly damage types.
        // Whichever of these balances are lowest is the kind of damage to purchase.
        // To figure out which kind of armor to purchase, attempt to do the opposite. Possibly in a single loop for performance.

        float [ ] damageBalance = new float [ 3 ];// Balance between team damage and enemy team armor
        float [ ] armorBalance = new float [ 3 ]; // Balance between team armor and enemy team damage

        ModifyBalance (allBalanceItems [ enemyTeam ], ref damageBalance, ref armorBalance, 1);
        ModifyBalance (allBalanceItems [ input.teamIndex ], ref damageBalance, ref armorBalance, -1);

        for (int i = 0; i < damageBalance.Length; i++) {
            teamDamageBalance [ input.teamIndex, i ] = damageBalance [ i ];
            teamArmorBalance [ input.teamIndex, i ] = armorBalance [ i ];
        }

        string preferredDamage = HealthScript.IntToType (GetHighest (damageBalance));
        ignoredArmor = HealthScript.IntToType (GetHighest (armorBalance));

        List<GameObject> applicableUnits = GlobalManager.manager.purchaseables.Where (x => {
            Unit unit = x.GetComponent<Unit> ();
            if (unit.newWeapon) {
                BulletScript bulletScript = unit.newWeapon.GetComponent<WeaponScript> ().bulletType.GetComponent<BulletScript> ();
                return preferredDamage == "all" || bulletScript.damageType == preferredDamage || bulletScript.damageType == "all";
            }
            return false;
        }).ToList ();
        if (applicableUnits.Count == 0)
            return GlobalManager.manager.purchaseables.ToList ();

        return applicableUnits;
    }

    private int GetHighest(float [ ] values) {
        int highest = -1;
        float highestValue = 0f;
        for (int i = 0; i < values.Length; i++) {
            if (values [ i ] > highestValue) {
                highestValue = values [ i ];
                highest = i;
            }
        }
        return highest;
    }

    private void ModifyBalance(List<IBalanceItem> items, ref float [ ] damageBalance, ref float [ ] armorBalance, int direction) {
        foreach (IBalanceItem item in items) {

            int armorType = HealthScript.TypeToInt (item.GetHealthType ());
            int damageType = (HealthScript.TypeToInt (item.GetDamageType ()));

            if (armorType == -1) {
                damageBalance.Select (x => x += direction * item.GetHealthValue ());
            } else if (armorType != -2) {
                damageBalance [ armorType ] += direction * item.GetHealthValue ();
            }

            if (damageType == -1) {
                armorBalance.Select (x => x += direction * item.GetDamageValue ());
            } else if (damageType != -2) {
                armorBalance [ damageType ] += direction * item.GetDamageValue ();
            }
        }
    }

    public static string FormatTeamBalance(int team) {
        string toReturn = "";
        int length = teamDamageBalance.GetLength (1);
        for (int i = 0; i < length; i++) {
            string typeName = HealthScript.IntToType (i).ToUpper ();
            toReturn += typeName + " DAMAGE - " + teamDamageBalance [team, i ] + " : " + typeName + " ARMOR - " + teamArmorBalance[team, i] + ", ";
        }
        return toReturn;
    }

    GameObject FindStrongestUnit (List<GameObject> units, string armorType, Func<Unit, HealthScript, WeaponScript, float> prioritizer, Predicate<Unit> filter = null, bool reverseSearch = false) {
        
		GameObject strongest = null;
		float value = float.MinValue;
		for (int i=0;i<units.Count;i++) {
			Unit u = units[i].GetComponent<Unit>();
			if (credits >= u.cost) {
                bool allow = filter == null ? true : filter.Invoke (u);
				if (u.newWeapon && allow) {
			        HealthScript hs = u.GetComponent<HealthScript>();
                    WeaponScript ws = u.newWeapon.GetComponent<WeaponScript> ();
                    float globalModifiers = UnityEngine.Random.Range (0.7f, 1.5f) * (hs.armorType != armorType ? 100f : 1f);
                    float newValue = globalModifiers * prioritizer.Invoke (u, hs, ws);

                    if (newValue > value) {
                        value = newValue;
                        strongest = units [ i ];
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
		if (aiType == "rusher") {
			if (credits > Mathf.Max (100,input.population*10)) {
				create = true;
			}
		}
		if (aiType == "steamroller" || aiType == "techie") {
			if (credits >= Mathf.Max (500,input.population * 100)) {
				create = true;
			}
		}

        if (create != true && UnityEngine.Random.Range (0, 100 + input.population * 10) == 1) {
            if (credits > Mathf.Max (100, input.population * 75)) {
                create = true;
            }
        }

        return create;
	}

	Unit FindWeakestStructure (Unit tryingUnit) {
	    List<GameObject> units = UnitManager.units[input.teamIndex];
		
		GameObject weakest = null;
		int cheapestCost = tryingUnit.cost;
		if (tryingUnit.cost <= credits) {
			for (int i=0;i<units.Count;i++) {
				if (units[i]) {
					Unit locUnit = units[i].GetComponent<Unit>();
					if (locUnit.unitType == "structure" && locUnit.playerIndex == input.id) {
						if (tryingUnit.cost / tryingUnit.spawnAmount > locUnit.cost / locUnit.spawnAmount) {
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
            if (newUnit) {
                input.selectedPurchaseOption = newUnit;
                GameObject newF = input.freindlyFortresses [ UnityEngine.Random.Range (0, input.freindlyFortresses.Length) ];
                if (newF) {
                    input.focusPoint = newF.transform.position;
                    placing = true;
                }
            } else
                placing = false;
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
                GameObject newF = input.freindlyFortresses [ UnityEngine.Random.Range (0, input.freindlyFortresses.Length) ];

                if (newF) {
					input.focusPoint = newF.transform.position;
				}
			}
            if (value == 3) {
                Vector3 openNearby = Vector3.zero;
                ProducingStructure withOpen = ProducingStructure.teamProducingStructures [ input.teamIndex ].FirstOrDefault (x => {
                    for (int i = 0; i < manager.nearbyVectors.Length; i++) {
                        openNearby = manager.nearbyVectors [ i ] * 2.5f;
                        Debug.DrawLine (x.transform.position, x.transform.position + openNearby, Color.white, 1f);
                        if (input.CanPlace (x.transform.position + openNearby) && manager.IsInsideBattlefield (x.transform.position + openNearby)) {
                            return true;
                        }
                    }
                    return false;
                });

                if (withOpen && UnityEngine.Random.Range (0, 4) != 3) {
                    input.focusPoint = withOpen.transform.position + openNearby;
                } else {
                    input.focusPoint += manager.nearbyVectors [ UnityEngine.Random.Range (0, manager.nearbyVectors.Length) ];
                }
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
 