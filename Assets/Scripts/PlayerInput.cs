using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	Unit character;

	// Use this for initialization
	void Start () {
		character = GetComponent<Unit>();
		character.unitType = "Player";
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButton ("Fire1")) {
			character.Fire();
		}
	}
}
