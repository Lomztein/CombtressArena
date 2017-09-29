using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Unit))]

public class GroundVehicleController : MonoBehaviour, IUnitController {

	public float speed;
	public float maxSpeed;
	public bool threaded;
	public bool swarmer;
	public bool alwaysTurnedTowardsTarget;
	public Unit unit;
    public Vector3 swarmerOffset;

	// Use this for initialization
	void Start () {
		unit = GetComponent<Unit>();
        swarmerOffset = (Vector2)Random.onUnitSphere * unit.weaponRange * 0.75f;
	}

    public float GetSpeed() {
        return maxSpeed;
    }
    
	// Update is called once per frame
	void Update () {

		if (unit.target) {
			Vector3 desiredRotation = Vector3.zero;
			float rot = 0;
			if (threaded) {
				rot = maxSpeed * 10 * Time.deltaTime;
			}else{
				rot = speed * 10 * Time.deltaTime;
			}

            if (unit.distanceToTarget < unit.weaponRange) {
                unit.weaponScript.Fire ();
                if (swarmer) {
                    desiredRotation = new Vector3 (0, 0, unit.directionToTarget - 180);
                    if (speed <= maxSpeed) {
                        speed += maxSpeed * 2.5f * Time.deltaTime;
                    } else {
                        speed = maxSpeed;
                    }
                } else {
                    if (alwaysTurnedTowardsTarget == false) {
                        desiredRotation = new Vector3 (0, 0, unit.direction);
                    } else {
                        desiredRotation = new Vector3 (0, 0, unit.directionToTarget);
                    }
                    if (speed > 0) {
                        speed -= maxSpeed * 5f * Time.deltaTime;
                    } else {
                        speed = 0;
                    }
                }
            } else {
                if (swarmer) {
                    desiredRotation = new Vector3 (0f, 0f, Mathf.Atan2 ((unit.targetPos.y + swarmerOffset.y) - transform.position.y, (unit.targetPos.x + swarmerOffset.x) - transform.position.x) * 180 / Mathf.PI);
                } else {
                    desiredRotation = new Vector3 (0, 0, unit.directionToTarget);
                }
                if (speed <= maxSpeed) {
					speed += maxSpeed * 2.5f * Time.deltaTime;
				}else{
					speed = maxSpeed;
				}
			}
			Quaternion dq = Quaternion.Euler (desiredRotation);
			transform.rotation = Quaternion.RotateTowards(transform.rotation,dq,rot);
			transform.position += transform.right * speed * Time.deltaTime;
		}
	}
}
