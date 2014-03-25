using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public float moveSpeed;
	public float zoomSpeed;
	public float gameSpeed;

	void Update () {

		Time.timeScale = gameSpeed;
	}
}