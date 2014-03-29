using UnityEngine;
using System.Collections;

public class GlobalManager : MonoBehaviour {

	public LayerMask team0Layer;
	public LayerMask team1Layer;
	public GameObject[] purchaseables;
	public PlayerController[] playerControllers;
	public PlayerController localPlayer;
	public Light sun;

	public int players;
	public string[] teamNames;
	public int[] credits;

	public float buttonSize;
	public float buttonDistance;

	public float menuOffset;

	// Use this for initialization
	void Start () {
		teamNames = new string[2];
		credits = new int[players];
	}

	void Update () {
		sun.intensity = Mathf.Sin ((Time.time+240)/240)/5+0.3f;
	}

	void OnGUI () {
		if ((buttonSize + buttonDistance) * purchaseables.Length + 20 > Screen.width) {
			float buttonsOnScreen = (buttonSize + buttonDistance)/Screen.width;
			//Debug.Log (buttonsOnScreen);
			menuOffset = GUI.HorizontalSlider (new Rect(10,Screen.height-buttonSize-30,Screen.width-20,20),menuOffset,((-buttonSize - buttonDistance) * purchaseables.Length),0);
		}
		for (int i=0;i<purchaseables.Length;i++) {
			if (GUI.Button (new Rect(menuOffset + 10 + i * (buttonSize + buttonDistance),Screen.height - buttonSize-10,buttonSize,buttonSize),purchaseables[i].name)) {
				localPlayer.selectedPurchaseOption = purchaseables[i];
				Debug.Log ("Bought a unit");
			}
		}
	}
}