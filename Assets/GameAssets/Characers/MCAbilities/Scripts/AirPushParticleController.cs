using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPushParticleController : MonoBehaviour {

    void Start () {
        StartCoroutine(Destroy());

    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
}
