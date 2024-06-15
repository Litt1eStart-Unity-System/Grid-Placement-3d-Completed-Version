using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="buildingSO", menuName="ScriptableObjects/BuildingSO")]
public class BuildingSO : ScriptableObject
{
    public Vector2Int Size;
    public string DisplayName;
    public GameObject prefab;
    public GameObject overlayPrefab;
}
