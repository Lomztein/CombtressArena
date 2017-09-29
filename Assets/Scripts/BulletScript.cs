using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

	public float damage;
	public Vector3 velocity;
	public Unit parentChar;
	public float range;
	public float time;
	public string damageType;
	public LayerMask layer;
	public bool dealNativeDamage = true;
	public bool modifySize;
	public bool homing;
	public bool piercing;
	public bool applyEffect;
    public bool hitscan;
	public GameObject effect;
	public float speedZ;
	public float turnSpeed;
	public Transform target;
	public float rangeOverride;
	public GameObject hitParticle;
    public Vector3 targetPosAtStart;
	float speed;
	SpriteRenderer sprite;
	HealthScript healthScript;

    public float particleDieTime = 1f;
    public GameObject particle;

	public virtual void Start () {
		sprite = transform.Find ("Sprite").GetComponent<SpriteRenderer>();
		if (modifySize) {
			sprite.transform.localScale = new Vector3 (0.45f,velocity.magnitude * 2 * Time.fixedDeltaTime,1);
			sprite.transform.localPosition += new Vector3(sprite.bounds.extents.x,0,0);
		}
	    speed = velocity.magnitude;
		if (Mathf.RoundToInt (rangeOverride) != 0) {
			range = rangeOverride;
		}
		time = range/velocity.magnitude;
		if (target) {
			if (Mathf.Round (transform.position.z) == Mathf.Round (targetPosAtStart.z)) {
				speedZ = 0;
			}else{
				speedZ = (transform.position.z - targetPosAtStart.z) / Vector2.Distance (transform.position, targetPosAtStart) * speed; // The amount of units down it should move per unit forward.
				if (homing) {
					speedZ = Mathf.Abs(speedZ);
				}
			}
		}
        transform.rotation = Quaternion.Euler (0f, 0f, Mathf.Atan2 (velocity.y, velocity.x) * 180 / Mathf.PI);
		Ray ray = new Ray(transform.position,velocity);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, velocity.magnitude * Time.fixedDeltaTime,layer)) {
			if (dealNativeDamage) {
				Hit(hit.collider,hit.point,transform.rotation);
			}
		}
        ParticleSystem pSystem = GetComponentInChildren<ParticleSystem> ();
        if (pSystem)
            particle = pSystem.gameObject;
	}

	public virtual void FixedUpdate () {
		Ray ray = new Ray(transform.position,velocity);
		RaycastHit hit;
		if (homing) {
            if (target) {
                float angle = Mathf.Atan2 (target.position.y - transform.position.y, target.position.x - transform.position.x) * 180 / Mathf.PI;
                Quaternion newDir = Quaternion.Euler (0f, 0f, angle);
                transform.rotation = Quaternion.RotateTowards (transform.rotation, newDir, turnSpeed * Time.fixedDeltaTime);
                ray = new Ray (transform.position, (target.position - transform.position).normalized);

                Vector3 targetZ = new Vector3 (transform.position.x, transform.position.y, target.position.z);
                transform.position = Vector3.MoveTowards (transform.position, targetZ, speedZ * Time.fixedDeltaTime);

                ray = new Ray (transform.position, (target.position - transform.position).normalized);
            } else {
                transform.position += transform.forward * speedZ * Time.fixedDeltaTime;
            }
			transform.position += transform.right * speed * Time.fixedDeltaTime;
		}else{
            transform.position += -(transform.forward * speedZ) * Time.fixedDeltaTime;
			transform.position += velocity * Time.fixedDeltaTime;
		}
		if (Physics.Raycast (ray, out hit, velocity.magnitude * Time.fixedDeltaTime,layer)) {
			if (dealNativeDamage) {
				Hit(hit.collider,hit.point,transform.rotation);
			}
		}
		if (time > 0) {
			time -= Time.fixedDeltaTime;
		}else{
            DetachParticle ();
			Destroy (gameObject);
		}
	}

	public void Hit (Collider other, Vector3 pos, Quaternion rot) {
		if (hitParticle) {
			Instantiate(hitParticle,pos+transform.forward * -2,rot);
		}
		healthScript = other.gameObject.GetComponent<HealthScript>();
        if (healthScript) {
            if (applyEffect) {
                GameObject ef = (GameObject)Instantiate (effect, other.transform.position, Quaternion.identity);
                ef.transform.parent = other.transform;
            }
            if (piercing == false) {
                Destroy (gameObject);
            }
            bool isAll = damageType == "all"; // Senseless optimizations!
            if (healthScript.armorType == damageType || isAll) {
                healthScript.health -= damage;
                parentChar.experience += isAll ? damage / 25 : damage / 5;
            } else {
                healthScript.health -= damage / 5;
                parentChar.experience += damage / 25;
            }
            healthScript.lastHit = parentChar;
            DetachParticle ();
        }
	}

    private void DetachParticle() {
        if (particle) {
            particle.transform.parent = null;
            particle.GetComponent<ParticleSystem> ().Stop ();
            Destroy (particle, particleDieTime);
        }
    }
}