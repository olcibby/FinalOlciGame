using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowController : MonoBehaviour {

    ParticleSystem particle;

	// Use this for initialization
	void Start () {
        particle = transform.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        particle.enableEmission = !GameController.isSummer;
	}
}
