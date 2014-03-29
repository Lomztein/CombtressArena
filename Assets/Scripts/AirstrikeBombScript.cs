using UnityEngine;
using System.Collections;

public class AirstrikeBombScript : MonoBehaviour {

	public bool homing;
	public float speed;
	public Transform target;
	public Vector3 targetPos;
	public Transform pointer;
	public float damage;
	public LayerMask layer;
	public float range;
	public float time;
	public string damageType;
	public Unit parentChar;
	public float fallSpeed;

	void Start () {
		if (target) {
			targetPos = target.position;
		}else{
			targetPos = transform.position + transform.right;
		}
		time = range/speed;
		if (Mathf.Round (transform.position.z) == Mathf.Round (targetPos.z)) {
			fallSpeed = 0;
		}else{
			fallSpeed = (transform.position.z - targetPos.z) / Vector3.Distance (transform.position,targetPos);
		}
	}
	
	void FixedUpdate () {
		Ray ray = new Ray(transform.position,transform.right + -(transform.forward * fallSpeed));
		RaycastHit hit;
		if (target) {
			targetPos = target.position;
		}else{
			targetPos = transform.position + transform.right;
		}
		if (homing) {
			float angle = Mathf.Atan2(targetPos.y-transform.position.y, targetPos.x-transform.position.x)*180 / Mathf.PI;
			Quaternion newDir = Quaternion.Euler(0f,0f,angle);
			transform.rotation = Quaternion.RotateTowards (transform.rotation,newDir,speed * 10 * Time.deltaTime);
		}
		Vector3 targetZ = new Vector3 (transform.position.x,transform.position.y,targetPos.z);
		transform.position = Vector3.MoveTowards(transform.position,targetZ,speed * Time.deltaTime);
		if (Physics.Raycast (ray, out hit, speed * Time.fixedDeltaTime,layer)) {
			Hit(hit.collider);
		}
		transform.position += (transform.right * speed * Time.fixedDeltaTime);
		if (time > 0) {
			time -= Time.fixedDeltaTime;
		}else{
			Destroy (gameObject);
		}
	}
	
	void Hit (Collider other) {
		HealthScript healthScript = other.gameObject.GetComponent<HealthScript>();
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
		Gizmos.DrawRay (new Ray(transform.position,(transform.right + ((-transform.forward) * fallSpeed)) * speed * Time.fixedDeltaTime));
	}
}
