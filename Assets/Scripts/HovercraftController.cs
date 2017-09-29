using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovercraftController : MonoBehaviour, IUnitController {

    public Unit unit;

    public Vector3 velocity;
    public float maxVelocityMagnitude;
    public float angularSpeed;
    public float accelSpeed;

    void Start() {
        unit = GetComponent<Unit> ();
    }

    public float GetSpeed() {
        return maxVelocityMagnitude;
    }

    // Update is called once per frame
    void FixedUpdate() {
        transform.position += velocity * Time.fixedDeltaTime;

        if (unit.target) {
            transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0f, 0f, unit.directionToTarget), angularSpeed * Time.fixedDeltaTime);

            float desiredRange = unit.weaponRange;
            Vector3 vectorToTarget = (unit.target.transform.position - transform.position).normalized;

            if (unit.distanceToTarget - desiredRange < 0) {
                unit.weaponScript.Fire ();
                if (velocity.magnitude > accelSpeed / 2f) {
                    velocity *= 0.95f;
                }
                if (unit.distanceToTarget - desiredRange < -2f) {
                    velocity += (vectorToTarget * -accelSpeed * Time.fixedDeltaTime);
                }
            } else {
                velocity += vectorToTarget * accelSpeed * Time.fixedDeltaTime;

                if (velocity.sqrMagnitude < maxVelocityMagnitude * maxVelocityMagnitude) {
                    velocity = velocity.normalized * maxVelocityMagnitude;
                }
            }
        }
    }
}