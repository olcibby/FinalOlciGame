using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    public GameObject DialUIWinter;
    public GameObject DialUISummer;
    public GameObject Mask1UI;
    public GameObject Mask1UION;
    public GameObject Mask2UI;
    public GameObject Mask2UION;
    public GameObject PickUpText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Mask1UI.SetActive(GameController.mask1Collected && !GameController.mask1Equipped);
        Mask1UION.SetActive(GameController.mask1Collected && GameController.mask1Equipped);
        Mask2UI.SetActive(GameController.mask2Collected && !GameController.mask2Equipped);
        Mask2UION.SetActive(GameController.mask2Collected && GameController.mask2Equipped);
        DialUIWinter.SetActive(GameController.dialCollected && !GameController.isSummer);
        DialUISummer.SetActive(GameController.dialCollected && GameController.isSummer);
        PickUpText.SetActive(GameController.displayPickupText);
	}
}
