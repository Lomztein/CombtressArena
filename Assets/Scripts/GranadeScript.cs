using UnityEngine;
using System.Collections;

public class GranadeScript : MonoBehaviour {

	public BulletScript bullet;

	public float range;
	public int minFragments;
	public int maxFragments;

	public GameObject explosionParticle;

	// Use this for initialization
	void Start () {
		bullet = GetComponent<BulletScript>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Physics.CheckSphere(transform.position,range,bullet.layer)) {
			if (explosionParticle) {
				Instantiate(explosionParticle,transform.position,transform.rotation);
			}
			Destroy (gameObject);
			int frags = Random.Range (minFragments,maxFragments);
			for (int i=0;i<frags;i++) {
				Vector2 newDir = new Vector2(Random.value-0.5f,Random.value-0.5f).normalized;
				Ray ray = new Ray(transform.position,newDir);
				RaycastHit hit;
				if (Physics.Raycast (ray,out hit,range*2)) {
					HealthScript oh = hit.collider.GetComponent<HealthScript>();
					if (oh) {
						Quaternion qDir = Quaternion.Euler(0,0,Mathf.Atan2(hit.point.y-transform.position.y, hit.point.x-transform.position.x)*180 / Mathf.PI);
						bullet.Hit (hit.collider,hit.point,qDir);
					}
				}
			}
		}
	}
}