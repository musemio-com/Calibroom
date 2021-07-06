using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MECM;

/// <summary>
/// Manages behaviour of puzzle table
/// </summary>
public class PuzzleBoard : MonoBehaviour
{
    public PuzzlePiece[] Pieces;
    public bool PuzzleComplete;
    [Tooltip("Complete the puzzle immediately. Doesn't trigger data collection")]
    public bool PuzzleCompleteDebug;
    public GameObject PuzzleCompletedFX;
    /// <summary>
    /// Fades to black the scene and loads next level
    /// </summary>
    private UIFadeToBlack m_LevelFader;

    /// <summary>
    /// Handles whether the puzzle should run its logic or not
    /// </summary>
    private bool m_PuzzleStarted;
    /// <summary>
    /// Flag that handles if the puzzle was completed at least once
    /// </summary>
    private bool m_PuzzleCompletedOnce;

    /// <summary>
    /// Used to trigger data collection on/off
    /// </summary>
    private DataCollectionController m_DataCtrlr;

    private void Awake()
    {
        // Get all pieces in that are children to the board
        Pieces = FindObjectsOfType<PuzzlePiece>();
        //m_LevelFader = FindObjectOfType<UIFadeToBlack>();
        // Get data controller
        m_DataCtrlr = FindObjectOfType<DataCollectionController>();
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
                StopPuzzle();
            }
        }
    }

    // Called when there is a change in the inspector
    private void OnValidate()
    {
        if (PuzzleCompleteDebug)
        {
            // Load Level
            //if (m_LevelFader != null)
            //    m_LevelFader.FadeToBlackLoadNextLevelStopDataCollection(debug: true);
            FindObjectOfType<VrSceneTransition>().loadNextScene();

            // Flag the puzzle as completed once to avoid the user accidentally completing it more than once and loading the next level more than once
            m_PuzzleCompletedOnce = true;

        }
    }

    /// <summary>
    /// Flags the puzzle as started
    /// </summary>
    public void StartPuzzle()
    {
        // Start the puzzle only once
        if (!m_PuzzleStarted)
        {
            PuzzleComplete = false;
            // Starts data collection
            if (m_DataCtrlr != null)
                m_DataCtrlr.ToggleCollectingData();

            m_PuzzleStarted = true;
        }

    }

    /// <summary>
    /// Flags the puzzle as completed, loads new level, and stops data collection
    /// </summary>
    public void StopPuzzle()
    {
        // Load Level
        //if (m_LevelFader != null)
        //    m_LevelFader.FadeToBlackLoadNextLevelStopDataCollection();
        if (m_DataCtrlr != null)
            m_DataCtrlr.ToggleCollectingData();

        m_PuzzleCompletedOnce = true;
        PuzzleCompletedFX.SetActive(true);

        FindObjectOfType<VrSceneTransition>().loadNextScene();

        // Flag the puzzle as completed once to avoid the user accidentally completing it more than once and loading the next level more than once

    }
}
