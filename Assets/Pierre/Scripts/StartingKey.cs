﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manage the behavior of the starting key
namespace MECM
{
    /// <summary>
    /// Mananges logic for starting key piece
    /// </summary>
    public class StartingKey : MonoBehaviour
    {
        private Vector3 m_OriginalPosition;
        private Quaternion m_OriginalRotation;
        public Puzzle puzzle;
        private float? m_startTimer = null;

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


        private void OnTriggerEnter(Collider other)
        {
            DeadLimit deadLimit = other.gameObject.GetComponent<DeadLimit>();

            // If we collide with a dead limit...
            if (deadLimit != null)
            {
                // Reset piece to original transform
                transform.position = m_OriginalPosition;
                transform.rotation = m_OriginalRotation;
            }

            // If starting Key collide with StartingHole, then puzzleActive is true.
            if (other.gameObject.GetComponent<StartingHole>())
            {
                m_startTimer = 0;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (m_startTimer != null)
            {
                m_startTimer += Time.deltaTime;
                if (m_startTimer > 1)
                {
                    puzzle.PuzzleActive(true);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<StartingHole>())
            {
                m_startTimer = null;
            }
        }
    }
}
