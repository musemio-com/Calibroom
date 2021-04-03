using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorGrab : MonoBehaviour
{
    public Transform m_offset;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            m_offset.position = other.transform.position;
            m_offset.rotation = other.transform.rotation;
        }
    }
}
