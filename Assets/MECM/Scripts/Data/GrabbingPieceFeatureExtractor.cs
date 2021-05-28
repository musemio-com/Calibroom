using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MECM
{
    /// <summary>
    /// Extracts the information if this hand is grabbing a piece or not
    /// </summary>
    public class GrabbingPieceFeatureExtractor : MonoBehaviour
    {
        /// <summary>
        /// Is Hand Grabbing a Piece?
        /// </summary>
        public bool GrabbingPiece;

        public enum ControllerType { None, LeftHand, RightHand}
        public ControllerType XRControllerType 
        { 
            get 
            {
                // Attempt to re-check what controller type is this...
                if (m_XRControllerType == ControllerType.None)
                {
                    // Attempt to find local XRController
                    if (m_XRController == null)
                        m_XRController = this.GetComponent<XRController>();

                    if (m_XRController != null)
                    {
                        if (m_XRController.controllerNode == UnityEngine.XR.XRNode.LeftHand)
                        {
                            return m_XRControllerType = ControllerType.LeftHand;
                        }
                        else if (m_XRController.controllerNode == UnityEngine.XR.XRNode.RightHand)
                        {
                            return m_XRControllerType = ControllerType.RightHand;
                        }
                    }
                }                
                return m_XRControllerType;
            }
            set { m_XRControllerType = value; }
        }
        [SerializeField]
        private ControllerType m_XRControllerType;

        private XRController m_XRController;

        /// <summary>
        /// Array containing all xr interactables from scene
        /// </summary>
        private XRGrabInteractable[] m_PieceInteractables;

        /// <summary>
        /// The current piece we are dealing with
        /// </summary>
        private List<XRGrabInteractable> m_PiecesSubscribed;
        /// <summary>
        /// The current piece we are dealing with
        /// </summary>
        private XRBaseInteractor m_CurrentPieceInteractor;

        private void Awake()
        {
            // Attempt to find all interactables
            m_PieceInteractables = FindObjectsOfType<XRGrabInteractable>(includeInactive: true);
            // Attempt to find local XRController
            if (m_XRController == null)
                m_XRController = this.GetComponent<XRController>();
        }

        // Start is called before the first frame update
        void Start()
        {
            // Set what kind of XRController we are extracting grabbing from
            if (m_XRController != null)
            {
                if (m_XRController.controllerNode == UnityEngine.XR.XRNode.LeftHand)
                {
                    m_XRControllerType = ControllerType.LeftHand;
                }
                else if (m_XRController.controllerNode == UnityEngine.XR.XRNode.RightHand)
                {
                    m_XRControllerType = ControllerType.RightHand;
                }
            }
            else
            {
                m_XRControllerType = ControllerType.None;
            }


            // if we have found interactable pieces...
            if (m_PieceInteractables != null && m_PieceInteractables.Length > 0)
            {
                foreach (var pieceInteractable in m_PieceInteractables)
                {
                    if (pieceInteractable != null)
                    {
                        // Add SetGrabbingPiece to be called by the itneractors events
                        // Grabbing flag is true when selecting the piece (by default trigger when grabbing a piece)
                        pieceInteractable.onSelectEnter.AddListener(x => SubscribePiece(pieceInteractable));
                        // Grabbing flag is false when de-selecting the piece (by default trigger when grabbing a piece)
                        pieceInteractable.onSelectExit.AddListener(x => UnSubscribePiece(pieceInteractable));
                    }
                }
            }
        }

        // Called once per frame
        private void Update()
        {
            if (m_PiecesSubscribed != null)
            {
                foreach (var piece in m_PiecesSubscribed)
                {
                    SetGrabbingPiece(piece.selectingInteractor);
                    if (GrabbingPiece == true)
                        return;
                }
            }
        }

        // Unsubscribing from listeners when the scenes unloads
        private void OnDestroy()
        {
            // Unsubscribe events if we have found interactable pieces...
            if (m_PieceInteractables != null && m_PieceInteractables.Length > 0)
            {
                foreach (var pieceInteractable in m_PieceInteractables)
                {
                    if (pieceInteractable != null)
                    {
                        // Add SetGrabbingPiece to be called by the itneractors events
                        // Grabbing flag is true when selecting the piece (by default trigger when grabbing a piece)
                        pieceInteractable.onSelectEnter.RemoveListener(x => SubscribePiece(pieceInteractable));
                        // Grabbing flag is false when de-selecting the piece (by default trigger when grabbing a piece)
                        pieceInteractable.onSelectExit.RemoveListener(x => UnSubscribePiece(pieceInteractable));
                    }
                }
            }

        }

        /// <summary>
        /// Subscribe piece to our private list if possible (used to pull data on update per subscribed piece)
        /// </summary>
        /// <param name="pieceInteractable"></param>
        public void SubscribePiece(XRGrabInteractable pieceInteractable)
        {
            // Make sure to init list
            if (m_PiecesSubscribed == null)
                m_PiecesSubscribed = new List<XRGrabInteractable>();

            // Subscribe piece to our private list if possible (used to pull data on update per subscribed piece)
            if (pieceInteractable != null)
            {
                if (!m_PiecesSubscribed.Contains(pieceInteractable))
                    m_PiecesSubscribed.Add(pieceInteractable);
            }
        }

        /// <summary>
        /// Unsubscribe piece to our private list if possible (used to pull data on update per subscribed piece)
        /// </summary>
        /// <param name="pieceInteractable"></param>
        public void UnSubscribePiece(XRGrabInteractable pieceInteractable)
        {
            // Make sure to init list
            if (m_PiecesSubscribed == null)
                m_PiecesSubscribed = new List<XRGrabInteractable>();

            // Unsubscribe piece to our private list if possible (used to pull data on update per subscribed piece)
            if (pieceInteractable != null)
            {
                if (m_PiecesSubscribed.Contains(pieceInteractable))
                {
                    // First make sure to release grabbing flag if needed
                    if (m_CurrentPieceInteractor == pieceInteractable.selectingInteractor)
                    {
                        // Delete knowledge and release grabbing flag
                        m_CurrentPieceInteractor = null;
                        GrabbingPiece = false;
                    }

                    // Unsubscribe piece
                    m_PiecesSubscribed.Remove(pieceInteractable);
                }
            }

        }


        /// <summary>
        /// Set Grab Piece
        /// </summary>
        /// <param name="grabbing"></param>
        public void SetGrabbingPiece(XRBaseInteractor interactor)
        {
            if (interactor != null)
            {
                var XRControllerGrabbingPiece = interactor.GetComponent<XRController>();
                if (XRControllerGrabbingPiece != null && m_XRController != null)
                {
                    //Debug.Log(XRControllerGrabbingPiece.controllerNode.ToString());

                    // Only set the grabbing if the controller that is interacting with the piece matches with our controller
                    if (XRControllerGrabbingPiece.controllerNode == m_XRController.controllerNode)
                    {
                        GrabbingPiece = true;
                        m_CurrentPieceInteractor = interactor;
                    }
                }
            }
            
        }
    }

}
