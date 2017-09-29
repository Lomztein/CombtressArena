using UnityEngine;
using System.Collections;

public class HealthScript : MonoBehaviour {

	public float health;
	public float maxHealth;
	public bool invincible;
	public float regenMax;
	public float regenSpeed;
	public GameObject debris;
	public string armorType;
	public int value;
	public Unit lastHit;
	public Unit unit;

	public GameObject healthBar;
	public SpriteRenderer fullBar;
	public SpriteRenderer curBar;

	// Use this for initialization
	void Start () {
		unit = GetComponent<Unit>();
		if (maxHealth == 0) {
			maxHealth = health;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (unit) {
			if (unit.manager) {
				value = unit.income * unit.manager.creditsMultiplier;
			}
		}
		if (health <= 0 && invincible == false) {
			Destroy(gameObject);
			if (debris) { Instantiate (debris,transform.position,Quaternion.identity); }
			if (lastHit) {
				if (lastHit.playerIndex >= 0) {
					lastHit.manager.credits[lastHit.playerIndex] += value;
				}
			}
		}
		if (health < regenMax) {
			health += regenSpeed * Time.deltaTime;
		}
	}

    public static int TypeToInt(string type) {
        switch (type) {
            case "light":
                return 0;

            case "medium":
                return 1;

            case "heavy":
                return 2;

            case "all":
                return -1;

            default:
                return -2;
        }
        // I wish I'd known of enums when I first wrote this game.
    }

    public static string IntToType(int type) {
        switch (type) {
            case -1:
                return "all";
            case 0:
                return "light";
            case 1:
                return "medium";
            case 2:
                return "heavy";
            default:
                return "";
        }
    }
}
