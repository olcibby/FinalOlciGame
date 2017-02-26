using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPushController : MonoBehaviour
{

    [SerializeField]
    float m_Speed = 5;
    [SerializeField]
    float m_MaxDistance = 5f;
    [SerializeField]
    float m_PushSpeed = 10f;

    Vector3 m_StartingPosition;
    private float m_ExpandSpeed;
    // Use this for initialization
    void Start()
    {
        m_StartingPosition = transform.position;
        m_ExpandSpeed = 1;
        StartCoroutine(Expand());
    }

    // Update is called once per frame
    void Update()
    {
            
    }

    void FixedUpdate()
    {
        transform.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x * m_Speed, 0, transform.forward.z * m_Speed);
        //transform.Translate(0, 0, m_Speed);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z + m_ExpandSpeed);
    }

    void OnCollisionEnter(Collision collision)
    {
        //Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null)
        {
            other.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x * m_PushSpeed, transform.forward.y * m_PushSpeed, transform.forward.z * m_PushSpeed);
        }
    }

    IEnumerator Expand()
    {
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(End());
        m_ExpandSpeed = -m_ExpandSpeed;
        //Destroy(this.gameObject);
    }

    IEnumerator End()
    {
        yield return new WaitForSeconds(0.4f);
        Destroy(this.gameObject);
    }
}
