using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manage the behavior of the starting key

public class StartingKey : MonoBehaviour
{
    private Vector3 m_OriginalPosition;
    private Quaternion m_OriginalRotation;
    public Puzzle puzzle;

    private void Awake()
    {
        puzzle = FindObjectOfType<Puzzle>();
    }

    private void Start()
    {
        // Save original transform if piece needs to reset (i.e. out of room limits)
        m_OriginalPosition = transform.position;
        m_OriginalRotation = transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        DeadLimit deadLimit = collision.gameObject.GetComponent<DeadLimit>();
        // If we collide with a dead limit...
        if (deadLimit != null)
        {
            // Reset piece to original transform
            transform.position = m_OriginalPosition;
            transform.rotation = m_OriginalRotation;
        }

        // If starting Key collide with StartingHole, then puzzleActive is true.
        StartingHole hole = collision.gameObject.GetComponent<StartingHole>();  
        if (hole != null)
        {
            puzzle.PuzzleActive(true);
        //    Debug.Log("collision detected");
        }
    }
}
