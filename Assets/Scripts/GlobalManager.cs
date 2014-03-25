using UnityEngine;
using System.Collections;

public class GlobalManager : MonoBehaviour {

	public LayerMask team0Layer;
	public LayerMask team1Layer;
	public GameObject[] purchaseables;
	public PurchaseButton[] buttons;

	public int players;
	public string[] teamNames;
	public string[] playerNames;
	public int[] credits;
	
	// Use this for initialization
	void Start () {
		teamNames = new string[2];
		playerNames = new string[players];
		credits = new int[players];
		for (int i=0;i<purchaseables.Length;i++) {
			buttons[i] = purchaseables[i].GetComponent<PurchaseButton>();
		}
	}

	void OnGUI () {
		for (int i=0;i<buttons.Length;i++) {
			if (GUI.Button (new Rect(10 + i * 30,Screen.height - 30,20,20),buttons[i].purchaseButton)) {
				Debug.Log ("Bought a unit!");
			}
		}
	}
}
