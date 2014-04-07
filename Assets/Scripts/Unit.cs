using UnityEngine;
using System.Collections;
[RequireComponent(typeof(HealthScript))]

public class Unit : MonoBehaviour {

	public string unitType;
	public string unitName;
	public string unitDisc;
	public string[] unitPros;
	public string[] unitCons;
	public string playerName;
	public int teamIndex;
	public int playerIndex;
	public string teamName;

	public float height;
	public float direction;
	public float directionToTarget;
	public float distanceToTarget;
	public Vector3 velocity;
	public Vector3 prevPos;
	public bool selected;

	public LayerMask freindlyLayer;
	public LayerMask enemyLayer;

	public GameObject weapon;
	public GameObject newWeapon;
	public float weaponRange;
	public WeaponScript weaponScript;
	public HealthScript health;
	public Transform weaponPos;

	public GameObject target;
	public GameObject targetOverride;
	public Unit targetUnit;
	public Vector3 targetPos;
	public Vector3 targetVel;
	
	public float bulletSpeed;
	public float bDamage = 1;
	public float bRange = 1;
	public float bFirerate = 1;
	public float bBulletSpeed = 1;

	public int income;
	public int cost;
	public int spawnAmount;

	public float experience;
	public float expNeeded;
	public int level = 1;

	public GameObject levelUpParticle;
	public GlobalManager manager;
	public MapManager map;
	public SpriteRenderer sprite;

	public Transform selectedSprite;
	LineRenderer line;

	// Use this for initialization
	void Start () {
		expNeeded = level * level * 25;
		if (newWeapon) {
			EquipWeapon();
		}
		if (newWeapon) {
			EquipWeapon ();
		}
		transform.position += -transform.forward * height;
		health = GetComponent<HealthScript>();
		name = unitName + ", level " + level.ToString();
		GameObject stats = GameObject.FindGameObjectWithTag("Stats");
		manager = stats.GetComponent<GlobalManager>();
		map = stats.GetComponent<MapManager>();
		Transform sp = transform.FindChild("Sprite");
		if (sp) {
			sprite = sp.gameObject.GetComponent<SpriteRenderer>();
			if (unitType == "structure") { sprite.transform.position += Vector3.forward/2; }
		}
		if (tag != "Fortress") {
			if (teamIndex == 0) {
				tag = "Team0";
			}else{
				tag = "Team1";
			}
		}
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
				weaponPos.parent = transform;
				weaponPos.position = transform.position;
			}
			Destroy (weapon);
			weapon = (GameObject)Instantiate(newWeapon,weaponPos.position,transform.rotation);
			weapon.transform.parent = transform;
			weaponScript = weapon.GetComponent<WeaponScript>();
			weaponScript.parent = this;
			newWeapon = null;
			bulletSpeed = weaponScript.bulletSpeed * bBulletSpeed;
		}
	}

	void LevelUp () {
		float excess = experience - expNeeded;
		experience = excess;
		health.maxHealth = health.maxHealth * 1.01f;
		health.health = health.maxHealth;
		if (weapon) {
			bDamage = bDamage * 1.01f;
			bRange = bRange * 1.01f;
			bBulletSpeed = bBulletSpeed * 1.005f;
			bulletSpeed = weaponScript.bulletSpeed * bBulletSpeed;
		}
		level++;
		name = unitName + ", level " + level.ToString();
		if (levelUpParticle) {
			Instantiate(levelUpParticle,transform.position,Quaternion.identity);
		}
		expNeeded = level * level * 25;
	}

	void OnMouseDown () {
		if (playerIndex == manager.localID) {
			selected = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton ("Fire1")) {
			if (playerIndex == manager.localID) {
				if (Physics.CheckSphere(transform.position,sprite.bounds.extents.magnitude/1.618f,manager.selectorLayer)) {
					selected = true;
				}else{
					if (!Input.GetButton ("Shift")) {
						selected = false;
					}
				}
			}
		}
		if (Input.GetButton ("Fire2")) {
			if (selected) {
				targetOverride = SelectTarget ();
			}
		}
		if (targetOverride) {
			target = targetOverride;
			if (!targetUnit) {
				targetUnit = target.GetComponent<Unit>();
			}
		}
		if (experience >= expNeeded) {
			LevelUp ();
		}
		if (newWeapon) {
			EquipWeapon ();
		}
		if (target) {
			if (targetUnit) { targetVel = targetUnit.velocity; }
			if (bulletSpeed > 0) {
				targetPos = CalculateFuturePosition (target.transform.position,targetVel,bulletSpeed);
			}else{
				targetPos = target.transform.position;
			}
			if (weapon) {
				directionToTarget = Mathf.Atan2(targetPos.y-weaponPos.position.y, targetPos.x-weaponPos.position.x)*180 / Mathf.PI;
				distanceToTarget = Vector3.Distance(transform.position,target.transform.position);
			}
		}
		if (weapon) {
			weaponRange = weaponScript.range * bRange;
		}
		direction = transform.rotation.eulerAngles.z;
		if (selected) {
			if (selectedSprite == null) {
				GameObject loc = (GameObject)Instantiate(manager.selectedSprite,transform.position,Quaternion.identity);
				line = loc.GetComponent<LineRenderer>();
				selectedSprite = loc.transform;
				selectedSprite.parent = transform;
				selectedSprite.position -= new Vector3 (0,0,-0.1f);
				selectedSprite.localScale = sprite.bounds.extents*1.1f;
			}else{
				selectedSprite.rotation = Quaternion.identity;
			}
		}else{
			if (selectedSprite) {
				Destroy (selectedSprite.gameObject);
			}
		}
		if (line) {
			if (target) {
				line.SetWidth(0.1f,0.1f);
				line.SetPosition(0,transform.position);
				line.SetPosition(1,target.transform.position);
				line.material.mainTextureScale = new Vector3 (Vector3.Distance (transform.position,target.transform.position),1);
			}
		}
	}

	void FixedUpdate () {
		velocity = -(prevPos - transform.position)/Time.fixedDeltaTime;
		prevPos = transform.position;
	}

	public GameObject SelectTarget () {
		GameObject nearest = null;
		float distance = float.MaxValue;
		if (selected) {
			Collider[] nearby = Physics.OverlapSphere(manager.mousePos,0.1f,enemyLayer);
			for (int i=0;i<nearby.Length;i++) {
				float nd = Vector3.Distance (manager.mousePos,nearby[i].transform.position);
				if (nd < distance) {
					distance = nd;
					nearest = nearby[i].gameObject;
				}
			}
		}
		targetUnit = null;
		return nearest;
	}

	public void Fire () {
		weaponScript.Fire();
	}

	Vector3 CalculateFuturePosition (Vector3 spos, Vector3 vel, float speed) {
		float time = distanceToTarget/speed;
		return spos + vel * time;
	}

	void OnDestroy () {
		if (unitType == "structure") {
			manager.playerControllers[playerIndex].population--;
		}
		if (tag == "Fortress") {
			manager.TestFortresses();
		}
	}

	public void Sell () {
		manager.credits[playerIndex] += Mathf.RoundToInt((float)cost*0.75f);
		Destroy (gameObject);
	}

	void OnDrawGizmos()  {
		if (target) {
			Gizmos.DrawSphere (target.transform.position,0.25f);
			Gizmos.DrawLine (transform.position,transform.position + velocity);
		}
		Gizmos.DrawWireSphere (transform.position,sprite.bounds.extents.magnitude/1.618f);
	}
}
