using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject m_target;

    private Vector3 m_offset;
    // Start is called before the first frame update
    void Start()
    {
        m_offset = new Vector3(0f, 0f, -10f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_target.transform.position + m_offset;
    }
}
