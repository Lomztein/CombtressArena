using UnityEngine;
using System.Collections;

public class BotManager : MonoBehaviour {

	public int bots;
	public GameObject[] characters;
	public GameObject bot;
	public GameObject[] startingWeapons;
	public float timeScale;

	// Update is called once per frame
	void Update () {
		Time.timeScale = timeScale;
		characters = GameObject.FindGameObjectsWithTag("Character");
		if (characters.Length < bots) {
			Vector3 ranPos = Random.onUnitSphere * characters.Length;
			Vector3 newPos = new Vector3(ranPos.x,ranPos.y,0);
			GameObject newBot = (GameObject)Instantiate (bot,newPos,Quaternion.identity);
			newBot.GetComponent<Unit>().newWeapon = startingWeapons[Random.Range (0,startingWeapons.Length)];
		}
	}
}
