using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the behaviour of the UI Canvas that fades to Black
/// </summary>
public class UIFadeToBlack : MonoBehaviour
{
    private GameObject m_MainCamera;
    /// <summary>
    /// Used to get the loading progress
    /// </summary>
    private GameLevelController m_LevelLoader;
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
        m_LevelLoader = FindObjectOfType<GameLevelController>();
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
                m_LevelLoader.LoadNextLevel();
                m_FadeToBlackAnimator.SetBool(m_LoadNextLevelAnimatorID, false);
            }

        }


    }

    /// <summary>
    /// Triggers the Fade To Black (when that ends, fade to black will set the Loadnextlevel flag to true)
    /// </summary>
    public void FadeToBlackAndLoadNextLevel()
    {
        if (m_FadeToBlackAnimator != null)
        {
            // This trigger will launch the animation
            m_FadeToBlackAnimator.SetTrigger("FadeToBlackTrigger");
            // When the animation is over, the animator LoadNextLevel bool will be set to true by a custom behaviour
            // When animator.LoadNextLevel bool is true, the Update loop will handle that and call LevelLoader.LoadNextLevel()
        }
    }
}
