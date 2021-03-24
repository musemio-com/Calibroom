using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages behaviour of puzzle table
/// </summary>
public class PuzzleBoard : MonoBehaviour
{
    public PuzzlePiece[] Pieces;
    public bool PuzzleComplete;
    private GameLevelController m_LevelLoader;
    /// <summary>
    /// Flag that handles if the puzzle was completed at least once
    /// </summary>
    private bool m_PuzzleCompletedOnce;

    private void Awake()
    {
        // Get all pieces in that are children to the board
        Pieces = FindObjectsOfType<PuzzlePiece>();
        m_LevelLoader = FindObjectOfType<GameLevelController>();
    }

    private void Update()
    {
        // Iterates over pieces data to understand if they are in place
        if (Pieces!=null && Pieces.Length > 0)
        {
            PuzzleComplete = true;
            foreach (var piece in Pieces)
            {
                // If only one piece is not in place...
                if (!piece.InPlace) 
                { 
                    PuzzleComplete = false;
                    // Stop searching to save computation time, one false is enough
                    return;
                }                    
            }

            // If the puzzle is complete and the puzzle hasn't been completed before...
            if (PuzzleComplete && !m_PuzzleCompletedOnce)
            {
                // Load Level
                if (m_LevelLoader != null)
                    m_LevelLoader.LoadNextLevel();

                // Flag the puzzle as completed once to avoid the user accidentally completing it more than once and loading the next level more than once
                m_PuzzleCompletedOnce = true;
            }
        }
    }
}
