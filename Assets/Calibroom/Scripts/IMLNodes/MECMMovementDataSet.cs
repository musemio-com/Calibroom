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
    /// Holds a Movement Data Set (in windows of N size)
    /// </summary>
    [NodeWidth(400)]
    public class MECMMovementDataSet : IMLNode, IDataSetIML, IFeatureIML
    {
        #region Variables

        /// <summary>
        /// Movement training examples
        /// </summary>
        [Input]
        public List<List<IMLTrainingExample>> MovementData;

        public List<IMLTrainingExample> TrainingExamplesVector { get { return m_TrainingExamplesVector; } }
        private List<IMLTrainingExample> m_TrainingExamplesVector;

        public List<IMLTrainingSeries> TrainingSeriesCollection => throw new NotImplementedException();

        public IMLBaseDataType FeatureValues { get { return m_FeatureValues; } }
        [Output, SerializeField]
        private IMLBaseDataType m_FeatureValues;

        public bool isExternallyUpdatable;

        public bool isUpdated { get; set; }

        bool IFeatureIML.isExternallyUpdatable { get; }

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

        public override void Initialize()
        {
            base.Initialize();
            // Attempt to load saved data
            m_TrainingExamplesVector = IMLDataSerialization.LoadTrainingSetFromDisk(GetJSONFileName());
            // Add all required dynamic ports
            // ToggleProcessData           
            this.GetOrCreateDynamicPort("ProcessDataPort", typeof(bool), NodePort.IO.Input);
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {            
            return UpdateFeature(); 
        }

        #endregion


        #region IFeatureIML Method Overrides

        public object UpdateFeature()
        {
            // Pull inputs from bool event nodeports
            if (GetInputValue<bool>("ProcessDataPort")) DataToWindows();
            
            // Attempt to output feature values from processed data
            if (m_TrainingExamplesVector != null && m_TrainingExamplesVector.Count > 0)
            {
                // Pick random window to output
                System.Random rnd = new System.Random();
                int windowIndex = rnd.Next(0, m_TrainingExamplesVector.Count-1 );
                
                // Add all data from window into flattened list
                if (m_TrainingExamplesVector[windowIndex].Inputs != null)
                {
                    List<float> flattenedInputs = new List<float>();
                    foreach (var input in m_TrainingExamplesVector[windowIndex].Inputs)
                    {
                        if (input == null || input.InputData == null)
                        {
                            NodeDebug.LogWarning("Null entry in dataset! It can't output a feature", this, true);
                            return null;

                        }
                        foreach (var value in input.InputData.Values)
                        {
                            flattenedInputs.Add(value);
                        }
                    }

                    // Output flattened array of all values in window
                    m_FeatureValues = new IMLArray(flattenedInputs.ToArray());                    
                    return this;
                }
                else
                {
                    NodeDebug.LogWarning("Null entry in dataset! It can't output a feature", this, true);
                    return null;
                }
            }
            else
            {
                NodeDebug.LogWarning("Data is not processed! It can't output a feature", this, true);
                return null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Chunks a list of trainingDataSets into windows of given size 
        /// Assumes by default that one window is 10 samples (10 samples per sec, 1 window = 1 sec)
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="lengthWindow"></param>
        public void DataToWindows()
        {
            MovementData = GetInputValue<List<List<IMLTrainingExample>>>("MovementData");
            Task.Run(async () =>
            {
                // Load training data
                m_TrainingExamplesVector = new List<IMLTrainingExample>();
                m_TrainingExamplesVector = await DataToWindowsAsync(MovementData);
                // Trigger saving data
                IMLDataSerialization.SaveTrainingSetToDisk(m_TrainingExamplesVector, GetJSONFileName());
            });
        }

        /// <summary>
        /// Returns the file name we want for the regular training examples list in JSON format, both for read and write
        /// </summary>
        /// <returns></returns>
        public virtual string GetJSONFileName(string subFolderDataPath = "")
        {
            if (this.graph != null)
            {
                string fileName = "MECMMovementDataSet" + this.id;

                // If we have a subfolder specified for the data...
                if (!String.IsNullOrEmpty(subFolderDataPath))
                    fileName = String.Concat(subFolderDataPath, "/", fileName);

                return fileName;
            }
            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Chunks a list of trainingDataSets into windows of given size 
        /// Assumes by default that one window is 10 samples (10 samples per sec, 1 window = 1 sec)
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="lengthWindow"></param>
        private async Task<List<IMLTrainingExample>> DataToWindowsAsync(List<List<IMLTrainingExample>> inputData, int lengthWindow = 10)
        {
            if (inputData == null)
            {
                NodeDebug.LogWarning("Input Data is null!", this);
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

                Debug.Log($"Processing dataset with labels {trainingExamples[0].Outputs[0].OutputData.Values[0]}...");

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
                        Debug.Log($"Window for dataset with labels {trainingExamples[0].Outputs[0].OutputData.Values[0]} is too big, skipping a total of {localLengthWindow - trainingExamples.Count} examples and moving to next dataSet...");
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
                        if (example != null && j == localLengthWindow - 1)
                        {
                            // Add the output data as the output for the entire window
                            foreach (var output in example.Outputs)
                            {
                                newExampleWindow.AddOutputExample(output.OutputData);
                            }

                        }
                    }

                    // Add window to training data set
                    windowedDataSet.Add(newExampleWindow);
                    numWindowsCreated++;
                    Debug.Log($"Added window. Examples in window = {newExampleWindow.Inputs.Count}.");

                }

                Debug.Log($"Processed dataset. Created {numWindowsCreated} windows!");
                NumDataSetsProcessed++;

            }

            ProcessingFinished = true;
            ProcessingStarted = false;
            return windowedDataSet;
        }

        #endregion

    }

}
