using UnityEngine;
using System.Collections;

public class MineController : MonoBehaviour {

	public float range;
	public float explosionRange;
	public float damage;
	public Unit unit;
	public LayerMask layer;
	public GameObject explosionParticle;

	void Update () {

		if (Physics.CheckSphere(transform.position,range,layer)) {
			Collider[] nearby = Physics.OverlapSphere(transform.position,explosionRange,layer);
			for (int i=0;i<nearby.Length;i++) {
				HealthScript oh = nearby[i].GetComponent<HealthScript>();
				if (oh) {
					oh.health -= damage;
					if (unit) { unit.experience += damage/5; }
					if (explosionParticle) { Instantiate(explosionParticle,transform.position,Quaternion.identity); }
					Destroy (gameObject);
				}
			}
		}
	}
}