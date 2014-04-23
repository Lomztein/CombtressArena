using UnityEngine;
using System.Collections;

public class MinePlacementScript : MonoBehaviour {

	public GameObject mine;
	public float range;
	public int amount;
	public Unit unit;

	// Use this for initialization
	void Start () {
		unit = GetComponent<Unit>();
		Vector2 pos = new Vector2 (transform.position.x,transform.position.y);
		for (int i=0;i<amount;i++) {
			Vector2 newPos = pos + Random.insideUnitCircle*range;
			GameObject newMine = (GameObject)Instantiate (mine,newPos,Quaternion.identity);
			MineController ms = newMine.GetComponent<MineController>();
			ms.layer = unit.enemyLayer;
		}
		Destroy(gameObject);
	}
}
