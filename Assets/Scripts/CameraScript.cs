using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public float moveSpeed;
	public float zoomSpeed;
	public float gameSpeed;

	void Update () {

		Time.timeScale = gameSpeed;

		Vector3 movement = Vector3.zero;
		Vector3 mp = Input.mousePosition;
		if (mp.x < 10) {
			movement += Vector3.left * moveSpeed;
		}
		if (mp.x > Screen.width - 10) {
			movement += Vector3.right * moveSpeed;
		}
		if (mp.y < 10) {
			movement += Vector3.down * moveSpeed;
		}
		if (mp.y > Screen.height - 10) {
			movement += Vector3.up * moveSpeed;
		}
		if (Input.GetButton ("Fire2")) {
			movement += Vector3.right * Input.GetAxis ("Mouse X") * moveSpeed/5;
			movement += Vector3.up * Input.GetAxis ("Mouse Y") * moveSpeed/5;
		}
		Camera.main.orthographicSize -= Input.GetAxis ("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
		Camera.main.transform.position += movement * Time.deltaTime;
	}
}