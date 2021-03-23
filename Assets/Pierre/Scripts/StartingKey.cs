using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manage the behavior of the starting key

public class StartingKey : MonoBehaviour
{
    public bool pieceColor = false;

    /// <summary>
    /// Saved info from original transform
    /// </summary>
    private Vector3 m_OriginalPosition;
    private Quaternion m_OriginalRotation;

    private void Start()
    {
        // Save original transform if piece needs to reset (i.e. out of room limits)
        m_OriginalPosition = transform.position;
        m_OriginalRotation = transform.rotation;
    }


    private void OnCollisionEnter(Collision collision)
    {
        //PuzzleBoard table = collision.gameObject.GetComponent<PuzzleBoard>();
        //// If we have collided with the table...
        //if (table != null)
        //{
        //    // Set flag to true
        //    OnTable = true;
        //}

        DeadLimit deadLimit = collision.gameObject.GetComponent<DeadLimit>();
        // If we collide with a dead limit...
        if (deadLimit != null)
        {
            // Reset piece to original transform
            transform.position = m_OriginalPosition;
            transform.rotation = m_OriginalRotation;
        }
    }//
}
