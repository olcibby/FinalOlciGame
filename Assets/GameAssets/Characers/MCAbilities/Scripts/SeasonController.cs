using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonController : MonoBehaviour {

    public bool isSummer = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToggleSeason()
    {
        isSummer = !isSummer;
        var summerObjects = GameObject.FindGameObjectsWithTag("Summer");
        var winterObjects = GameObject.FindGameObjectsWithTag("Winter");

        foreach (var obj in summerObjects)
        {
            if (obj.GetComponent<Renderer>() != null)
                obj.GetComponent<Renderer>().enabled = isSummer;
            if (obj.GetComponent<Collider>() != null)
                obj.GetComponent<Collider>().enabled = isSummer;
            if (obj.GetComponent<Light>() != null)
                obj.GetComponent<Light>().enabled = isSummer;
        }

        foreach (var obj in winterObjects)
        {
            if (obj.GetComponent<Renderer>() != null)
                obj.GetComponent<Renderer>().enabled = !isSummer;
            if (obj.GetComponent<Collider>() != null)
                obj.GetComponent<Collider>().enabled = !isSummer;
            if (obj.GetComponent<Light>() != null)
                obj.GetComponent<Light>().enabled = !isSummer;
        }
    }
}
