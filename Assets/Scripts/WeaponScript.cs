using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour {

	public GameObject bulletType;
	public float bulletSpeed;
	public float reloadTime;
	public bool reloaded;
	public float damage;
	public float inaccuracy;
	public int amount = 1;
	public Unit parent;
	public float range;
	public float turnSpeed;
	public Transform[] muzzles;
	public int muzzleIndex;
	GameObject bullet;

	void Start () {
		parent.bulletSpeed = bulletSpeed;
		if (muzzles.Length == 0) {
			muzzles = new Transform[1];
			muzzles[0] = new GameObject("Muzzle").transform;
			muzzles[0].position = transform.position;
			muzzles[0].rotation = transform.rotation;
			muzzles[0].parent = transform;
		}
		Transform sprite = transform.FindChild ("Sprite");
		if (sprite) {
			sprite.position += Vector3.back;
		}
	}

	void Update () {
		Quaternion dq = Quaternion.identity;
		if (parent.target) {
			dq = Quaternion.Euler(new Vector3(0,0,parent.directionToTarget));
		}else{
			dq = Quaternion.Euler(new Vector3(0,0,parent.direction));
		}
		transform.rotation = Quaternion.RotateTowards(transform.rotation,dq,turnSpeed*Time.deltaTime);
		if (turnSpeed == 0) {
			transform.rotation = parent.transform.rotation;
		}
	}

	public bool Fire () {
		bool hasFired = false;
		if (Mathf.Abs (Mathf.DeltaAngle (parent.directionToTarget,transform.rotation.eulerAngles.z)) < 10) {
			if (reloaded == true) {
				reloaded = false;
				Invoke("Reload",reloadTime * parent.bFirerate);
				for (int i=0;i<amount;i++) {
					bullet = (GameObject)Instantiate(bulletType,muzzles[muzzleIndex].position,muzzles[muzzleIndex].rotation);
					Vector3 force = (muzzles[muzzleIndex].right * bulletSpeed * parent.bBulletSpeed * (Random.Range (90f,110f)/100f));
					force += (muzzles[muzzleIndex].up * (Random.Range (-inaccuracy,inaccuracy)));
					BulletScript bs = bullet.GetComponent<BulletScript>();
					hasFired = true;
					muzzleIndex++;
					muzzleIndex = muzzleIndex % muzzles.Length;
					bs.velocity = force;
					bs.damage = damage * parent.bDamage;
					bs.parentChar = parent;
					bs.range = range * parent.bRange;
					bs.layer = parent.enemyLayer;
					bs.target = parent.target.transform;
				}
			}
		}
		return hasFired;
	}

	void Reload () {
		reloaded = true;
	}
}
