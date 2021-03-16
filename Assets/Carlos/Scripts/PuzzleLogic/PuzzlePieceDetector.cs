using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects whether the correct piece is in place
/// </summary>
public class PuzzlePieceDetector : MonoBehaviour
{
    /// <summary>
    /// Is it forbidden to get in touch with a piece here?
    /// </summary>
    public bool ForbidDetecting;
    
    /// <summary>
    /// Which piece is the detector looking for?
    /// </summary>    
    public List<ExpectedPuzzlePiece> PuzzlePiecesExpected;
    
    /// <summary>
    /// Are all the correct pieces detected?
    /// </summary>
    public bool AllPiecesDetected;

    private void OnTriggerEnter(Collider other)
    {
        // If we can detect pieces...
        if (!ForbidDetecting)
        {
            // If null or empty expected pieces...
            if (PuzzlePiecesExpected == null || PuzzlePiecesExpected.Count == 0)
                // Cancel execution
                return;

            PuzzlePiece piece = other.GetComponent<PuzzlePiece>();
            // If a puzzle piece entered the trigger...
            if (piece != null)
            {
                // Iterate through all the pieces we expect
                    foreach (var expectedPiece in PuzzlePiecesExpected)
                {
                    // Is the ID the same as one of the IDs expected?
                    if (piece.ID == expectedPiece.ID)
                    {
                        // If so, flag it
                        expectedPiece.InPlace = true;                        
                    }
                }

                AllPiecesDetected = true;
                // Iterate again all expected pieces to check if all are in place
                foreach (var expectedPiece in PuzzlePiecesExpected)
                {
                    // If one is not in place, we force false for overall flag
                    if (expectedPiece.InPlace == false)
                        AllPiecesDetected = false;
                }
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        // If we can detect pieces...
        if (!ForbidDetecting)
        {
            // If null or empty expected pieces...
            if (PuzzlePiecesExpected == null || PuzzlePiecesExpected.Count == 0)
                // Cancel execution
                return;

            PuzzlePiece piece = other.GetComponent<PuzzlePiece>();
            // If a puzzle piece entered the trigger...
            if (piece != null)
            {
                // Iterate through all the pieces we expect
                foreach (var expectedPiece in PuzzlePiecesExpected)
                {
                    // Is the ID the same as one of the IDs expected?
                    if (piece.ID == expectedPiece.ID)
                    {
                        // If so, remove flag
                        expectedPiece.InPlace = false;
                        // since at least one piece is not in place, the overall flag can't be true
                        AllPiecesDetected = false;
                    }
                }
            }
        }

    }
}
