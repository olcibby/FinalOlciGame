using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimbController : MonoBehaviour {

    MainCharacterController mainCharController;
    // Use this for initialization
    void Start () {
        mainCharController = transform.parent.parent.GetComponent<MainCharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter (Collider other)
    {
        if (other.tag == "WallClimb")
        {
            mainCharController.wallClimb = true;
            mainCharController.climbingWall = other.transform;
        }
    }

    void OnTriggerExit (Collider other)
    {
        if (other.tag == "WallClimb")
        {
            mainCharController.wallClimb = false;
            mainCharController.climbingWall = null;
        }
    }
}
