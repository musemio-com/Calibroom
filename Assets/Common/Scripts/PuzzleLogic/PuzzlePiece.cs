using System;
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
    public bool InPlace { get { return m_InPlace = CheckInPlace(m_Sensors); } }
    [SerializeField]
    private bool m_InPlace;

    /// <summary>
    /// Is the piece on the table?
    /// </summary>
    public bool OnTable;

    /// <summary>
    /// List of sensors from the piece
    /// </summary>
    private List<PuzzlePieceSensor> m_Sensors;

    /// <summary>
    /// Saved info from original transform
    /// </summary>
    private Vector3 m_OriginalPosition;
    private Quaternion m_OriginalRotation;

    public AudioSource playSound;


    private void Start()
    {
        // Save original transform if piece needs to reset (i.e. out of room limits)
        m_OriginalPosition = transform.position;
        m_OriginalRotation = transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        PuzzleBoard table = collision.gameObject.GetComponent<PuzzleBoard>();
        // If we have collided with the table...
        if (table != null)
        {
            // Set flag to true
            OnTable = true;
        }

        DeadLimit deadLimit = collision.gameObject.GetComponent<DeadLimit>();
        // If we collide with a dead limit...
        if (deadLimit != null)
        {
            // Reset piece to original transform
            transform.position = m_OriginalPosition;
            transform.rotation = m_OriginalRotation;
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

    /// <summary>
    /// Subscribe sensor to internal list
    /// </summary>
    /// <param name="sensor"></param>
    public void SubscribeSensor(PuzzlePieceSensor sensor)
    {
        if (m_Sensors == null)
            m_Sensors = new List<PuzzlePieceSensor>();

        if (!m_Sensors.Contains(sensor))
            m_Sensors.Add(sensor);
    }

    /// <summary>
    /// Checks if the piece is in place
    /// </summary>
    private bool CheckInPlace(List<PuzzlePieceSensor> sensors)
    {
        
        bool inPlace = false;
        int numberOfPiecesWanted = 2;
        int numberOfPiecesSensed = 0;

        // Iterate over list of sensors
        if (sensors != null && sensors.Count > 0)
        {
            foreach (var sensor in m_Sensors)
            {
                // VERY RELAXED CONDITION: with at least one sensor reporting OK we consider the piece in place
                if (sensor.ObjectivesSensed)
                {
                    // We increase by 1 the number of pieces 
                    numberOfPiecesSensed++;

                }

                // If we have reached two sensors reporting OK, then we stop looking and consider the piece in place
                if (numberOfPiecesSensed == numberOfPiecesWanted)
                {
                    inPlace = true;

                
                    // Stop execution to save computation time
                    return inPlace;         
                }
            }
        }
        // Returns false if we reach here
        return inPlace;
       
    }
}
