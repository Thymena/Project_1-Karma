using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADDisplayUI : MonoBehaviour
{
    public float delay = 2f;
    public GameObject ad;

    private float timer = 0f;
    private bool done = false;

    public bool isDisplayed = false;

    // Start is called before the first frame update
    void Start()
    {
        ad.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= delay && !done)
        {
            ad.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            ad.SetActive(false);
            isDisplayed = true;
            done = true;
        }
    }
}
