using UnityEngine;
using System.Collections;

public class GlobalManager : MonoBehaviour {

	public LayerMask team0Layer;
	public LayerMask team1Layer;
	public GameObject[] purchaseables;
	public GameObject factory;
	public PlayerController[] playerControllers;
	public PlayerController localPlayer;
	public Light sun;

	public Texture2D buttonBackground;
	Texture2D[] buttonSprites;

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
		buttonSprites = new Texture2D[purchaseables.Length];

		for (int i=0;i<purchaseables.Length;i++) {
			buttonSprites[i] = purchaseables[i].transform.FindChild ("Sprite").GetComponent<SpriteRenderer>().sprite.texture;
		}
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
			Unit newU = purchaseables[i].GetComponent<Unit>();
			Rect rect = new Rect (menuOffset + 10 + i * (buttonSize + buttonDistance),Screen.height - buttonSize-10,buttonSize,buttonSize);
			//GUI.Label (rect,buttonBackground);
			if (GUI.Button (rect,buttonSprites[i])) {
				Vector3 newPos = Vector3.zero;
				GameObject newFactory = (GameObject)Instantiate(factory,newPos,Quaternion.identity);
				ProducingStructure fac = newFactory.GetComponent<ProducingStructure>();
				fac.unit = purchaseables[i];
				fac.time = 10;
			}
		}
	}
}