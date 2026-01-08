using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpaceBarDisplayUI : MonoBehaviour
{
    public float delay = 1f;
    public GameObject spaceBar;
    public ADDisplayUI adDisplayUI;

    private float timer = 0f;
    private bool done = false;
   
    // Start is called before the first frame update
    void Start()
    {
        spaceBar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (adDisplayUI.isDisplayed == true)
        {
            timer += Time.deltaTime;
        
            if (timer >= delay && !done)
            {
                spaceBar.SetActive(true);
            }
        
            if (Input.GetKeyDown(KeyCode.Space))
            {
                spaceBar.SetActive(false);
                done = true;
            }
        }
    }
}
