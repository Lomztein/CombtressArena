using UnityEngine;
using System.Collections;
[RequireComponent(typeof(HealthScript))]

public class Unit : MonoBehaviour {

	public string unitType;
	public string unitName;
	/*public string unitDisc;
	public string[] unitPros;
	public string[] unitCons;*/
	public string playerName;
	public int teamIndex;
	public int playerIndex;
	public string teamName;

	public float direction;
	public float directionToTarget;
	public float distanceToTarget;
	public Vector3 velocity;
	public Vector3 prevPos;

	public LayerMask freindlyLayer;
	public LayerMask enemyLayer;

	public GameObject weapon;
	public GameObject newWeapon;
	public float weaponRange;
	public WeaponScript weaponScript;
	public HealthScript health;
	public Transform weaponPos;

	public GameObject target;
	public Unit targetUnit;
	public Vector3 targetPos;
	
	public float bulletSpeed;
	public float bDamage = 1;
	public float bRange = 1;
	public float bFirerate = 1;
	public float bBulletSpeed = 1;
	public int income;

	public float experience;
	public float expNeeded;
	public int level = 1;

	public GameObject levelUpParticle;
	public GlobalManager manager;
	public MapManager map;

	// Use this for initialization
	void Start () {
		expNeeded = level * level * 25;
		if (newWeapon) {
			EquipWeapon();
		}
		health = GetComponent<HealthScript>();
		name = unitName + ", level " + level.ToString();
		GameObject stats = GameObject.FindGameObjectWithTag("Stats");
		manager = stats.GetComponent<GlobalManager>();
		map = stats.GetComponent<MapManager>();
		if (unitType == "structure") { transform.FindChild("Sprite").position += Vector3.forward/2; }
		health.value = income * Mathf.RoundToInt (level * 0.25f);
		GetLayers();
	}

	public void GetLayers () {
		if (manager) {
			if (teamIndex == 0) {
				freindlyLayer = manager.team0Layer;
				enemyLayer = manager.team1Layer;
				gameObject.layer = teamIndex + 10;
			}else{
				freindlyLayer = manager.team1Layer;
				enemyLayer = manager.team0Layer;
				gameObject.layer = teamIndex + 10;
			}
		}else{
			Debug.LogWarning (name + " didn't grap the global manager, for whatever reason.",gameObject);
		}
	}

	void EquipWeapon () {
		if (newWeapon) {
			if (!weaponPos) {
				weaponPos = new GameObject("WeaponPos").transform;
				weaponPos.position = transform.position;
			}
			Destroy (weapon);
			weapon = (GameObject)Instantiate(newWeapon,weaponPos.position,transform.rotation);
			weapon.transform.parent = transform;
			weaponScript = weapon.GetComponent<WeaponScript>();
			weaponScript.parent = this;
			newWeapon = null;
		}
	}

	void LevelUp () {
		float excess = experience - expNeeded;
		experience = excess;
		health.maxHealth = health.maxHealth * 1.25f;
		health.health = health.maxHealth;
		if (weapon) {
			bDamage = bDamage * 1.25f;
			bRange = bRange * 1.25f;
			bBulletSpeed = bBulletSpeed * 1.10f;
			bulletSpeed = weaponScript.bulletSpeed * bBulletSpeed;
		}
		level++;
		name = unitName + ", level " + level.ToString();
		if (levelUpParticle) {
			Instantiate(levelUpParticle,transform.position,Quaternion.identity);
		}
		health.value = income * Mathf.RoundToInt (level * 0.25f);
		expNeeded = level * level * 25;
	}
	
	// Update is called once per frame
	void Update () {

		if (experience >= expNeeded) {
			LevelUp ();
		}
		if (newWeapon) {
			EquipWeapon ();
		}
		if (target) {
			//targetPos = CalculateFuturePosition (target.transform.position,targetUnit.velocity,bulletSpeed);
			targetPos = target.transform.position;
			directionToTarget = Mathf.Atan2(targetPos.y-transform.position.y, targetPos.x-transform.position.x)*180 / Mathf.PI;
			distanceToTarget = Vector3.Distance(transform.position,target.transform.position);
		}
		if (weapon) {
			weaponRange = weaponScript.range * bRange;
		}
		direction = transform.rotation.eulerAngles.z;
		velocity = -(prevPos - transform.position)/Time.deltaTime;
		prevPos = transform.position;
	}
	public void Fire () {
		weaponScript.Fire();
	}

	Vector3 CalculateFuturePosition (Vector3 spos, Vector3 vel, float speed) {
		float distance = Vector3.Distance (transform.position,spos);
		float time = distance/speed;
		return spos + vel * time;
	}

	void OnDrawGizmos()  {
		if (target) {
			Gizmos.DrawSphere (target.transform.position,0.25f);
			Gizmos.DrawLine (transform.position,transform.position + velocity);
		}
	}
}
