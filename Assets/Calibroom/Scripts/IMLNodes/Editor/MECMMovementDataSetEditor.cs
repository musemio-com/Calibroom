using UnityEngine;
using InteractML;
using MECM;
using XNode;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace MECM
{
    [CustomNodeEditor(typeof(MECMMovementDataSet))]
    public class MECMMovementDataSetEditor : IMLNodeEditor
    {
        /// <summary>
        /// Node to draw
        /// </summary>
        MECMMovementDataSet m_NodeDataSet;

        /// <summary>
        /// Boolean that shows or hides training data sets
        /// </summary>
        protected bool m_ShowDataSetDropdown;
        /// <summary>
        /// List of dropdowns per data entry
        /// </summary>
        protected bool[] m_DataDropdowns;
        /// <summary>
        /// List of dropdowns per data entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_DataDropdownsLvl0;
        /// <summary>
        /// List of dropdowns per data entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_DataDropdownsLvl1;
        /// <summary>
        /// List of dropdowns per data entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_DataDropdownsLvl2;
        /// <summary>
        /// List of dropdowns per data entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_DataDropdownsLvl3;
        /// <summary>
        /// List of dropdowns per data entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_DataDropdownsLvl4;

        /// <summary>
        /// Rect for dropdown layout
        /// </summary>
        protected Rect m_Dropdown;
        /// <summary>
        /// Position of scroll for dropdown
        /// </summary>
        protected Vector2 m_ScrollPos;

        /// <summary>
        /// Style of foldout
        /// </summary>
        GUIStyle m_FoldoutStyle;
        GUIStyle m_FoldoutEmptyStyle;
        GUIStyle m_ScrollViewStyle;

        /// <summary>
        /// The label to show on the button port labels
        /// </summary>
        protected GUIContent m_ButtonPortLabel;
        /// <summary>
        /// NodePort for button. Loaded in OnHeaderHUI()
        /// </summary>
        protected NodePort m_ButtonPortProcessData;
        /// <summary>
        /// NodePort for button. Loaded in OnHeaderHUI()
        /// </summary>
        protected NodePort m_ButtonPortDataProcessed;


        public override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            // get refs
            if (m_NodeDataSet == null)
                m_NodeDataSet = target as MECMMovementDataSet;
            if (m_FoldoutStyle == null)
                SetDropdownStyle(out m_FoldoutStyle);
            if (m_FoldoutEmptyStyle == null)
                m_FoldoutEmptyStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("foldoutempty");
            if (m_ScrollViewStyle == null)
                m_ScrollViewStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview");
            // Get button port
            if (m_ButtonPortProcessData == null)
                m_ButtonPortProcessData = m_NodeDataSet.GetPort("ProcessDataPort");
            if (m_ButtonPortDataProcessed == null)
                m_ButtonPortDataProcessed = m_NodeDataSet.GetPort("DataProcessedPort");
            // Create inputport button label
            if (m_ButtonPortLabel == null)
                m_ButtonPortLabel = new GUIContent("");

        }

        public override void OnBodyGUI()
        {
            //base.OnBodyGUI();

            IMLGraph graph = this.target.graph as IMLGraph;
            if (graph.IsGraphRunning)
            {
                // If we want to reskin the node
                if (UIReskinAuto)
                {

                    // Unity specifically requires this to save/update any serial object.
                    // serializedObject.Update(); must go at the start of an inspector gui, and
                    // serializedObject.ApplyModifiedProperties(); goes at the end.
                    serializedObject.Update();

                    // Draw Port Section
                    DrawPortLayout();
                    ShowNodePorts(InputPortsNamesOverride, OutputPortsNamesOverride, showOutput);
                    // checks if node port is hovered and draws tooltip
                    PortTooltip();

                    // Draw Body Section
                    InitBodyLayout();                                     

                    // if nodespace is not set in the node editor sets it to 100 
                    if (nodeSpace == 0)
                        nodeSpace = 50;

                    // Shows body content
                    // We modify indent level since it seems to be slightly off for this node?
                    //EditorGUI.indentLevel++;
                    
                    GUILayout.Space(40);
                    // Port ProcessData
                    GUILayout.BeginHorizontal();
                    // Draw port
                    IMLNodeEditor.PortField(m_ButtonPortLabel, m_ButtonPortProcessData, m_NodeSkin.GetStyle("Port Label"), GUILayout.MaxWidth(10));
                    // Button ProcessData
                    string buttonText = "";
                    if (m_NodeDataSet.ProcessingStarted)
                        buttonText = $"Processing... {m_NodeDataSet.NumDataSetsProcessed}";
                    else
                        buttonText = "Process Data";
                    if (GUILayout.Button(buttonText, m_NodeSkin.GetStyle("Run")))
                    {
                        m_NodeDataSet.DataToWindows();
                    }
                    // Draw output port dataProcessed
                    IMLNodeEditor.PortField(m_ButtonPortLabel, m_ButtonPortDataProcessed, m_NodeSkin.GetStyle("Port Label"), GUILayout.MaxWidth(10));
                    GUILayout.EndHorizontal();

                    // Show data sets dropdown
                    ShowDataSetsDropdown();

                    //EditorGUI.indentLevel++;


                    // Draw warning box if needed
                    if (NodeDebug.Logs.ContainsKey(m_IMLNode))
                    {

                        // Get text
                        NodeDebug.Logs.TryGetValue(m_IMLNode, out warningText);
                        ShowWarning(warningText);
                        // time up for when to show a warning
                        showWarningTimeCurrent++;
                        // If we have reached the max amount of time
                        if (showWarningTimeCurrent > showWarningTime)
                        {
                            // Reset timer
                            showWarningTimeCurrent = 0f;
                            // Delete warning text from debug
                            NodeDebug.DeleteLogWarning(m_IMLNode);
                        }
                    }

                    GUILayout.Space(nodeSpace);

                    // Draw help button
                    float bottomY = HeaderRect.height + m_PortRect.height + m_BodyRect.height;
                    DrawHelpButtonLayout(bottomY);
                    ShowHelpButton(m_HelpRect);

                    serializedObject.ApplyModifiedProperties();

                    // if hovering port show port tooltip
                    if (showPort)
                    {
                        ShowTooltip(m_PortRect, TooltipText);
                    }
                    //if hovering over help show tooltip 
                    if (showHelp && nodeTips != null)
                    {
                        ShowTooltip(m_HelpRect, nodeTips.HelpTooltip);
                    }

                    // Make sure we are not recalculating rects every frame
                    m_RecalculateRects = false;
                }
                // If we want to keep xNode's default skin
                else
                {
                    base.OnBodyGUI();
                }
            }





        }

        /// <summary>
        /// Set style for dropdown for training examples nodes
        /// </summary>
        protected void SetDropdownStyle(out GUIStyle myFoldoutStyle)
        {
            GUI.skin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");
            myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
            Color myStyleColor = Color.white;
            myFoldoutStyle.fontStyle = FontStyle.Bold;
            myFoldoutStyle.normal.textColor = myStyleColor;
            myFoldoutStyle.onNormal.textColor = myStyleColor;
            myFoldoutStyle.hover.textColor = myStyleColor;
            myFoldoutStyle.onHover.textColor = myStyleColor;
            myFoldoutStyle.focused.textColor = myStyleColor;
            myFoldoutStyle.onFocused.textColor = myStyleColor;
            myFoldoutStyle.active.textColor = myStyleColor;
            myFoldoutStyle.onActive.textColor = myStyleColor;
            myFoldoutStyle.fontStyle = FontStyle.Bold;
            myFoldoutStyle.normal.textColor = myStyleColor;
            myFoldoutStyle.fontStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview").fontStyle;
        }

        private void ShowDataSetsDropdown()
        {
            int numEntries = 0;
            if (m_NodeDataSet.TrainingExamplesVector != null)
                numEntries = m_NodeDataSet.TrainingExamplesVector.Count;

            m_ShowDataSetDropdown = EditorGUILayout.Foldout(m_ShowDataSetDropdown, $"View DataSet ({numEntries} Entries)", m_FoldoutStyle);

            if (m_ShowDataSetDropdown)
            {
                //m_Dropdown = m_HelpRect;

                //m_Dropdown.height = 300;
                //if (Event.current.type == EventType.Layout)
                //{
                //    GUI.DrawTexture(m_Dropdown, NodeColor);
                //}

                //GUILayout.BeginArea(m_Dropdown);

                EditorGUI.indentLevel++;

                if (m_NodeDataSet.TrainingExamplesVector == null)
                    return;

                if (m_NodeDataSet.TrainingExamplesVector.Count < 0)
                {
                    EditorGUILayout.LabelField("Training Data Set is empty", m_FoldoutEmptyStyle);
                }
                else
                {
                    // Begins Vertical Scroll
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUILayout.BeginVertical();

                    m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth() - 25), GUILayout.Height(GetWidth() - 100));

                    // Make array of dropdowns match number of entries in dataset
                    if (m_DataDropdowns == null || m_DataDropdowns.Length != m_NodeDataSet.TrainingExamplesVector.Count)
                        m_DataDropdowns = new bool[m_NodeDataSet.TrainingExamplesVector.Count];

                    // Iterate entries 
                    for (int i = 0; i < m_NodeDataSet.TrainingExamplesVector.Count; i++)
                    {
                        EditorGUILayout.LabelField($"User Training Data Set {i}", m_ScrollViewStyle);

                        var trainingExample = m_NodeDataSet.TrainingExamplesVector[i];

                        EditorGUI.indentLevel++;

                        int numInputs = trainingExample.Inputs == null ? 0 : trainingExample.Inputs.Count;
                        string inputsName = $"Inputs ({numInputs})";

                        m_DataDropdowns[i] = EditorGUILayout.Foldout(m_DataDropdowns[i], inputsName, m_FoldoutStyle);

                        // DRAW INPUTS 
                        if (m_DataDropdowns[i])
                        {

                            EditorGUI.indentLevel++;
                            // Are there any examples in series?
                            if (trainingExample.Inputs == null)
                            {
                                EditorGUILayout.LabelField("Inputs are null ", m_ScrollViewStyle);
                                break;
                            }

                            EditorGUI.indentLevel++;
                            for (int j = 0; j < trainingExample.Inputs.Count; j++)
                            {
                                // init dropdowns
                                if (m_DataDropdownsLvl0 == null || m_DataDropdownsLvl0.Length != trainingExample.Inputs.Count)
                                    m_DataDropdownsLvl0 = new bool[trainingExample.Inputs.Count];

                                m_DataDropdownsLvl0[j] = EditorGUILayout.Foldout(m_DataDropdownsLvl0[j], $"Input Feature {j}", m_FoldoutStyle);

                                if (m_DataDropdownsLvl0[j])
                                {
                                    // Check for null
                                    if (trainingExample.Inputs[j] == null || trainingExample.Inputs[j].InputData == null || trainingExample.Inputs[j].InputData.Values == null)
                                    {
                                        EditorGUI.indentLevel++;
                                        EditorGUILayout.LabelField($"Input {j} is null", m_ScrollViewStyle);
                                        EditorGUI.indentLevel--;
                                        continue;
                                    }

                                    // Each input feature in training examples
                                    for (int k = 0; k < trainingExample.Inputs[j].InputData.Values.Length; k++)
                                    {

                                        // init dropdowns
                                        if (m_DataDropdownsLvl1 == null || m_DataDropdownsLvl1.Length != trainingExample.Inputs[j].InputData.Values.Length)
                                            m_DataDropdownsLvl1 = new bool[trainingExample.Inputs[j].InputData.Values.Length];

                                        m_DataDropdownsLvl1[k] = EditorGUILayout.Foldout(m_DataDropdownsLvl1[k], $"Input {k} ({trainingExample.Inputs[k].InputData.DataType})", m_FoldoutStyle);

                                        if (m_DataDropdownsLvl1[k])
                                        {
                                            for (int w = 0; w < trainingExample.Inputs[k].InputData.Values.Length; w++)
                                            {
                                                EditorGUI.indentLevel++;

                                                EditorGUILayout.LabelField(trainingExample.Inputs[k].InputData.Values[w].ToString(), m_ScrollViewStyle);

                                                EditorGUI.indentLevel--;
                                            }
                                        }
                                    }

                                }


                            }
                            EditorGUI.indentLevel--;

                        }

                        int numOutputs = trainingExample.Outputs == null ? 0 : trainingExample.Outputs.Count;
                        string outputsName = $"Outputs ({numOutputs})";

                        // init dropdowns
                        if (m_DataDropdownsLvl2 == null || m_DataDropdownsLvl2.Length != m_NodeDataSet.TrainingExamplesVector.Count)
                            m_DataDropdownsLvl2 = new bool[m_NodeDataSet.TrainingExamplesVector.Count];

                        m_DataDropdownsLvl2[i] = EditorGUILayout.Foldout(m_DataDropdownsLvl2[i], outputsName, m_FoldoutStyle);

                        // DRAW OUTPUTS
                        if (m_DataDropdownsLvl2[i])
                        {

                            EditorGUI.indentLevel++;
                            // Are there any examples in series?
                            if (trainingExample.Outputs == null)
                            {
                                EditorGUILayout.LabelField("Outputs are null ", m_ScrollViewStyle);
                                break;
                            }
                            
                            for (int j = 0; j < trainingExample.Outputs.Count; j++)
                            {
                                // init dropdowns
                                if (m_DataDropdownsLvl3 == null || m_DataDropdownsLvl3.Length != trainingExample.Outputs.Count)
                                    m_DataDropdownsLvl3 = new bool[trainingExample.Outputs.Count];

                                m_DataDropdownsLvl3[j] = EditorGUILayout.Foldout(m_DataDropdownsLvl3[j], $"Output Feature {j}", m_FoldoutStyle);

                                if (m_DataDropdownsLvl3[j])
                                {
                                    EditorGUI.indentLevel++;

                                    if (trainingExample.Outputs[j].OutputData == null || trainingExample.Outputs[j].OutputData.Values == null)
                                    {
                                        EditorGUILayout.LabelField($"Output {j} is null", m_ScrollViewStyle);
                                    }
                                    else
                                    {
                                        // Each input feature in training examples
                                        for (int k = 0; k < trainingExample.Outputs[j].OutputData.Values.Length; k++)
                                        {

                                            // init dropdowns
                                            if (m_DataDropdownsLvl4 == null || m_DataDropdownsLvl4.Length != trainingExample.Outputs[j].OutputData.Values.Length)
                                                m_DataDropdownsLvl4 = new bool[trainingExample.Outputs[j].OutputData.Values.Length];

                                            m_DataDropdownsLvl4[k] = EditorGUILayout.Foldout(m_DataDropdownsLvl4[k], $"Output {k} ({trainingExample.Outputs[k].OutputData.DataType})", m_FoldoutStyle);

                                            if (m_DataDropdownsLvl4[k])
                                            {
                                                for (int w = 0; w < trainingExample.Outputs[k].OutputData.Values.Length; w++)
                                                {
                                                    EditorGUI.indentLevel++;

                                                    EditorGUILayout.LabelField(trainingExample.Outputs[k].OutputData.Values[w].ToString(), m_ScrollViewStyle);

                                                    EditorGUI.indentLevel--;
                                                }

                                            }
                                        }
                                    }

                                    EditorGUI.indentLevel--;

                                }

                            }
                            EditorGUI.indentLevel--;

                        }


                        EditorGUI.indentLevel--;

                    }

                    // Ends Vertical Scroll
                    EditorGUI.indentLevel = indentLevel;
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                }



            }

            //GUILayout.EndArea();


        }
    }

}
