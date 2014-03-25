using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Unit))]

public class InfantryController : MonoBehaviour {

	public Vector3 targetPos;
	public Vector3 velocity;
	public Transform pointer;
	public float maxSpeed;
	public float angle;
	public Vector2 movement;
	public Unit unit;
	public TargetFinder tf;

	// Use this for initialization
	void Start () {
		pointer = new GameObject("Pointer").transform;
		pointer.transform.position = transform.position;
		pointer.transform.parent = transform;
		unit = GetComponent<Unit>();
		tf = GetComponent<TargetFinder>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (unit.target) {

			targetPos = unit.targetPos;
			if (unit.distanceToTarget > unit.weaponRange) {
				velocity = transform.right * maxSpeed * Time.deltaTime;
				movement = new Vector3(velocity.x,velocity.y,0);
				transform.position = new Vector3 (movement.x,movement.y,0f) + transform.position;
			}else{
				unit.weaponScript.Fire();
			}

			pointer.LookAt (targetPos,Vector3.up);
			angle = unit.directionToTarget;
			
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

		}
	
	}
}
