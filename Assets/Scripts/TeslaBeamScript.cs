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
    private int arcDir = -1;
    public float arcSpeed = 5f;

    // Use this for initialization
    void Start() {
        bullet = GetComponent<BulletScript> ();
        line = GetComponent<LineRenderer> ();
        start = transform.position;

        Ray ray = new Ray (transform.position, bullet.velocity);
        RaycastHit hit;

        if (Physics.Raycast (ray, out hit, bullet.range, bullet.layer)) {
            end = hit.point;
            Destroy (gameObject, time);
            bullet.Hit (hit.collider, end, transform.rotation);
            distance = hit.distance;
        } else {
            end = ray.GetPoint (bullet.range);
            distance = Vector3.Distance (start, end);
        }

        if (Random.Range (0, 2) == 1) {
            arcDir = 1;
        }

        sections = Mathf.FloorToInt (distance / sectionLength);

        line.positionCount = sections + 1;
        line.startWidth = width;
        line.endWidth = width;
        UpdateLines ();
    }

    void FixedUpdate() {
        UpdateLines ();
        line.material.color = new Color (line.material.color.r, line.material.color.g, line.material.color.b, line.material.color.a - (1/time) * Time.fixedDeltaTime);
    }

    void UpdateLines() {
        if (Random.Range (0, changeChance) == 0) {
            Vector3 between = (end - start) / sections;

            for (int i = 0; i < sections + 1; i++) {
                Vector3 newPos = start + between * i;
                Vector2 r = Random.insideUnitCircle * desturbanceFactor;
                Vector3 newRandom = new Vector3 (r.x, r.y, 0);
                if (i == 0) {
                    newPos = start;
                    newRandom = Vector3.zero;
                }
                if (i == sections) {
                    newPos = end;
                    newRandom = Vector3.zero;
                }
                line.SetPosition (i, newPos + newRandom);
            }
        } else {
            for (int i = 0; i < sections; i++) {
                Vector3 rot = Quaternion.Euler (0f, 0f, 90f + Random.Range (20f, -20f)) * bullet.velocity.normalized;
                Vector3 nextPos = line.GetPosition (i) + rot * desturbanceFactor * Random.Range (-2f * arcDir, 4f * arcDir) * arcSpeed * Time.fixedDeltaTime * Mathf.Sin ((float)i/sections * Mathf.PI) * ((end-start).magnitude / 20f);
                line.SetPosition (i, nextPos);
            }
        }
    }
}
