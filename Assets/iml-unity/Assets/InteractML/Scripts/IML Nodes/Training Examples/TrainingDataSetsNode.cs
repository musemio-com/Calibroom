using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
using XNode;
using System.Threading.Tasks;


namespace InteractML
{
    /// <summary>
    /// Holds multiple training data sets
    /// </summary>
    [CreateNodeMenuAttribute("Interact ML/DataSets/TrainingDataSets")]
    [NodeWidth(350)]

    public class TrainingDataSetsNode : IMLNode, IFeatureIML, IUpdatableIML
    {
        #region Variables
        /// <summary>
        /// List of Training dataset holding all the training examples
        /// </summary>
        [Output]
        public List<List<IMLTrainingExample>> TrainingDataSets;

        /// <summary>
        /// Path of folder to search in
        /// </summary>
        public string FolderPath;

        /// <summary>
        /// When loading, are we looking only for a specific nodeID in file(s)?
        /// </summary>
        public string SpecificNodeID;

        /// <summary>
        /// Number of training examples lists loaded
        /// </summary>
        public int DataSetSize { get { return m_DataSetSize; } }
        [SerializeField]
        private int m_DataSetSize;

        // Flags for loading
        [System.NonSerialized]
        private bool m_LoadingStarted;
        public bool LoadingStarted { get { return m_LoadingStarted; } }
        [System.NonSerialized]
        private bool m_LoadingFinished;
        public bool LoadingFinished { get { return m_LoadingFinished; } }

        public IMLBaseDataType FeatureValues => default(IMLBaseDataType);

        public bool isExternallyUpdatable => true;

        public bool isUpdated { get => m_isUpdated; set => m_isUpdated = value; }
        private bool m_isUpdated;


        #endregion

        #region xNode Messages
        // Use this for initialization
        protected override void Init()
        {
            base.Init();

            if (TrainingDataSets == null || TrainingDataSets.Count == 0)
                m_DataSetSize = 0;

            // Add all required dynamic ports
            // LoadData          
            this.GetOrCreateDynamicPort("LoadDataPort", typeof(bool), NodePort.IO.Input);
            // DataLoadedPort        
            this.GetOrCreateDynamicPort("DataLoadedPort", typeof(bool), NodePort.IO.Output);

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            // Returns the list of training examples 
            if (port.Equals(GetOutputPort("TrainingDataSets"))) return TrainingDataSets;
            // Data loading finished
            else if (port.Equals(GetOutputPort("DataLoadedPort"))) 
            {
                //Debug.Log($"DataLoadedPort: {LoadingFinished}");
                return LoadingFinished; 
            }
            else return null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads several dataset files from a folder
        /// </summary>
        /// <param name="path">path of file to load</param>
        /// <param name="specificID">When loading, are we looking only for a specific nodeID in file(s)?</param>        
        public void LoadDataSets(string path, string specificID = "")
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
                if (TrainingDataSets == null)
                    TrainingDataSets = new List<List<IMLTrainingExample>>();
                else
                    TrainingDataSets.Clear();

                m_DataSetSize = 0;

                Task.Run(async () => 
                {
                    // First, find all the folders
                    // Iterate to upload all files in folder, including subdirectories
                    string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    //Debug.Log($"{files.Length + 1} files found. Loading data sets, please wait...");
                    foreach (string file in files)
                    {
                        // If there is a json file, attempt to load
                        if (Path.GetExtension(file) == ".json")
                        {
                            // Are we looking for a specific ID?
                            if (!string.IsNullOrEmpty(specificID))
                            {
                                // skip if the file doesn't contain the ID we want
                                if (!file.ToLower().Contains(specificID))// te
                                    continue;
                                //else
                                //    Debug.Log($"Starting to load file {file}...");
                            }

                            // Load training data set
                            var singleDataSet = await IMLDataSerialization.LoadTrainingSetFromDiskAsync(file, ignoreDefaultLocation: true);

                            // Add to list if not null
                            if (singleDataSet != null && singleDataSet[0].Inputs != null)
                            {
                                TrainingDataSets.Add(singleDataSet);
                                m_DataSetSize++;
                            }
                        }
                    }

                    if (TrainingDataSets.Count == 0)
                    {
                        m_LoadingFinished = false;
                        m_LoadingStarted = false; // allow to re-load if user wants to
                        NodeDebug.LogWarning("Couldn't load folder!", this, debugToConsole: true);
                    }
                    else
                    {
                        m_DataSetSize = TrainingDataSets.Count;
                        m_LoadingFinished = true;
                        //Debug.Log($"Setting loading finished to: {LoadingFinished}");
                        m_LoadingStarted = false; // allow to re-load if user wants to
                        //Debug.Log($"{TrainingDataSets.Count + 1} Data Sets Loaded!");
                    }


                });

            }
            else
            {
                NodeDebug.LogWarning("The folder doesn't exist!", this);
            }
        }

        public object UpdateFeature()
        {
            // Pull inputs from bool event nodeports
            if (GetInputValue<bool>("LoadDataPort")) LoadDataSets(FolderPath, SpecificNodeID);            
            return this;
        }

        public void Update()
        {
            // Do nothing
        }

        public void LateUpdate()
        {
            // Allow flag data loaded only for a frame emulating an event
            if (m_LoadingFinished) {  m_LoadingFinished = false; }
        }

        #endregion
    }

}
