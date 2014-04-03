using UnityEngine;
using System.Collections;

public class BotInput : MonoBehaviour {

	public GlobalManager manager;
	public PlayerController input;
	public bool placing;
	
	void Start () {
		GameObject stats = GameObject.FindGameObjectWithTag("Stats");
		manager = stats.GetComponent<GlobalManager>();
	}

	void FixedUpdate () {
		if (Random.Range (0,100) == 1) {
			GameObject newUnit = manager.purchaseables[Random.Range (0,manager.purchaseables.Length)];
			input.selectedPurchaseOption = newUnit;
			GameObject newF = input.freindlyFortresses[Random.Range (0,input.freindlyFortresses.Length)];
			if (newF) {
				input.focusPoint = newF.transform.position;
				placing = true;
			}
		}
		if (placing) {
			int value = input.PlacePurchase();
			if (value == 0) {
				placing = false;
			}
			if (value == 1) {
				GameObject newUnit = manager.purchaseables[Random.Range (0,manager.purchaseables.Length)];
				input.selectedPurchaseOption = newUnit;
			}
			if (value == 2) {
				GameObject newF = input.freindlyFortresses[Random.Range (0,input.freindlyFortresses.Length)];
				if (newF) {
					input.focusPoint = newF.transform.position;
				}
			}
			if (value == 3) {
				input.focusPoint += manager.nearbyVectors[Random.Range (0,manager.nearbyVectors.Length)];
			}
		}
	}
}