using UnityEngine;
using System.Collections;
[RequireComponent(typeof(LineRenderer))]

public class LaserBullet : MonoBehaviour {

	BulletScript bullet;
	LineRenderer line;

	public float width;
	public Vector3 start;
	public Vector3 end;
	public float time;

	// Use this for initialization
	void Start () {
		bullet = GetComponent<BulletScript>();
		line = GetComponent<LineRenderer>();
		start = transform.position;
		Destroy(gameObject,time);
		Ray ray = new Ray(transform.position,bullet.velocity.normalized * bullet.range * bullet.parentChar.bRange);
		RaycastHit hit;
		if (Physics.Raycast (ray,out hit,bullet.range*bullet.parentChar.bRange,bullet.layer)) {
			end = hit.point;
			bullet.Hit (hit.collider,hit.point,transform.rotation);
		}else{
			Vector3 nEnd = ray.GetPoint(bullet.range * bullet.parentChar.bRange);
			if (bullet.hitParticle) {
				Instantiate(bullet.hitParticle,nEnd,transform.rotation);
			}
			end = nEnd;
		}
		line.SetWidth(width,width);
		line.SetPosition(0,start);
		line.SetPosition(1,end);
	}
}
