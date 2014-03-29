using UnityEngine;
using System.Collections;

public class GlobalManager : MonoBehaviour {

	public LayerMask team0Layer;
	public LayerMask team1Layer;
	public GameObject[] purchaseables;
	public PurchaseButton[] buttons;
	public PlayerController[] playerControllers;
	public PlayerController localPlayer;
	public Light sun;

	public int players;
	public string[] teamNames;
	public int[] credits;
	
	// Use this for initialization
	void Start () {
		teamNames = new string[2];
		credits = new int[players];
	}

	void Update () {
		sun.intensity = Mathf.Sin (Time.time/240)/5+0.3f;
	}

	void OnGUI () {
		for (int i=0;i<buttons.Length;i++) {
			if (GUI.Button (new Rect(10 + i * 30,Screen.height - 30,20,20),buttons[i].purchaseButton)) {
				localPlayer.selectedPurchaseOption = purchaseables[i];
				Debug.Log ("Bought a unit");
			}
		}
	}
}
