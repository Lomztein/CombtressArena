using UnityEngine;
using System.Collections;

public class BoostWaveScript : MonoBehaviour {

	public float startSize = 0.1f;
	public float endSize = 5;
	public float time;
	public float boostSpeed;
	float growSpeed;
	SpriteRenderer sprite;
	public Unit parentUnit;

	public bool heal;
	public bool repair;

	// Use this for initialization
	void Start () {
		transform.localScale = Vector3.one * startSize;
		sprite = GetComponentInChildren<SpriteRenderer>();
		if (heal) { sprite.color = Color.red; }
		if (repair) { sprite.color = Color.blue; }
		growSpeed = (endSize-startSize)/time;
	}
	
	void Update () {
		Color sc = sprite.material.color;
		sprite.color -= new Color (0,0,0,1/time) * Time.deltaTime;
		transform.localScale += Vector3.one * growSpeed * Time.deltaTime;
		if (transform.localScale.x > endSize) {
			Destroy(gameObject);
		}
	}

	void Heal (Unit ou) {
		if (ou.health.health < ou.health.maxHealth) {
			ou.health.health += boostSpeed * Time.deltaTime;
			parentUnit.experience += boostSpeed/5*Time.deltaTime;
		}else{
			ou.health.health = ou.health.maxHealth;
		}
	}

	void OnTriggerStay ( Collider other ) {
		Unit ou = other.GetComponent<Unit>();
		if (ou) {
			if (heal && ou.unitType == "infantry") {
				Heal (ou);
			}
			if (repair && (ou.unitType == "vehicle" || ou.unitType == "structure")) {
				Heal (ou);
			}
		}
	}
}
