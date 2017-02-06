using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BushLeafController : MonoBehaviour {

    
    public float strength = 1f;
    public float turningRate = 1f;

    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private bool entered = false;
    private List<Transform> otherTransforms;
    private Vector3 averageCenter;
    private Quaternion targetRotation;

    private float xSway = 0f;
    private float zSway = 0f;
    private float actualTurningRate;
    private Transform closestTransform;


	// Use this for initialization
	void Start () {
        initialRotation = transform.rotation;
        initialPosition = transform.GetChild(0).transform.position;
        otherTransforms = new List<Transform>();
        targetRotation = initialRotation;
    }
	
	// Update is called once per frame
	void Update () {
        otherTransforms.RemoveAll(x => x == null);

        if (otherTransforms.Count == 0)
        {
            entered = false;
            targetRotation = initialRotation;
        }

		if (entered == true)
        {

                float minDistance = strength*5000;
            foreach (Transform item in otherTransforms)
            {
                if (Vector3.Distance(item.position, initialPosition) < minDistance)
                {
                    closestTransform = item;
                    minDistance = Vector3.Distance(item.position, initialPosition);
                }
            }
            float averageX = closestTransform.position.x;
            float averageZ = closestTransform.position.z;

            averageCenter = new Vector3(averageX, 0, averageZ);
            if (entered == true && Vector2.Distance(new Vector2(averageCenter.x, averageCenter.z), new Vector2(initialPosition.x, initialPosition.z)) <= strength)
            {
                xSway = strength - (averageCenter.x - initialPosition.x);
                zSway = strength - (averageCenter.z - initialPosition.z);
                xSway = xSway >= 0 && xSway < strength ? 90 * (xSway / strength - 1) : xSway >= strength && xSway < strength * 2 ? -90 * (1 - xSway / strength) : 0f;
                xSway = -xSway;
                zSway = zSway >= 0 && zSway < strength ? 90 * (zSway / strength - 1) : zSway >= strength && zSway < strength * 2 ? -90 * (1 - zSway / strength) : 0f;

                targetRotation = Quaternion.Euler(initialRotation.eulerAngles.x + zSway, 0, initialRotation.eulerAngles.z + xSway);
            }
            else
            {
                targetRotation = initialRotation;
            }
        }

        // Turn towards our target rotation.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turningRate * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        entered = true;
        otherTransforms.Add(other.transform);
    }

    void OnTriggerExit(Collider other)
    {
        otherTransforms.Remove(other.transform);
    }
}
