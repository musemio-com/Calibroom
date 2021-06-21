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
    /// Holds multiple BrainHQ files
    /// </summary>
    [NodeWidth(300)]

    public class BrainHQDataSetNode : IMLNode, IUpdatableIML
    {

        #region Variables

        /// <summary>
        /// Data set containing BrainHQ info per user
        /// </summary>
        [Output]
        public List<BrainHQUserData> BrainHQDataSet;

        /// <summary>
        /// Path of folder to search 
        /// </summary>
        public string FolderPath;

        // Flags for loading
        public bool LoadingStarted { get { return m_LoadingStarted; } }
        [System.NonSerialized]
        private bool m_LoadingStarted;
        public bool LoadingFinished { get { return m_LoadingFinished; } }

        [System.NonSerialized]
        private bool m_LoadingFinished;

        public bool isExternallyUpdatable => true;

        public bool isUpdated { get => m_isUpdated; set => m_isUpdated = value; }
        private bool m_isUpdated;


        #endregion

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

            // Add all required dynamic ports
            // ToggleProcessData           
            this.GetOrCreateDynamicPort("LoadDataPort", typeof(bool), NodePort.IO.Input);

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return BrainHQDataSet;
        }

        #region Public Methods

        /// <summary>
        /// Loads several BrainHQ files from a folder
        /// </summary>
        /// <param name="path">path of file to load</param>
        /// <param name="specificID">When loading, are we looking only for a specific nodeID in file(s)?</param>        
        public void LoadDataSets(string path)
        {
            if (m_LoadingStarted)
            {
                NodeDebug.LogWarning("Can't start loading when there is a loading in progress...", this);
                return;
            }
            if (Directory.Exists(path))
            {
                m_LoadingStarted = true;
                m_LoadingFinished = false;

                // init data set list
                if (BrainHQDataSet == null)
                    BrainHQDataSet = new List<BrainHQUserData>();
                else
                    BrainHQDataSet.Clear();


                Task.Run(async () =>
                {
                    // First, find all the folders
                    // Iterate to upload all files in folder, including subdirectories
                    string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    Debug.Log($"{files.Length + 1} files found. Loading BrainHQ data sets, please wait...");

                    foreach (var file in files)
                    {
                        // If there is a csv file, attempt to load
                        if (Path.GetExtension(file) == ".csv")
                        {
                            // The entire CSV in one line
                            string line = "";
                            // File ID
                            string fileName = Path.GetFileNameWithoutExtension(file);
                            string userID = new String(fileName.Where(Char.IsDigit).ToArray());

                            // Async read and extracting relevant brainhq entries from file
                            using (var reader = File.OpenText(file))
                            {
                                Debug.Log($"Opening file {fileName}");
                                line = await reader.ReadToEndAsync();
                                // Do something with fileText...
                                if (!string.IsNullOrEmpty(line))
                                {
                                    var rows = line.Split('\n');

                                    // BrainHQ vars
                                    string doubleDecision = "";
                                    string mindBender = "";
                                    string rightTurn = "";
                                    string mentalMap = "";
                                    string targetTracker = "";

                                    // Extract the desired values from each column
                                    foreach (var row in rows)
                                    {
                                        if (!string.IsNullOrEmpty(row))
                                        {
                                            // Speed/Processing Task //
                                            // Double Decision
                                            if (row.Contains("Double Decision"))
                                                doubleDecision = GetValueFromRow(row, 5);

                                            // Mind Bender
                                            if (row.Contains("Mind Bender"))
                                                mindBender = GetValueFromRow(row, 5);

                                            // Visuo-Spatial Processing //
                                            // Right Turn
                                            if (row.Contains("Right Turn"))
                                                rightTurn = GetValueFromRow(row, 5);

                                            // Mental Map
                                            if (row.Contains("Mental Map"))
                                                mentalMap = GetValueFromRow(row, 5);

                                            // Target Tracker
                                            if (row.Contains("Target Tracker"))
                                                targetTracker = GetValueFromRow(row, 5);

                                        }
                                    }

                                    // Store brainHQ user data
                                    BrainHQUserData data = new BrainHQUserData(int.Parse(userID),
                                        float.Parse(doubleDecision), float.Parse(mindBender),
                                        float.Parse(rightTurn), float.Parse(mentalMap), float.Parse(targetTracker));

                                    // Add to list if not null
                                    if (BrainHQDataSet != null)
                                    {
                                        BrainHQDataSet.Add(data);
                                        Debug.Log($"Added brainHQ data to list {fileName}");

                                    }

                                }
                                // close file
                                reader.Close();
                                Debug.Log($"Closing file {fileName}");

                            }

                        }

                    }

                    if (BrainHQDataSet.Count == 0)
                    {
                        NodeDebug.LogWarning("Couldn't load folder!", this);

                    }
                    else
                    {
                        m_LoadingFinished = true;
                        m_LoadingStarted = false; // allow to re-load if user wants to
                        Debug.Log($"{BrainHQDataSet.Count + 1} BrainHQ Files Loaded!");
                    }

                });

            }
            else
            {
                NodeDebug.LogWarning("The folder doesn't exist!", this);
            }
        }



        #endregion

        #region Private Methods

        /// <summary>
        /// Extracts a value from a CSV row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetValueFromRow(string row, int index) 
        {            
            // Get threshold value
            string[] entries = row.Split(',');
            string value = null;
            if (entries.Length > index && !string.IsNullOrEmpty(entries[index]))
            {
                value = entries[index];
            }

            return value;
        }

        #endregion

        #region IUpdatableIML

        public void Update()
        {
            // Pull inputs from bool event nodeports
            if (GetInputValue<bool>("LoadDataPort")) LoadDataSets(FolderPath);
        }

        #endregion

    }
}
