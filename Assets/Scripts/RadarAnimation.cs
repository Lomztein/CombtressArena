using UnityEngine;
using System.Collections;

public class RadarAnimation : MonoBehaviour {

	public Transform dish;
	public float rotateSpeed;
	public bool rotate;
	
	// Update is called once per frame
	void Update () {
		if (rotate) {
			dish.Rotate (0,0,rotateSpeed * Time.deltaTime);
		}
	}
}
