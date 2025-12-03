using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryController : MonoBehaviour
{
    public GameObject[] berries;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in berries)
            obj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
