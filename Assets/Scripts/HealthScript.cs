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

	// Use this for initialization
	void Start () {
		if (maxHealth == 0) {
			maxHealth = health;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (health <= 0 && invincible == false) {
			Destroy(gameObject);
			if (lastHit) {
				lastHit.manager.credits[lastHit.playerIndex] += value;
			}
		}
		if (health < regenMax) {
			health += regenSpeed * Time.deltaTime;
		}
	}
}
