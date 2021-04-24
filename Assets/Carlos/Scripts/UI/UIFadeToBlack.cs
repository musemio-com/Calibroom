using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MECM;

/// <summary>
/// Manages the behaviour of the UI Canvas that fades to Black
/// </summary>
public class UIFadeToBlack : MonoBehaviour
{
    private GameObject m_MainCamera;
    /// <summary>
    /// Used to get the loading progress
    /// </summary>
    private MECM.MECMLevelController m_LevelLoader;
    /// <summary>
    /// Used to send data to the animator about loading progress
    /// </summary>
    private Animator m_FadeToBlackAnimator;

    /// <summary>
    /// Flag that manages when to load the next level
    /// </summary>
    private int m_LoadNextLevelAnimatorID;

    /// <summary>
    /// Black image to perform fading to/from black animations
    /// </summary>
    private Image m_BlackImage;

    /// <summary>
    /// Used to trigger data collection on/off
    /// </summary>
    private DataCollectionController m_DataCtrlr;

    /// <summary>
    /// Are we in debug mode?
    /// </summary>
    private bool m_Debug;

    private void OnEnable()
    {
        m_MainCamera = null;
        // Try and find the main camera
        m_MainCamera = Camera.main.gameObject;
        
        // If we found a camera...
        if (m_MainCamera != null)
        {
            // Position the UI canvas a bit further away of the main camera so it can be properly visible
            this.transform.SetParent(m_MainCamera.transform);
            this.transform.localPosition = Vector3.forward * 0.35f;

            // Try and find black image
            m_BlackImage = GetComponentInChildren<Image>();
            // Make the blaack image bigger so that it can be seen by main camera (by default it is set to .001)
            if (m_BlackImage != null)
                m_BlackImage.transform.localScale *= 500f;
        }

        // Try and find animator
        m_FadeToBlackAnimator = this.GetComponent<Animator>();
        if (m_FadeToBlackAnimator != null)
        {
            // Get the id of LoadNextLevel bool from the animator 
            m_LoadNextLevelAnimatorID = Animator.StringToHash("LoadNextLevel");
        }

        // Try and find the level loader
        m_LevelLoader = FindObjectOfType<MECM.MECMLevelController>();

        // Get data controller
        m_DataCtrlr = FindObjectOfType<DataCollectionController>();

    }

    private void Update()
    {
        if (m_FadeToBlackAnimator != null && m_LevelLoader != null)
        {
            // Update everyframe the loading progress in the animator controller
            m_FadeToBlackAnimator.SetFloat("LoadingProgress", m_LevelLoader.LoadingProgress);

            // Loadnextlevel flag will be called from the animator behaviour, at the end of the FadeToBlackAnimation
            if (m_FadeToBlackAnimator.GetBool(m_LoadNextLevelAnimatorID))
            {
                // Stop data collection (if in debug mode we should never be collecting data, no need to stop)
                if (m_DataCtrlr != null && m_Debug == false)
                    m_DataCtrlr.FireToggleCollectDataEvent();
                // Load next level
                m_LevelLoader.LoadNextLevel();
                // Stop the animation since we are already in black
                m_FadeToBlackAnimator.SetBool(m_LoadNextLevelAnimatorID, false);

            }

        }


    }

    /// <summary>
    /// Triggers the Fade To Black (when that ends, fade to black will set the Loadnextlevel flag to true)
    /// </summary>
    public void FadeToBlackLoadNextLevelStopDataCollection(bool debug = false)
    {
        if (m_FadeToBlackAnimator != null)
        {
            // Set debug flag
            m_Debug = false;
            // This trigger will launch the animation
            m_FadeToBlackAnimator.SetTrigger("FadeToBlackTrigger");
            // When the animation is over, the animator LoadNextLevel bool will be set to true by a custom behaviour
            // When animator bool is true, the Update loop will handle that and call LevelLoader.LoadNextLevel()
        }
    }
}
