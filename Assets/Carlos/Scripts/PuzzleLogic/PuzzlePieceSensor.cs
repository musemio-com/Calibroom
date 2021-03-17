using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Senses whether a correct piece is in place
/// </summary>
public class PuzzlePieceSensor : MonoBehaviour
{
    public enum SensingType { PuzzlePiece, PuzzleBoard };
    /// <summary>
    /// Is the sensor looking for pieces or for the board?
    /// </summary>
    [Tooltip("Specified what is the sensor looking for.")]
    public SensingType SensingMode;

    /// <summary>
    /// Is it forbidden to sense a piece here?
    /// </summary>
    public bool StopSensing;

    /// <summary>
    /// Are all the correct pieces detected?
    /// </summary>
    public bool AllPiecesSensed;

    /// <summary>
    /// Which piece is the detector looking for?
    /// </summary>    
    public List<ExpectedPuzzlePiece> PuzzlePiecesExpected;

    private void Awake()
    {
        // Subscribe to parent puzzle piece (if available)
        PuzzlePiece parentPiece = GetComponentInParent<PuzzlePiece>();
        if (parentPiece != null)
            parentPiece.SubscribeSensor(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If we can detect pieces...
        if (!StopSensing)
        {
            switch (SensingMode)
            {
                // If the sensor is looking for pieces...
                case SensingType.PuzzlePiece:
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

                        AllPiecesSensed = true;
                        // Iterate again all expected pieces to check if all are in place
                        foreach (var expectedPiece in PuzzlePiecesExpected)
                        {
                            // If one is not in place, we force false for overall flag
                            if (expectedPiece.InPlace == false)
                                AllPiecesSensed = false;
                        }

                    }
                    break;
                // If the sensor is looking for the board...
                case SensingType.PuzzleBoard:
                    PuzzleBoard board = other.GetComponent<PuzzleBoard>();
                    // If the puzzle board entered the trigger...
                    if (board != null)
                    {
                        AllPiecesSensed = true;
                    }
                    break;
                default:
                    break;
            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
        // If we can detect pieces...
        if (!StopSensing)
        {
            switch (SensingMode)
            {
                // If the sensor is looking for pieces...
                case SensingType.PuzzlePiece:
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
                                AllPiecesSensed = false;
                            }
                        }
                    }
                    break;
                // If the sensor is looking for the board...
                case SensingType.PuzzleBoard:
                    PuzzleBoard board = other.GetComponent<PuzzleBoard>();
                    // If the puzzle board entered the trigger...
                    if (board != null)
                    {
                        AllPiecesSensed = false;
                    }
                    break;
                default:
                    break;
            }

        }

    }
}
