using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Bulding Data")]
public class TileSO : ScriptableObject
{

    public List<Building> buildings = new List<Building>();


}

[Serializable]
public class Building
{
    [field: SerializeField]
    public Vector2Int Size { get; private set; }
    [field: SerializeField]
    public int Id { get; private set; }
    [field: SerializeField]
    public GameObject Prefab { get; set; }
}
