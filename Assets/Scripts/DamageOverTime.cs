using UnityEngine;
using System.Collections;

public class DamageOverTime : MonoBehaviour {

	public float damage;
	public float time;
	HealthScript health;
	public string damageType;

	// Use this for initialization
	void Start () {
		health = transform.parent.GetComponent<HealthScript>();
		Destroy(this,5);
	}
	
	// Update is called once per frame
	void Update () {
		if (health) {
			if (health.armorType == damageType) {
				health.health -= damage * Time.deltaTime;
			}else{
				health.health -= damage/5 * Time.deltaTime;
			}
		}
	}
}
