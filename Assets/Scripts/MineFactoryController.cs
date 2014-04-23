using UnityEngine;
using System.Collections;

public class MineFactoryController : MonoBehaviour {

	public GameObject mine;
	public float spawnSpeed;
	public float mineDamage;
	public float range;
	public Unit unit;
	public MapManager map;

	// Use this for initialization
	void Start () {
		unit = GetComponent<Unit>();
		map = unit.map;
		Invoke ("SpawnMine",spawnSpeed * unit.bFirerate);
	}

	void SpawnMine () {
		Invoke("SpawnMine",spawnSpeed * unit.bFirerate);
		Vector2 pos = new Vector2 ( transform.position.x,transform.position.y );
		Vector2 newPos = pos + Random.insideUnitCircle * range * unit.bRange;
		GameObject newMine = (GameObject)Instantiate (mine,newPos,Quaternion.identity);
		MineController ms = newMine.GetComponent<MineController>();
		ms.layer = unit.enemyLayer;
		ms.damage = mineDamage * unit.bDamage;
		ms.unit = unit;
	}
}