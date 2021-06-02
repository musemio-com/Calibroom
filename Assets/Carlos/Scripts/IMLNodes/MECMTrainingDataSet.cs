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

        #region Private Methods

        /// <summary>
        /// Chunks a list of trainingDataSets into windows of given size 
        /// Assumes by default that one window is 10 samples (10 samples per sec, 1 window = 1 sec)
        /// </summary>
        /// <param name="inputData"></param>
        /// <param name="lengthWindow"></param>
        private void DataToWindows(List<List<IMLTrainingExample>> inputData, List<BrainHQUserData> outputData, int lengthWindow = 10)
        {
            if (inputData == null)
                return;

            List<List<IMLTrainingExample>> windowedData = new List<List<IMLTrainingExample>>();

            // Iterate through one dataset at a time
            foreach (List<IMLTrainingExample> trainingExamples in inputData)
            {
                // Select the BrainHQ dataset that matches the user ID of this dataset
                var brainHQLabel = outputData.First(x => x.ID == trainingExamples[0].Outputs[0].OutputData.Values[0]);
                

                // TODO CREATE MORE WINDOWS!!!

                // Chunk training Examples in new list
                int index = 0; // TODO we need to have a for loop to increase this index. RIGHT NOW WE ONLY MAKE 1 WINDOW!!
                List<IMLTrainingExample> windowedDataSet = new List<IMLTrainingExample>();
                IMLTrainingExample newExampleWindow = new IMLTrainingExample();
                
                // Add examples to window
                for (int i = 0; i < lengthWindow; i++)
                {
                    // Add example input data to window
                    var example = trainingExamples[i];
                    if (example != null) 
                    {
                        foreach (var input in example.Inputs)
                        {
                            newExampleWindow.AddInputExample(input.InputData);
                        }
                    }
                    // If it is the last input added to window...
                    if (i == lengthWindow - 1)
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
            }
        }

        #endregion

    }
}