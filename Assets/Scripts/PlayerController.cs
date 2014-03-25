using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public int id;
	public int teamIndex;
	public string playerName;

	public Vector3 focusPoint;
	public GameObject selectedPurchaseOption;
	public GlobalManager manager;

	public bool botControlled;
	public BotInput bot;

	// Use this for initialization
	void Start () {
		manager = GetComponent<GlobalManager>();
		if (botControlled) {
			bot = new BotInput();
			bot.input = this;
		}
	}
	
	// Update is called once per frame
	void PlacePurchase () {
		Unit purchaseUnit = selectedPurchaseOption.GetComponent<Unit>();
		GameObject purchase = (GameObject)Instantiate(selectedPurchaseOption,focusPoint,Quaternion.identity);
		Unit newU = purchase.GetComponent<Unit>();
		newU.teamIndex = teamIndex;
		newU.playerIndex = id;
		newU.playerName = playerName;
		newU.teamName = manager.teamNames[teamIndex];
		if (teamIndex == 0) {
			newU.freindlyLayer = manager.team0Layer;
			newU.enemyLayer = manager.team1Layer;
		}else{
			newU.freindlyLayer = manager.team1Layer;
			newU.enemyLayer = manager.team0Layer;
		}
	}
}
