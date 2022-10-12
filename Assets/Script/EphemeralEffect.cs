using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EphemeralEffect : MonoBehaviour
{
    private void SelfDestruct_CalledByAnimator()
    {
        Destroy(this.gameObject);
    }
}
