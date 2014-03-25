using UnityEngine;
using System.Collections;

public class WeaponPickupScript : MonoBehaviour {

	public GameObject[] weapons;
	private GameObject pickup;
	private Vector3 screenPos;
	private string weaponName;

	// Use this for initialization
	void Start () {
		pickup = weapons[Random.Range (0,weapons.Length)];
		weaponName = pickup.GetComponent<WeaponStatsScript>().weaponName;
	}
	
	void OnTriggerEnter (Collider other) {
		Debug.Log ("Weapon picked up!");
		Destroy (gameObject);
		other.gameObject.GetComponent<Unit>().newWeapon = pickup;
	}

	void OnGUI () {
		screenPos = Camera.main.WorldToScreenPoint (transform.position);
		GUI.Label(new Rect(screenPos.x - pickup.name.Length * 3,screenPos.y - 30f,Screen.width,Screen.height),weaponName);
	}
}