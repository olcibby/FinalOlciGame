using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBounceController : MonoBehaviour {

    public float bouncePower = 10f;
    Animator[] animators;
	// Use this for initialization
	void Start () {
        var parent = transform.parent;
        animators = parent.GetComponentsInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter (Collider other)
    {
        if (other.tag == "Player")
        {
            var rigidbody = other.GetComponent<Rigidbody>();
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y + bouncePower, rigidbody.velocity.z);
            other.GetComponent<Animator>().SetBool("DoubleJump", true);

            foreach(Animator anim in animators)
            {
                anim.SetBool("Bounce", true);
                StartCoroutine(Unbounce());
            }
        }
    }

    private IEnumerator Unbounce()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (Animator anim in animators)
        {
            anim.SetBool("Bounce", false);
        }
    }
}
