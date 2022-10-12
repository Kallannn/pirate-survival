using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBehavior : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private bool followOnX = true;
    [SerializeField] private bool followOnY = true;
    [SerializeField] private bool followOnZ = true;

    private float newXPosition => (followOnX ? targetToFollow.position.x : this.transform.position.x);
    private float newYPosition => (followOnY ? targetToFollow.position.y : this.transform.position.y);
    private float newZPosition => (followOnZ ? targetToFollow.position.z : this.transform.position.z);

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = new Vector3(newXPosition, newYPosition, newZPosition);
    }
}
