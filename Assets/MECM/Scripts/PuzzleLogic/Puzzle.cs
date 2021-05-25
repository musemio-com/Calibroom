using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MECM
{
    public class Puzzle : MonoBehaviour
    {
        /// <summary>
        /// Used to let the puzzle board when the puzzle should start
        /// </summary>
        private PuzzleBoard m_Board;

        private void Awake()
        {
            m_Board = FindObjectOfType<PuzzleBoard>();
        }

        // Start is called before the first frame update
        void Start()
        {
            gameObject.SetActive(false);
        }

        // learning if the startingHole has been activated.
        public void PuzzleActive(bool activated)
        {
            // if puzzleactive is true then puzzle is active
            if (activated == true)
            {
                // Debug.Log("puzzle is activated");
                gameObject.SetActive(true);
                m_Board.StartPuzzle();
            }
        }
    }
}
