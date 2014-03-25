using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

	public float damage;
	public Vector3 velocity;
	public Unit parentChar;
	public float range;
	public float time;
	public string damageType;
	public LayerMask layer;
	SpriteRenderer sprite;
	HealthScript healthScript;

	void Start () {
		sprite = transform.FindChild ("Sprite").GetComponent<SpriteRenderer>();
		sprite.transform.localScale = new Vector3 (0.45f,velocity.magnitude * 2 * Time.fixedDeltaTime,1);
		sprite.transform.localPosition += new Vector3(sprite.bounds.extents.x,0,0);
		time = range/velocity.magnitude;
		Ray ray = new Ray(transform.position,velocity);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, velocity.magnitude * Time.fixedDeltaTime)) {
			Hit(hit.collider);
		}
	}

	void FixedUpdate () {
		Ray ray = new Ray(transform.position,velocity);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, velocity.magnitude * Time.fixedDeltaTime,layer)) {
			Hit(hit.collider);
		}
		transform.position += (velocity * Time.fixedDeltaTime);
		if (time > 0) {
			time -= Time.fixedDeltaTime;
		}else{
			Destroy (gameObject);
		}
	}

	void Hit (Collider other) {
		healthScript = other.gameObject.GetComponent<HealthScript>();
		if (healthScript) {
			Destroy(gameObject);
			if (healthScript.armorType == damageType) {
				healthScript.health -= damage;
				parentChar.experience += damage/5;
			}else{
				healthScript.health -= damage/5;
				parentChar.experience += damage/25;
			}
			healthScript.lastHit = parentChar;
		}
	}

	void OnDrawGizmos () {
		Gizmos.DrawLine (transform.position, transform.position + velocity * Time.fixedDeltaTime);
	}
}