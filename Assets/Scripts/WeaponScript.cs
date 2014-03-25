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
	public Transform muzzle;
	GameObject bullet;

	void Start () {
		parent.bulletSpeed = bulletSpeed;
		if (!muzzle) {
			muzzle = new GameObject("Muzzle").transform;
			muzzle.position = transform.position;
			muzzle.rotation = transform.rotation;
			muzzle.parent = transform;
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
			transform.rotation = Quaternion.RotateTowards(transform.rotation,dq,turnSpeed*Time.deltaTime);
		}else{
			dq = Quaternion.Euler(new Vector3(0,0,parent.direction));
			transform.rotation = Quaternion.RotateTowards(transform.rotation,dq,turnSpeed*Time.deltaTime);
		}
		if (turnSpeed == 0) {
			transform.rotation = parent.transform.rotation;
		}
	}

	public void Fire () {
		if (reloaded == true) {
			reloaded = false;
			Invoke("Reload",reloadTime * parent.bFirerate);
			for (int i=0;i<amount;i++) {
				bullet = (GameObject)Instantiate(bulletType,muzzle.position,muzzle.rotation);
				Vector3 force = (muzzle.right * bulletSpeed * parent.bBulletSpeed * (Random.Range (90f,110f)/100f));
				force += (muzzle.up * (Random.Range (-inaccuracy,inaccuracy)));
				BulletScript bs = bullet.GetComponent<BulletScript>();
				bs.velocity = force;
				bs.damage = damage * parent.bDamage;
				bs.parentChar = parent;
				bs.range = range * parent.bRange;
				bs.layer = parent.enemyLayer;
			}
		}
	}

	void Reload () {
		reloaded = true;
	}
}
