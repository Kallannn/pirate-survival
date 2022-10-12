using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Match Configuration Obj", menuName = "ScriptableObjects/MatchConfigObject", order = 1)]
public class MatchConfiguration : ScriptableObject
{
    public int matchDuration = 120;
    public float shooterSpawnTime = 5f;
    public float suicideSpawnTime = 2.5f;
}
