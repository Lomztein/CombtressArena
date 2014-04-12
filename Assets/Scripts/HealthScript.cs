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
}
