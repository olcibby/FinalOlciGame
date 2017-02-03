using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPushParticleController : MonoBehaviour {

    ParticleSystem emitter;
    [SerializeField]
    float m_Speed = 50f;
    [SerializeField]
    float m_MaxDistance = 5f;

    Rigidbody m_Rigidbody;
    // Use this for initialization
    void Start () {

        m_Rigidbody = GetComponent<Rigidbody>();
        emitter = transform.GetComponent<ParticleSystem>();

        m_Rigidbody.velocity = new Vector3(m_Speed * transform.forward.x, m_Speed * transform.forward.y, m_Speed * transform.forward.z);
    }
	
	// Update is called once per frame
	void Update () {
    }

    public void Detach()
    {
        transform.parent = null;
        emitter.Stop();
    }
}
