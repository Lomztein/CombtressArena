using UnityEngine;
using System.Collections;

public class BlastBulletScript : MonoBehaviour {

	public BulletScript bullet;
	public SpriteRenderer sprite;
	public float startScale;
	public float growSpeed;
	public float fadeSpeed;

	// Use this for initialization
	void Start () {
		bullet = GetComponent<BulletScript>();
		sprite = transform.FindChild ("Sprite").GetComponent<SpriteRenderer>();
		transform.localScale = Vector3.one * startScale;
		fadeSpeed = 1/bullet.time;
		bullet.damage *= Time.fixedDeltaTime;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale += Vector3.one * growSpeed * Time.deltaTime;
		sprite.color -= new Color (0,0,0,fadeSpeed) * Time.deltaTime;
	}

	void OnTriggerStay (Collider other) {
		bullet.Hit (other,transform.position,transform.rotation);
	}
}
