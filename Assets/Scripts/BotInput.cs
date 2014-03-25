using UnityEngine;
using System.Collections;

public class BotInput : MonoBehaviour {

	public GlobalManager manager;
	public string[] botNames;
	public PlayerController input;

	public GameObject optimalStructure;
	public Vector2 optimalPosition;

	void Start () {
		GameObject stats = GameObject.FindGameObjectWithTag("Stats");
		manager = stats.GetComponent<GlobalManager>();
		input.playerName = botNames[Random.Range (0,botNames.Length)];
	}
}