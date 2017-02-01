﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPushController : MonoBehaviour {

    [SerializeField]
    float m_Speed = 50f;
    [SerializeField]
    float m_MaxDistance = 5f;

    Rigidbody m_Rigidbody;
    Vector3 m_StartingPosition;

    // Use this for initialization
    void Start () {

        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.velocity = new Vector3(m_Speed * transform.forward.x, m_Speed * transform.forward.y , m_Speed * transform.forward.z );
        m_StartingPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(m_StartingPosition, transform.position) >= m_MaxDistance)
        {
            Destroy(this.gameObject);
        }
	}

    void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
