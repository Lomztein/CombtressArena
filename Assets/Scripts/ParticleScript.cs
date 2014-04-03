using UnityEngine;
using System.Collections;

public class ParticleScript : MonoBehaviour {

	GlobalManager manager;

	void Start () {
		manager = GameObject.FindGameObjectWithTag("Stats").GetComponent<GlobalManager>();
		if (manager.particleAmount > manager.maxParticles) {
			Destroy(gameObject);
		}
	}
}
