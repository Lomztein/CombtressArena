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
			input.focusPoint = input.freindlyFortresses[Random.Range (0,input.freindlyFortresses.Length)].transform.position;
			placing = true;
		}
		if (placing) {
			if (input.PlacePurchase() == false) {
				input.focusPoint += manager.nearbyVectors[Random.Range (0,manager.nearbyVectors.Length)];
				if (input.PlacePurchase ()) {
					placing = false;
				}
			}else{
				placing = false;
			}
		}
	}
}