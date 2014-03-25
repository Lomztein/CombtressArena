using UnityEngine;
using System.Collections;

public class RaycastBullet : MonoBehaviour {

	public float maxRange;
	public float width;
	public Vector3 start;
	public Vector3 end;
	public LineRenderer line;
	public BulletScript bullet;

	// Use this for initialization
	void Start () {
		start = transform.position;
		Ray ray = new Ray(transform.position,bullet.velocity.normalized);
		RaycastHit hit;
		if (Physics.Raycast (ray,out hit,maxRange)) {
			GameObject other = hit.collider.gameObject;
			HealthScript oh = other.GetComponent<HealthScript>();
			end = hit.point;
			line.SetWidth(width,width);
			line.SetPosition (0,start);
			line.SetPosition (1,end);
			if (oh) {
				oh.health -= bullet.damage;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
