using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the data of a PuzzlePiece
/// </summary>
public class PuzzlePiece : MonoBehaviour
{
    /// <summary>
    /// The ID of this particular piece
    /// </summary>
    public string ID;


    /// <summary>
    /// Is the piece in correct place?
    /// </summary>
    public bool InPlace;

    /// <summary>
    /// Is the piece on the table?
    /// </summary>
    public bool OnTable;

    private void OnCollisionEnter(Collision collision)
    {
        PuzzleBoard table = collision.gameObject.GetComponent<PuzzleBoard>();
        // If we have collided with the table...
        if (table != null)
        {
            // Set flag to true
            OnTable = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        PuzzleBoard table = collision.gameObject.GetComponent<PuzzleBoard>();
        // If we left the table...
        if (table != null)
        {
            // Set flag to false
            OnTable = false;
        }

    }

}
