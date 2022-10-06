using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickParticleManager : MonoBehaviour
{
    public Action relaseParticle;
    private void OnEnable()
    {
        Invoke("Release", 1f);
    }

    void Release()
    { 
        relaseParticle();
    }


}
