using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask1PickupController : MonoBehaviour {


    private bool active = false;
    GameObject other;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && active)
        {
            GameController.mask1Collected = true;
            other.transform.parent.gameObject.SetActive(false);
            GameController.displayPickupText = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GameController.displayPickupText = true;
        this.other = other.gameObject;
        active = true;
    }

    void OnTriggerExit(Collider other)
    {
        GameController.displayPickupText = false;
        active = false;
    }
}
