using UnityEngine;
using System.Collections;

public class ShieldGeneratorController : MonoBehaviour {

	public GameObject shield;
	public GameObject activeShield;
	public HealthScript shieldHealth;
	public float spawnSpeed;
	public Unit unit;
	public SpriteRenderer shieldSprite;
	public float turnSpeed = 20;

	// Use this for initialization
	void Start () {
		unit = GetComponent<Unit>();
		SpawnShield();
	}
	
	// Update is called once per frame
	void Update () {
		if (activeShield == null && IsInvoking("SpawnShield") == false) {
			Invoke ("SpawnShield",spawnSpeed * unit.bFirerate);
		}
		if (activeShield) {
			float sHealthFactor = shieldHealth.health/100;
			shieldSprite.color = new Color (shieldSprite.color.r,shieldSprite.color.g,shieldSprite.color.b,sHealthFactor);
			unit.sprite.transform.Rotate (0,0,turnSpeed * sHealthFactor * Time.deltaTime);	
		}
	}

	void SpawnShield () {
		activeShield = (GameObject)Instantiate(shield,transform.position,Quaternion.identity);
		shieldHealth = activeShield.GetComponent<HealthScript>();
		shieldSprite = activeShield.GetComponentInChildren<SpriteRenderer>();
		shieldHealth.maxHealth *= unit.bDamage;
		shieldHealth.regenSpeed *= unit.bFirerate;
		activeShield.layer = unit.gameObject.layer;
		activeShield.tag = "Shield";
		activeShield.transform.position += Vector3.back * 2f;
	}
}
