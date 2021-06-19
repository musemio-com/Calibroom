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
    [NodeWidth(400)]
    public class MECMTrainingDataSet : TrainingExamplesNode
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
        
        public int NumDataSetsProcessed;
        public bool ProcessingStarted;
        public bool ProcessingFinished;

        #endregion

        #region XNode Messages

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            // Init code for training examples
            Initialize();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return this; // Replace this
        }

        #endregion

        #region TrainingExamples Overrides

        /// <summary>
        /// Sets Data Collection Type 
        /// </summary>
        protected override void SetDataCollection()
        {
            ModeOfCollection = CollectionMode.SingleExample;
        }
        /// <summary>
        /// Save IML Training Data to Disk 
        /// </summary>
        public override void SaveDataToDisk()
        {
            IMLDataSerialization.SaveTrainingSetToDisk(m_TrainingExamplesVector, GetJSONFileName());
        }
        /// <summary>
        /// Returns the file name we want for the regular training examples list in JSON format, both for read and write
        /// </summary>
        /// <returns></returns>
        public override string GetJSONFileName()
        {
            if (this.graph != null)
            {
                string fileName = "MECMTrainingDataSet" + this.id;

                // If we have a subfolder specified for the data...
                if (!String.IsNullOrEmpty(SubFolderDataPath))
                    fileName = String.Concat(SubFolderDataPath, "/", fileName);

                return fileName;
            }
            return null;
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
            MovementData = GetInputValue<List<List<IMLTrainingExample>>>("MovementData");
            BrainHQDataSet = GetInputValue<List<BrainHQUserData>>("BrainHQDataSet");
            Task.Run(async () => 
            {                
                // Load training data
                m_TrainingExamplesVector = new List<IMLTrainingExample>();
                m_TrainingExamplesVector = await DataToWindowsAsync(MovementData, BrainHQDataSet);
                // Force to update dataset configuration for model
                UpdateDesiredInputOutputConfigFromDataVector(updateDesiredFeatures: true);
                // Trigger save
                SaveDataToDisk();
            });

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Chunks a list of trainingDataSets into windows of given size 
        /// Assumes by default that one window is 10 samples (10 samples per sec, 1 window = 1 sec)
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="lengthWindow"></param>
        private async Task<List<IMLTrainingExample>> DataToWindowsAsync(List<List<IMLTrainingExample>> inputData, List<BrainHQUserData> outputData, int lengthWindow = 10)
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
            //if (ProcessingStarted)
            //{
            //    NodeDebug.LogWarning("Data is already processing! Cannot start twice", this);
            //    return null;
            //}

            NumDataSetsProcessed = 0;
            ProcessingStarted = true;
            ProcessingFinished = false;
            // Chunk training Examples in new list
            List<IMLTrainingExample> windowedDataSet = new List<IMLTrainingExample>();

            Debug.Log($"Starting to process data. {inputData.Count} DataSets detected.");
            // Iterate through one dataset at a time
            foreach (List<IMLTrainingExample> trainingExamples in inputData)
            {

                if (trainingExamples == null)
                {
                    NodeDebug.LogWarning("There is a null DataSet in collection! Aborting processing", this);
                    ProcessingStarted = false;
                    return null;
                }

                // Select the BrainHQ dataset that matches the user ID of this dataset
                var brainHQLabel = outputData.First(x => x.ID == trainingExamples[0].Outputs[0].OutputData.Values[0]);

                Debug.Log($"Processing dataset with labels {trainingExamples[0].Outputs[0].OutputData.Values[0]} and ID {brainHQLabel.ID}...");

                int numWindowsCreated = 0;
                // We chunk N examples together in one window (newExampleWindow). Move the index up the size of the window 
                for (int i = 0; i < trainingExamples.Count; i += lengthWindow)
                {
                    IMLTrainingExample newExampleWindow = new IMLTrainingExample();
                    // Get our specific window boundaries (starting from last window last index)
                    int localLengthWindow = i + lengthWindow;
                    // Check that there are enough inputs left for window (in case this is the last window). If not, we skip this window
                    if (localLengthWindow > trainingExamples.Count)
                    {
                        Debug.Log($"Window for dataset with labels {trainingExamples[0].Outputs[0].OutputData.Values[0]} and ID {brainHQLabel.ID} is too big, skipping a total of {localLengthWindow - trainingExamples.Count} examples and moving to next dataSet...");
                        continue;
                    }

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
                            ProcessingStarted = false;
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
                            newExampleWindow.AddOutputExample(new IMLFloat(brainHQLabel.MentalMapScore));
                            newExampleWindow.AddOutputExample(new IMLFloat(brainHQLabel.TargetTracker));
                        }
                    }

                    // Add window to training data set
                    windowedDataSet.Add(newExampleWindow);
                    numWindowsCreated++;
                    Debug.Log($"Added window with ID {brainHQLabel.ID}. Examples in window = {newExampleWindow.Inputs.Count}.");

                }

                Debug.Log($"Processed dataset with ID {brainHQLabel.ID}. Created {numWindowsCreated} windows!");
                NumDataSetsProcessed++;

            }

            ProcessingFinished = true;
            ProcessingStarted = false;
            return windowedDataSet;
        }

        /// <summary>
        /// Coroutine to save data to disk
        /// </summary>
        /// <returns></returns>
        private IEnumerator SaveDataCoroutine()
        {
            // Wait between frames until processing is finished
            while (!ProcessingFinished) yield return null;
            // Save data once processing is finished
            SaveDataToDisk();

        }
        #endregion

    }
}