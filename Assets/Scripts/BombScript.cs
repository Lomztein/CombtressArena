using UnityEngine;
using System.Collections;

public class BombScript : MonoBehaviour {

	public float range;
	public float damage;
	public Unit unit;
	public LayerMask layer;
	public float fallSpeed;
	public GameObject explosionParticle;

	// Use this for initialization
	void Start () {
		if (unit) {
			layer = unit.enemyLayer;
		}else{
			Debug.LogWarning("Something fucked up, check it out",gameObject);
		}
	}
	
	void FixedUpdate () {
		transform.position += (Vector3.forward + transform.right * 0.3f) * fallSpeed * Time.fixedDeltaTime;
		if (transform.position.z >= 0) {
			Explode();
		}
	}

	void Explode () {
		Collider[] nearby = Physics.OverlapSphere(transform.position,range,layer);
		Instantiate(explosionParticle,transform.position,Quaternion.identity);
		Destroy(gameObject);
		for (int i=0;i<nearby.Length;i++) {
			HealthScript oh = nearby[i].GetComponent<HealthScript>();
			if (oh) {
				oh.health -= damage;
			}
		}
	}
}
