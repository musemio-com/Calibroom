using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using InteractML;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Linq;

namespace MECM
{
    /// <summary>
    /// Holds a Training Data Set for MECM purposes
    /// </summary>
    [NodeWidth(300)]
    public class MECMTrainingDataSet : IMLNode
    {
        #region Variables

        /// <summary>
        /// Movement training examples
        /// </summary>
        [Input]
        public List<List<IMLTrainingExample>> MovementData;

        /// <summary>
        /// BrainHQData to use as labels per input
        /// </summary>
        [Input]
        public List<BrainHQUserData> BrainHQDataSet;
        
        public int DataSetsProcessed;
        private bool m_ProcessingStarted;
        private bool m_ProcessingFinished;

        #endregion

        #region XNode Messages

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Chunks a list of trainingDataSets + BrainHQData into windows of given size 
        /// Assumes by default that one window is 10 samples (10 samples per sec, 1 window = 1 sec)
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="lengthWindow"></param>
        public void DataToWindows()
        {
            Task.Run(async () => await DataToWindowsAsync(MovementData, BrainHQDataSet) );
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Chunks a list of trainingDataSets into windows of given size 
        /// Assumes by default that one window is 10 samples (10 samples per sec, 1 window = 1 sec)
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="lengthWindow"></param>
        private async Task<List<List<IMLTrainingExample>>> DataToWindowsAsync(List<List<IMLTrainingExample>> inputData, List<BrainHQUserData> outputData, int lengthWindow = 10)
        {
            if (inputData == null)
            {
                NodeDebug.LogWarning("Input Data is null!", this);
                return null;
            }
            if (outputData == null)
            {
                NodeDebug.LogWarning("Output Data is null!", this);
                return null;
            }
            if (m_ProcessingStarted)
            {
                NodeDebug.LogWarning("Data is already processing! Cannot start twice", this);
                return null;
            }

            DataSetsProcessed = 0;
            m_ProcessingStarted = true;
            m_ProcessingFinished = false;
            List<List<IMLTrainingExample>> windowedData = new List<List<IMLTrainingExample>>();
            // Iterate through one dataset at a time
            foreach (List<IMLTrainingExample> trainingExamples in inputData)
            {
                if (trainingExamples == null)
                {
                    NodeDebug.LogWarning("There is a null DataSet in collection! Aborting processing", this);
                    m_ProcessingStarted = false;
                    return null;
                }

                // Select the BrainHQ dataset that matches the user ID of this dataset
                var brainHQLabel = outputData.First(x => x.ID == trainingExamples[0].Outputs[0].OutputData.Values[0]);
                
                // Chunk training Examples in new list
                List<IMLTrainingExample> windowedDataSet = new List<IMLTrainingExample>();
                
                // We chunk N examples together in one window (newExampleWindow). Move the index up the size of the window 
                for (int i = 0; i < trainingExamples.Count; i += lengthWindow)
                {
                    IMLTrainingExample newExampleWindow = new IMLTrainingExample();
                    // Get our specific window boundaries (starting from last window last index)
                    int localLengthWindow = i + lengthWindow;
                    // Check that there are enough inputs left for window (in case this is the last window). If not, we skip this window
                    if (localLengthWindow > trainingExamples.Count)
                        break;

                    // Add examples to window (start on last entry of dataset list)
                    for (int j = i; j < localLengthWindow; j++)
                    {
                        // Add example input data to window
                        var example = trainingExamples[j];
                        if (example != null)
                        {
                            foreach (var input in example.Inputs)
                            {
                                newExampleWindow.AddInputExample(input.InputData);
                            }
                        }
                        else
                        {
                            NodeDebug.LogWarning("Null training example in DataSet! Aborting processing", this);
                            m_ProcessingStarted = false;
                            return null;

                        }

                        // If it is the last input added to window...
                        if (j == localLengthWindow - 1)
                        {
                            // Add the BrainHQ data as the output for the entire window
                            // User ID
                            newExampleWindow.AddOutputExample(new IMLInteger(brainHQLabel.ID));
                            // Speed/Processing Task //   
                            newExampleWindow.AddOutputExample(new IMLFloat(brainHQLabel.DoubleDecisionScore));
                            newExampleWindow.AddOutputExample(new IMLFloat(brainHQLabel.MindBenderScore));
                            // Visuo-Spatial Processing //
                            newExampleWindow.AddOutputExample(new IMLFloat(brainHQLabel.RightTurnScore));
                            newExampleWindow.AddOutputExample(new IMLInteger(brainHQLabel.MentalMapScore));
                            newExampleWindow.AddOutputExample(new IMLFloat(brainHQLabel.TargetTracker));
                        }
                    }

                    // Add window to training data set
                    windowedDataSet.Add(newExampleWindow);
                    DataSetsProcessed++;
                }

                // Add windowed dataSet to collection of dataSets
                windowedData.Add(windowedDataSet);
            }

            m_ProcessingFinished = true;
            m_ProcessingStarted = false;
            return windowedData;
        }

        #endregion

    }
}