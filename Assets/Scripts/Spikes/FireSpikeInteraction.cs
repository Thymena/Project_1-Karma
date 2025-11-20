using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpikeInteraction : MonoBehaviour
{
    public FireMove onFire;
    public IceMove onIce;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (onFire.isHitSpikes == true)
        {
            this.tag = "Floor";
            Debug.Log("I hit spikes");
        }

        if (onIce.isHitSpikes == true)
        {
            this.tag = "Ice";
        }
    }
}
