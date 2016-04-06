using UnityEngine;
using System.Collections;

public class TeslaBeamScript : MonoBehaviour {

	public float distance;
	public float sectionLength;
	public int sections;
	public Vector3 start;
	public Vector3 end;
	public float width;
	public float desturbanceFactor;

	public float time;
	public int changeChance = 20;
	public LineRenderer line;
	public BulletScript bullet;

	// Use this for initialization
	void Start () {
		bullet = GetComponent<BulletScript>();
		line = GetComponent<LineRenderer>();
		if (TestRange ()) {
			start = transform.position;
			end = bullet.target.position;
			Destroy (gameObject,time);
			distance = Vector3.Distance(start,end);
			sections = Mathf.FloorToInt(distance/sectionLength);
			line.SetVertexCount(sections+1);
			line.SetWidth(width,width);
			UpdateLines ();
			bullet.Hit (bullet.target.GetComponent<Collider>(),end,transform.rotation);
		}else{
			Destroy (gameObject);
		}
	}

	bool TestRange () {
		if (bullet.parentChar.distanceToTarget <= bullet.parentChar.weaponScript.range * bullet.parentChar.bRange) {
			return true;
		}
		return false;
	}

	void FixedUpdate () {
		if (TestRange()) {
			if (Random.Range (0,changeChance) == 1) {
				UpdateLines();
			}
		}
	}

	void UpdateLines () {
		Vector3 between = (end-start)/sections;
		for (int i=0;i<sections+1;i++) {
			Vector3 newPos = start + between * i;
			Vector2 r = Random.insideUnitCircle * desturbanceFactor;
			Vector3 newRandom = new Vector3(r.x,r.y,0);
			if (i == 0) {
				newPos = start;
				newRandom = Vector3.zero;
			}
			if (i == sections) {
				newPos = end;
				newRandom = Vector3.zero;
			}
			line.SetPosition(i,newPos + newRandom);
		}
	}
}
