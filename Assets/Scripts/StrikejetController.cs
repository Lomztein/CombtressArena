using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Unit))]

public class StrikejetController : MonoBehaviour, IUnitController {

	public int rounds;
	public bool infiniteRounds;
	public float speed;
	public float maxSpeed;
	public float rotateSpeed;
	public bool bomber;
	public bool startedRun;
	public Unit unit;
	public Vector3 knownTargetPos;
	public float distanceToReturn;
	public bool returning;

    public float GetSpeed() {
        return maxSpeed;
    }

    // Use this for initialization
    void Start () {
		unit = GetComponent<Unit>();
	}
	
	// Update is called once per frame
	void Update () {

		distanceToReturn = (maxSpeed*(unit.weaponScript.reloadTime*unit.bFirerate))/2;
		if (Vector3.Distance (knownTargetPos,transform.position) > distanceToReturn) {
			returning = true;
		}

		if (unit.target) {
			if (speed < maxSpeed) {
				speed += maxSpeed * 10 * Time.deltaTime;
			}else{
				speed = maxSpeed;
			}
			Quaternion newDir = Quaternion.identity;
			if (startedRun == false) {
				if (rounds > 0 || returning) {
					newDir = Quaternion.Euler(0f,0f,unit.directionToTarget);
				}else{
					if (unit.teamIndex == 0) {
						newDir = Quaternion.identity;
					}else{
						newDir = Quaternion.Euler(0f,0f,180f);
					}
				}
			}else{
				newDir = Quaternion.Euler (0f,0f,unit.direction);
			}
			transform.position += (transform.right * speed * Time.deltaTime);
			transform.rotation = Quaternion.RotateTowards(transform.rotation,newDir,rotateSpeed * Time.deltaTime);

			if (unit.distanceToTarget < unit.weaponRange || startedRun) {
				if (unit.weaponScript.reloaded == true && (rounds > 0 || infiniteRounds)) {
					if (unit.weaponScript.Fire ()) {
						rounds--;
						returning = false;
						knownTargetPos = unit.targetPos;
						if (bomber) {
							startedRun = true;
						}
					}
				}
			}
		}

		if (unit.sprite.isVisible == false && rounds <= 0 && infiniteRounds == false) {
			Destroy (gameObject);
		}
	}
}
