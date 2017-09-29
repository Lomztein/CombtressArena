using UnityEngine;
using System.Collections;
using System.Linq;

public class MineFactoryController : MonoBehaviour {

	public GameObject mine;
	public float spawnSpeed;
	public float mineDamage;
	public float range;
	public Unit unit;
	public MapManager map;

    public float minDistance = 3f;
    public float maxDistance = 10f;

	// Use this for initialization
	void Start () {
		unit = GetComponent<Unit>();
		map = unit.map;
		Invoke ("SpawnMine",spawnSpeed * unit.bFirerate);
	}

    void SpawnMine() {
        Invoke ("SpawnMine", spawnSpeed * unit.bFirerate);

        Vector3 curPos = transform.position;
        float curDistance = 0f;
        int maxTries = 500;
        while (curDistance < minDistance || curDistance > maxDistance || maxTries < 0) {
            curPos = MapManager.GetPosWithinMap ();
            Transform nearest = TargetFinder.FindClosest (curPos, ProducingStructure.teamProducingStructures [ unit.teamIndex ].ConvertAll (x => x.transform).ToArray ());
            curDistance = Vector3.Distance (curPos, nearest.position);
            maxTries--;
        }

        GameObject newMine = (GameObject)Instantiate (mine, curPos, Quaternion.identity);
        MineController ms = newMine.GetComponent<MineController> ();
        ms.layer = unit.enemyLayer;
        ms.damage = mineDamage * unit.bDamage;
        ms.unit = unit;
    }
}