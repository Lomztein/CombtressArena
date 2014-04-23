using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour {

	public GameObject bulletType;
	public GameObject fireParticle;
	public float fireParticleSize;
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
	public float fireSequence;
	public bool ignoreDirection;
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

	void FeedBulletData (GameObject bullet) {
		Vector3 force = (muzzles[muzzleIndex].right * bulletSpeed * parent.bBulletSpeed * (Random.Range (90f,110f)/100f));
		force += (muzzles[muzzleIndex].up * (Random.Range (-inaccuracy,inaccuracy)));
		BulletScript bs = bullet.GetComponent<BulletScript>();
		if (bs) {
			bs.velocity = force;
			bs.damage = damage * parent.bDamage;
			bs.parentChar = parent;
			bs.range = range * parent.bRange;
			bs.layer = parent.enemyLayer;
			if (parent.target) { bs.target = parent.target.transform; }
		}
		BombScript bomb = bullet.GetComponent<BombScript>();
		if (bomb) {
			bomb.unit = parent;
		}
	}

	public bool Fire () {
		bool hasFired = false;
		if ((Mathf.Abs (Mathf.DeltaAngle (parent.directionToTarget,transform.rotation.eulerAngles.z)) < 10) || ignoreDirection) {
			if (reloaded == true) {
				reloaded = false;
				Invoke("Reload",reloadTime * parent.bFirerate);
				if (fireParticle) {
					Instantiate(fireParticle,muzzles[muzzleIndex].position + Vector3.back,muzzles[muzzleIndex].rotation);
				}
				for (int i=0;i<amount;i++) {
					bullet = (GameObject)Instantiate(bulletType,muzzles[muzzleIndex].position,muzzles[muzzleIndex].rotation);
					FeedBulletData (bullet);
					hasFired = true;
				}
				muzzleIndex++;
				if (muzzleIndex < muzzles.Length) {
					Invoke ("FireSequence",fireSequence);
				}
				muzzleIndex = muzzleIndex % muzzles.Length;
			}
		}
		return hasFired;
	}

	void FireSequence () {
		if (fireParticle) {
			Instantiate(fireParticle,muzzles[muzzleIndex].position + Vector3.back,muzzles[muzzleIndex].rotation);
		}
		for (int i=0;i<amount;i++) {
			bullet = (GameObject)Instantiate(bulletType,muzzles[muzzleIndex].position,muzzles[muzzleIndex].rotation);
			FeedBulletData (bullet);
		}
		muzzleIndex++;
		if (muzzleIndex < muzzles.Length) {
			Invoke ("FireSequence",fireSequence);
		}
		muzzleIndex = muzzleIndex % muzzles.Length;
	}

	void Reload () {
		reloaded = true;
	}
}
