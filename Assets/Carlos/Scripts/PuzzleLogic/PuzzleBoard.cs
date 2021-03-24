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
    /// <summary>
    /// Fades to black the scene and loads next level
    /// </summary>
    private UIFadeToBlack m_LevelFader;
    /// <summary>
    /// Flag that handles if the puzzle was completed at least once
    /// </summary>
    private bool m_PuzzleCompletedOnce;

    private void Awake()
    {
        // Get all pieces in that are children to the board
        Pieces = FindObjectsOfType<PuzzlePiece>();
        m_LevelFader = FindObjectOfType<UIFadeToBlack>();
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
                if (m_LevelFader != null)
                    m_LevelFader.FadeToBlackAndLoadNextLevel();

                // Flag the puzzle as completed once to avoid the user accidentally completing it more than once and loading the next level more than once
                m_PuzzleCompletedOnce = true;
            }
        }
    }
}
