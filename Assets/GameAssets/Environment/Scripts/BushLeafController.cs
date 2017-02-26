using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BushLeafController : MonoBehaviour {

    
    public float distanceTreshold = 1f;
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
            //NOTE:set threshold from object
            float minDistance = distanceTreshold * 5000;
            foreach (Transform item in otherTransforms)
            {
                if (Vector3.Distance(item.position, initialPosition) < minDistance)
                {
                    closestTransform = item;
                    minDistance = Vector3.Distance(item.position, initialPosition);
                }
            }
            float closestX = closestTransform.position.x;
            float closestZ = closestTransform.position.z;

            float distanceFromPlant = Vector2.Distance(new Vector2(closestX, closestZ), new Vector2(initialPosition.x, initialPosition.z));

            if (distanceFromPlant <= distanceTreshold)
            {
                float xDistance = closestX - initialPosition.x;
                float zDistance = closestZ - initialPosition.z;

                float positiveXDistance = Mathf.Abs(xDistance);
                float positiveZDistance = Mathf.Abs(zDistance);

                float xRatio;
                float zRatio;
                if (!(positiveXDistance < 0.01f && positiveZDistance < 0.01f))
                {
                    xRatio = positiveXDistance >= positiveZDistance ? 1 : positiveXDistance / positiveZDistance;
                    zRatio = positiveZDistance >= positiveXDistance ? 1 : positiveZDistance / positiveXDistance;
                }
                else
                {
                    xRatio = zRatio = 1;
                }

                xRatio = xDistance > 0 ? xRatio : -xRatio;
                zRatio = zDistance > 0 ? zRatio : -zRatio;

                float sway = 90 * (1 - distanceFromPlant / distanceTreshold);
                xSway = sway * xRatio;
                zSway = sway * zRatio;

                targetRotation = Quaternion.Euler(initialRotation.eulerAngles.x - zSway, 0, initialRotation.eulerAngles.z + xSway);
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
