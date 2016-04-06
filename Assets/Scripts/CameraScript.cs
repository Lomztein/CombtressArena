using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public float moveSpeed;
	public float zoomSpeed;
	public MapManager map;
	public float standardSize;

	public float minCamSize = 2;
	public float maxCamSize;

	public GameObject hudCam;
	public Transform pointer;

	static public Transform follow;

	void Start () {
		hudCam = GameObject.Find ("HudCamera");
		map = GameObject.FindGameObjectWithTag("Stats").GetComponent<MapManager>();
		standardSize = GetComponent<Camera>().orthographicSize;
		maxCamSize = Mathf.Max (map.mapWidth,map.mapHeight);
		if (map.mapWidth > map.mapHeight) {
			maxCamSize /= Camera.main.aspect;
		}
	}
	void Update () {

		Vector3 movement = Vector3.zero;
		Vector3 mp = Input.mousePosition;
		pointer.position = hudCam.GetComponent<Camera>().ScreenToWorldPoint(mp+Vector3.forward*5);
		float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
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
		if (Camera.main.orthographicSize - zoom * Time.deltaTime > maxCamSize) {
			Camera.main.orthographicSize = maxCamSize;
			zoom = 0;
		}
		if (Camera.main.orthographicSize - zoom * Time.deltaTime < minCamSize) {
			Camera.main.orthographicSize = minCamSize;
			zoom = 0;
		}
		Camera.main.orthographicSize -= zoom * Time.deltaTime * GetComponent<Camera>().orthographicSize/standardSize;
		Camera.main.transform.position += movement * Time.deltaTime * GetComponent<Camera>().orthographicSize/standardSize;
		if (follow) {
			transform.position = new Vector3(follow.position.x,follow.position.y,transform.position.z);
		}
	}
}