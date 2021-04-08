using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorGrab : MonoBehaviour
{
    public Transform m_offset;
    private Transform m_currentHand;

    private void Update()
    {
        if (m_currentHand)
        {
            m_offset.position = m_currentHand.position;
            m_offset.rotation = m_currentHand.rotation;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            // same as m_currentHand != null
            if(!m_currentHand)
            {
                m_currentHand = other.transform;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_currentHand = other.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            m_currentHand = null;
        }
    }
}