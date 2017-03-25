using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    public GameObject DialUIWinter;
    public GameObject Mask1UI;
    public GameObject Mask2UI;
    public GameObject PickUpText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Mask1UI.SetActive(GameController.mask1Collected);
        Mask2UI.SetActive(GameController.mask2Collected);
        DialUIWinter.SetActive(GameController.dialCollected);
        PickUpText.SetActive(GameController.displayPickupText);
	}
}
