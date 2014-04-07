using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public float moveSpeed;
	public float zoomSpeed;
	public float gameSpeed;
	public MapManager map;

	void Start () {
		map = GameObject.FindGameObjectWithTag("Stats").GetComponent<MapManager>();
	}
	void OnGUI () {

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
		if (transform.position.x + movement.x*Time.deltaTime > map.mapWidth) {
			transform.position = new Vector3 (map.mapWidth,transform.position.y,transform.position.z);
			movement = new Vector3 (0,movement.y);
		}
		if (transform.position.x + movement.x*Time.deltaTime < -map.mapWidth) {
			transform.position = new Vector3 (-map.mapWidth,transform.position.y,transform.position.z);
			movement = new Vector3 (0,movement.y);
		}
		if (transform.position.y + movement.y*Time.deltaTime > map.mapHeight) {
			transform.position = new Vector3 (transform.position.x,map.mapHeight,transform.position.z);
			movement = new Vector3 (movement.x,0);
		}
		if (transform.position.y + movement.y*Time.deltaTime < -map.mapHeight) {
			transform.position = new Vector3 (transform.position.x,-map.mapHeight,transform.position.z);
			movement = new Vector3 (movement.x,0);
		}
		Camera.main.orthographicSize -= Input.GetAxis ("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
		Camera.main.transform.position += movement * Time.deltaTime;
	}
}