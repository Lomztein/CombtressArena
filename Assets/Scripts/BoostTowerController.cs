using UnityEngine;
using System.Collections;

public class BoostTowerController : MonoBehaviour {

	public GameObject boost;
	public float size = 10;
	public float time;
	public float boostSpeed;
	public Unit unit;
	public float spawnTime;
	
	public bool heal;
	public bool repair;

	// Use this for initialization
	void Start () {
		unit = GetComponent<Unit>();
		Invoke ("SpawnBoost",spawnTime * unit.bFirerate);
	}
	
	void SpawnBoost () {
		Invoke ("SpawnBoost",spawnTime * unit.bFirerate);
		GameObject nb = (GameObject)Instantiate(boost,transform.position,Quaternion.identity);
		BoostWaveScript bs = nb.GetComponent<BoostWaveScript>();
		bs.endSize = size * unit.bRange;
		bs.time = time;
		bs.boostSpeed = boostSpeed * unit.bDamage;
		bs.heal = heal;
		bs.repair = repair;
		bs.parentUnit = unit;
	}
}
