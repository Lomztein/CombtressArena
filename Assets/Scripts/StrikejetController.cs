using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Unit))]

public class StrikejetController : MonoBehaviour {

	public int rounds;
	public bool infiniteRounds;
	public float speed;
	public float maxSpeed;
	public float rotateSpeed;
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

			if (unit.distanceToTarget < unit.weaponRange) {
				if (unit.weaponScript.reloaded == true && (rounds > 0 || infiniteRounds)) {
					if (unit.weaponScript.Fire ()) {
						rounds--;
					}
				}
			}
		}

		if (unit.sprite.isVisible == false && rounds <= 0 && infiniteRounds == false) {
			Destroy (gameObject);
		}
	}
}
