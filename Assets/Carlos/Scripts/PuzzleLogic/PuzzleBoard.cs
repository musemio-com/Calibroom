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

    private void Awake()
    {
        // Get all pieces in that are children to the board
        Pieces = FindObjectsOfType<PuzzlePiece>();
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
        }
    }
}
