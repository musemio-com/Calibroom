using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
using XNode;
using System.Linq;

namespace MECM
{
    /// <summary>
    /// Averages a list of model outputs into one 
    /// </summary>
    [NodeWidth(400)]
    public class MECMAverageModelOutputs : IMLNode, IUpdatableIML
    {
        #region Variables

        /// <summary>
        /// Used to know how many entries there are in a processed dataset
        /// </summary>
        [Input]
        public List<IMLTrainingExample> ProcessedMovementData;

        [Input]
        public float VisuoSpatialScoreIn;

        [Input]
        public float SpeedProcessingScoreIn;

        /// <summary>
        /// Is the model meant to be running?
        /// </summary>
        [Input]
        public bool StartProcessing;
        
        [Output]
        public float VisuoSpatialScoreOut;

        [Output]
        public float SpeedProcessingOut;

        public bool isExternallyUpdatable => true;

        public bool isUpdated { get ; set; }

        [Output]
        public bool ProcessingFinished;

        /// <summary>
        /// Max num data windows from processed input data (to know when to stop)
        /// </summary>
        private int NumDataWindows;
        /// <summary>
        /// Current index of the input data (to know where the processing is). Increment one a frame.
        /// </summary>
        private int CurrentWindow;

        /// <summary>
        /// Memory of scores in
        /// </summary>
        private List<float> visuoSpatialIns;
        /// <summary>
        /// Memory of scores in
        /// </summary>
        private List<float> speedProcessingIns;

        #endregion

        #region XNode

        public override object GetValue(NodePort port)
        {
            // Out ports
            if (port.Equals(GetOutputPort("ProcessingFinished"))) return ProcessingFinished;
            if (port.Equals(GetOutputPort("VisuoSpatialScoreOut"))) return VisuoSpatialScoreOut;
            if (port.Equals(GetOutputPort("SpeedProcessingOut"))) return SpeedProcessingOut;
            return base.GetValue(port);
        }

        #endregion

        #region IUpdatable

        public void Update()
        {
            // Pull model running
            if (!StartProcessing) StartProcessing = GetInputValue<bool>("StartProcessing");

            // Average score when requested
            if (StartProcessing)
            {
                //Debug.Log("Processing Average Model Score...");
                // Pull processed movement data if needed
                if (ProcessedMovementData == null || ProcessedMovementData.Count == 0) 
                    ProcessedMovementData = GetInputValue<List<IMLTrainingExample>>("ProcessedMovementData");

                // If we have processed movement data
                if (ProcessedMovementData != null && ProcessedMovementData.Count > 0)
                {
                    // Pull model scores that are being output
                    VisuoSpatialScoreIn = GetInputValue<float>("VisuoSpatialScoreIn");
                    SpeedProcessingScoreIn = GetInputValue<float>("SpeedProcessingScoreIn");
                    
                    // set max for counter
                    NumDataWindows = ProcessedMovementData.Count;

                    // init lists if needed
                    if (visuoSpatialIns == null) visuoSpatialIns = new List<float>();
                    if (speedProcessingIns == null) speedProcessingIns = new List<float>();

                    // Add one score entry per frame
                    if (CurrentWindow < NumDataWindows)
                    {                        
                        visuoSpatialIns.Add(VisuoSpatialScoreIn);
                        speedProcessingIns.Add(SpeedProcessingScoreIn);
                        CurrentWindow++;
                    }
                    // All data windows inferred by model
                    else
                    {
                        // Average
                        AverageScore(NumDataWindows, visuoSpatialIns, speedProcessingIns, out VisuoSpatialScoreOut, out SpeedProcessingOut);
                        // Clear lists 
                        visuoSpatialIns.Clear();
                        speedProcessingIns.Clear();
                        // Update processing finished flag
                        ProcessingFinished = true;
                        // reset flag to allow re process if needed
                        StartProcessing = false;
                        Debug.Log($"Finished averaging model! Avg VisuoSpatial is {VisuoSpatialScoreOut} and Avg SpeedProcessing is {SpeedProcessingOut}");
                    }

                }
            }
            // Reset counters when model stops running
            else
            {
                NumDataWindows = 0;
                CurrentWindow = 0;
            }
        }

        public void LateUpdate()
        {
            // Only allow for a frame, emulating event
            if (ProcessingFinished)
            {
                ProcessingFinished = false;

                // Forcing to stop model if it hasn't already
                if ((graph as IMLGraph).IsGraphRunning)
                {
                    var imlGraph = (graph as IMLGraph);
                    var mlComponent = imlGraph.SceneComponent;
                    if (mlComponent.MLSystemNodeList != null)
                    {
                        foreach (var mlsystem in mlComponent.MLSystemNodeList)
                        {
                            if (mlsystem.Running) mlsystem.ToggleRunning();
                        }

                    }
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Averages brainHQ score
        /// </summary>
        /// <param name="movementData"></param>
        /// <param name="visuoSpatials"></param>
        /// <param name="speedProcessings"></param>
        /// <param name="AvgVisuoSpatial"></param>
        /// <param name="AvgSpeedProcessing"></param>
        private void AverageScore(int maxEntries, List<float> visuoSpatials, List<float> speedProcessings, out float AvgVisuoSpatial, out float AvgSpeedProcessing)
        {
            AvgVisuoSpatial = 0;
            AvgSpeedProcessing = 0;
            //if (visuoSpatials.Count != maxEntries || speedProcessings.Count != maxEntries)
            //{
            //    Debug.LogError("Can't average score! Counting is wrong.");
            //    return;
            //}

            // If all is good, calculate averages
            AvgVisuoSpatial = visuoSpatials.Average();
            AvgSpeedProcessing = speedProcessings.Average();

        }

        #endregion
    }

}
