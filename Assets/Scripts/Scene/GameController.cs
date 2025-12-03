using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool pastTest = false;
    public bool startTest = false;
    public GameObject[] gameObjects;
    public CollectibleManager collectibleManager;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in gameObjects)
            obj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (startTest == true)
        {
            foreach (GameObject obj in gameObjects)
                obj.SetActive(true);
            collectibleManager.BeginSpawning();

        }

        if (pastTest == true)
        {
            foreach (GameObject obj in gameObjects)
                obj.SetActive(false);
        }
    }
}
