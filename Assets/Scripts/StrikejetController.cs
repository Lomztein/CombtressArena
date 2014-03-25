using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Unit))]

public class StrikejetController : MonoBehaviour {

	public int rounds;
	public bool infiniteRounds;
	public float speed;
	public float maxSpeed;
	public float rotateSpeed;
	public float range;
	public GameObject bomb;
	public float reloadTime;
	public bool reloaded;
	public Transform[] muzzles;
	public int muzzleIndex;
	public Unit unit;

	// Use this for initialization
	void Start () {
		unit = GetComponent<Unit>();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (unit.target) {
			if (speed < maxSpeed) {
				speed += maxSpeed * 10 * Time.deltaTime;
			}else{
				speed = maxSpeed;
			}
			Quaternion newDir = Quaternion.identity;
			if (rounds > 0 || infiniteRounds) {
				newDir = Quaternion.Euler(0f,0f,unit.directionToTarget);
			}else{
				if (unit.teamIndex == 0) {
					newDir = Quaternion.identity;
				}else{
					newDir = Quaternion.Euler(0f,0f,180f);
				}
			}
			transform.position += (transform.right * speed * Time.deltaTime);
			transform.rotation = Quaternion.RotateTowards(transform.rotation,newDir,rotateSpeed * Time.deltaTime);
			if (unit.distanceToTarget < range) {
				if (reloaded == true && (rounds > 0 || infiniteRounds)) {
					reloaded = false;
					Invoke("Reload",reloadTime);
					rounds--;
					GameObject newBomb = (GameObject)Instantiate(bomb,muzzles[muzzleIndex].position,muzzles[muzzleIndex].rotation);
					muzzleIndex++;
					muzzleIndex = muzzleIndex % muzzles.Length;
					AirstrikeBombScript sbomb = newBomb.GetComponent<AirstrikeBombScript>();
					sbomb.target = unit.target.transform;
					sbomb.layer = unit.enemyLayer;
					sbomb.parentChar = unit;
					sbomb.range = unit.weaponRange/2;
				}
			}
		}

		if (unit.sprite.isVisible == false && rounds <= 0 && infiniteRounds == false) {
			Destroy (gameObject);
		}
	}

	void Reload () {
		reloaded = true;
	}
}
