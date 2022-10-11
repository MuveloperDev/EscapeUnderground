using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class MapSelectManager : MonoBehaviourPun
{
    [SerializeField] int mapSelect;
    // ∑£¥˝«— ∏ ¿« ¿Œµ¶Ω∫∏¶ º≥¡§
    private void Awake()
    {
        mapSelect = Random.Range(1, 3);
    }

    public int GetMapSelect()
    {
        return mapSelect;
    }
}
