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
        // Iterate over list of sensors
        if (sensors != null && sensors.Count > 0)
        {
            foreach (var sensor in m_Sensors)
            {
                // VERY RELAXED CONDITION: with at least one sensor reporting OK we consider the piece in place
                if (sensor.AllPiecesSensed)
                {
                    inPlace = true;
                    // Stop searching as this is a relaxed condition
                    return inPlace;
                }
            }
        }
        // Returns false if we reach here
        return inPlace;
    }
}
