using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour {

    public float mapWidth;
    public float mapHeight;
    public int fortressAmount;
    public float fDistance;
    public float fDistanceFromEnd;
    public float fRange;

    public bool startWithTurrets;
    public GameObject turretType;
    public int turretAmount;
    public float tDistance;
    public float tDistanceFromFortress;

    public GameObject fortress;
    public GameObject [ ] fortresses;

    public GlobalManager manager;
    public static MapManager cur;

    // Use this for initialization
    void Start() {
        manager = GetComponent<GlobalManager> ();
        cur = this;
        InvokeRepeating ("SpawnUnitsLoop", 20, 20);
    }

    public static bool IsWithinMap(Vector3 pos) {
        if (pos.x > cur.mapWidth / 2)
            return false;
        if (pos.x < -cur.mapWidth / 2)
            return false;

        if (pos.y > cur.mapHeight / 2)
            return false;
        if (pos.y < -cur.mapHeight / 2)
            return false;
        return true;
    }

    public static Vector3 GetPosWithinMap() {
        return new Vector3 (Random.Range (-cur.mapWidth / 2, cur.mapWidth / 2), Random.Range (-cur.mapHeight / 2, cur.mapHeight / 2));
    }

    private int [ ] indexes = new int [ 3 ];
    public void SpawnUnitsLoop() {
        ProducingStructure.SpawnOfType ("light");
        if (indexes [ 1 ] == 1)
            ProducingStructure.SpawnOfType ("medium");
        if (indexes [ 2 ] == 2)
            ProducingStructure.SpawnOfType ("heavy");

        for (int i = 0; i < indexes.Length; i++) {
            indexes[i]++;
            indexes[i] %= (i + 1);
        }
    }

	public void GenerateMap () {
		float fortressOffsetY = ((float)fortressAmount-1)*fDistance;
		fortresses = new GameObject[fortressAmount*2];
		for (int i=0;i<fortressAmount*2;i++) {
			Vector3 newPos = new Vector3 (0,0,0);
			int newTeam = -1;
			if (i < fortressAmount) {
				newPos = new Vector3 (mapWidth-fDistanceFromEnd,fortressOffsetY - (i * fDistance*2),0);
				newTeam = 0;
			}else{
				newPos = new Vector3 (-mapWidth+fDistanceFromEnd,fortressOffsetY - ((i-fortressAmount) * fDistance*2),0);
				newTeam = 1;
			}
			GameObject nf = (GameObject)Instantiate(fortress,newPos,Quaternion.identity);
			fortresses[i] = nf;
			Unit newU = nf.GetComponent<Unit>();
            ProducingStructure.InitPlacementNodesList ();
            ProducingStructure.placementNodes[newTeam].Add (newU);
			newU.teamIndex = newTeam;
		}

        ProducingStructure.SetFurthestFactory (0);
        ProducingStructure.SetFurthestFactory (1);

        if (startWithTurrets) {
			float turretOffsetY = ((float)turretAmount-1) * tDistance;
			for (int i=0;i<turretAmount*2;i++) {
				Vector3 newPos = Vector3.zero;
				int newTeam = -1;
				LayerMask freindlyMask = -1;
				LayerMask enemyMask = -1;
				if (i < turretAmount) {
					newPos = new Vector3 (mapWidth-fDistanceFromEnd-tDistanceFromFortress,turretOffsetY - (i * tDistance*2),0);
					freindlyMask = manager.team0Layer;
					enemyMask = manager.team1Layer;
					newTeam = 0;
				}else{
					newPos = new Vector3 (-mapWidth+(fDistanceFromEnd+tDistanceFromFortress),turretOffsetY - ((i-turretAmount) * tDistance*2),0);
					freindlyMask = manager.team1Layer;
					enemyMask = manager.team0Layer;
					newTeam = 1;
				}
				GameObject nt = (GameObject)Instantiate(turretType,newPos,Quaternion.identity);
				Unit newU = nt.GetComponent<Unit>();
				newU.playerIndex = -1;
				newU.teamIndex = newTeam;
				newU.freindlyLayer = freindlyMask;
				newU.enemyLayer = enemyMask;
			}
		}
	}
}
