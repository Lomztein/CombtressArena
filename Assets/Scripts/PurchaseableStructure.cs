using UnityEngine;
using System.Collections;

public class PurchaseButton : MonoBehaviour {

	public int cost;
	public Texture2D purchaseButton;
	public int techLevel;
	public GameObject unit;
	public Unit unitData;
	public Sprite unitSprite;

	void Start () {
		unitSprite = unit.transform.Find ("Sprite").GetComponent<SpriteRenderer>().sprite;
	}
}