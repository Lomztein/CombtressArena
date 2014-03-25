using UnityEngine;
using System.Collections;

public class BotInput : MonoBehaviour {

	public GlobalManager manager;
	public int playerIndex;
	public string[] botNames;

	public GameObject optimalStructure;
	public Vector2 optimalPosition;

	void Start () {
		GameObject stats = GameObject.FindGameObjectWithTag("Stats");
		manager = stats.GetComponent<GlobalManager>();
		manager.playerNames[playerIndex] = botNames[Random.Range (0,botNames.Length)];
	}
}