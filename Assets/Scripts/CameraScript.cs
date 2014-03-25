using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	public GameObject[] characters;
	public GameObject playerPrefab;
	public GameObject player;
	public float moveSpeed;

	void FixedUpdate () {
		GameObject[] chars = GameObject.FindGameObjectsWithTag("Character");
		if (!player) {
			Vector2 ranPos = Random.insideUnitCircle * chars.Length;
			Vector3 newPos = new Vector3 (ranPos.x,ranPos.y,0);
			player = (GameObject)Instantiate(playerPrefab,newPos,Quaternion.identity);
		}else{
			Vector3 targetPos = new Vector3 (player.transform.position.x,player.transform.position.y,transform.position.z);
			transform.position = Vector3.Lerp (transform.position,targetPos,moveSpeed * Time.deltaTime);
		}
	}
}