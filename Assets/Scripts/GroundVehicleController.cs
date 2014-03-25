using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Unit))]

public class GroundVehicleController : MonoBehaviour {

	public float speed;
	public float maxSpeed;
	public bool threaded;
	public Unit unit;

	// Use this for initialization
	void Start () {
		unit = GetComponent<Unit>();
	}
	
	// Update is called once per frame
	void Update () {

		if (unit.target) {
			Vector3 desiredRotation = new Vector3 (0,0,unit.directionToTarget);
			Quaternion dq = Quaternion.Euler (desiredRotation);
			float rot = 0;
			if (threaded) {
				rot = maxSpeed * 10 * Time.deltaTime;
			}else{
				rot = speed * 10 * Time.deltaTime;
			}
			transform.rotation = Quaternion.RotateTowards(transform.rotation,dq,rot);

			if (unit.distanceToTarget < unit.weaponRange) {
				unit.weaponScript.Fire ();
				if (speed > 0) {
					speed -= maxSpeed * 5f * Time.deltaTime;
				}else{
					speed = 0;
				}
			}else{
				if (speed <= maxSpeed) {
					speed += maxSpeed * 2.5f * Time.deltaTime;
				}else{
					speed = maxSpeed;
				}
			}
			transform.position += transform.right * speed * Time.deltaTime;
		}
	}
}
