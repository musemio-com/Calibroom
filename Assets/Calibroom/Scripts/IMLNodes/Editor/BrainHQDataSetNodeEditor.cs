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
    [CustomNodeEditor(typeof(BrainHQDataSetNode))]
    public class BrainHQDataSetNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Node to draw
        /// </summary>
        BrainHQDataSetNode m_NodeDataSet;

        /// <summary>
        /// Boolean that shows or hides training data sets
        /// </summary>
        protected bool m_ShowDataSetDropdown;
        /// <summary>
        /// List of dropdowns per data entry
        /// </summary>
        protected bool[] m_DataDropdowns;
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
        protected NodePort m_ButtonPortLoadData;

        public override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            // get refs
            if (m_NodeDataSet == null)
                m_NodeDataSet = target as BrainHQDataSetNode;
            if (m_FoldoutStyle == null)
                SetDropdownStyle(out m_FoldoutStyle);
            if (m_FoldoutEmptyStyle == null)
                m_FoldoutEmptyStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("foldoutempty");
            if (m_ScrollViewStyle == null)
                m_ScrollViewStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview");
            // Get button port
            if (m_ButtonPortLoadData == null)
                m_ButtonPortLoadData = m_NodeDataSet.GetPort("LoadDataPort");
            // Create inputport button label
            if (m_ButtonPortLabel == null)
                m_ButtonPortLabel = new GUIContent("");

        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            // We modify indent level since it seems to be slightly off for this node?
            EditorGUI.indentLevel++;
            
            // Port LoadDataSets
            GUILayout.BeginHorizontal();
            // Draw port
            IMLNodeEditor.PortField(m_ButtonPortLabel, m_ButtonPortLoadData, m_NodeSkin.GetStyle("Port Label"), GUILayout.MaxWidth(10));
            // Button LoadDataSets
            string buttonText = "";
            if (m_NodeDataSet.LoadingStarted)
                buttonText = $"Loading... ({m_NodeDataSet.BrainHQDataSet.Count} entries)";
            else
                buttonText = "Load BrainHQ Files";

            if (GUILayout.Button(buttonText, m_NodeSkin.GetStyle("Run")))
            {
                m_NodeDataSet.LoadDataSets(m_NodeDataSet.FolderPath);

            }
            GUILayout.EndHorizontal();

            // Show data sets dropdown
            ShowDataSetsDropdown();

            EditorGUI.indentLevel++;


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
            if (m_NodeDataSet.BrainHQDataSet != null)
                numEntries = m_NodeDataSet.BrainHQDataSet.Count;

            m_ShowDataSetDropdown = EditorGUILayout.Foldout(m_ShowDataSetDropdown, $"View BrainHQ DataSet ({numEntries} Entries)", m_FoldoutStyle);

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

                if (m_NodeDataSet.BrainHQDataSet == null || m_NodeDataSet.BrainHQDataSet.Count == 0)
                    EditorGUILayout.LabelField("BrainHQ Data Set is empty", m_FoldoutEmptyStyle);
                else
                {
                    // Begins Vertical Scroll
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUILayout.BeginVertical();

                    m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth() - 25), GUILayout.Height(GetWidth() - 100));

                    // Make array of dropdowns match number of entries in dataset
                    if (m_DataDropdowns == null || m_DataDropdowns.Length != m_NodeDataSet.BrainHQDataSet.Count)
                        m_DataDropdowns = new bool[m_NodeDataSet.BrainHQDataSet.Count];

                    // Iterate entries 
                    for (int i = 0; i < m_NodeDataSet.BrainHQDataSet.Count; i++)
                    {
                        EditorGUILayout.LabelField($"BrainHQ User Data Set {i}", m_ScrollViewStyle);

                        var brainHQData = m_NodeDataSet.BrainHQDataSet[i];
                        
                        EditorGUI.indentLevel++;

                        string userID = brainHQData.ID.ToString();

                        m_DataDropdowns[i] = EditorGUILayout.Foldout(m_DataDropdowns[i], $"User ID: {userID}", m_FoldoutStyle);

                        // we only draw inputs if there are more than 5 examples
                        if (m_DataDropdowns[i])
                        {
                            EditorGUI.indentLevel++;

                            EditorGUILayout.LabelField("Speed/Processing Task", m_ScrollViewStyle);

                            EditorGUI.indentLevel++;
                                // Speed/Processing Task //
                                // Double Decision
                                EditorGUILayout.LabelField($"Double Decision: {brainHQData.DoubleDecisionScore} ", m_ScrollViewStyle);

                                // Mind Bender
                                EditorGUILayout.LabelField($"Mind Bender: {brainHQData.MindBenderScore} ", m_ScrollViewStyle);
                            EditorGUI.indentLevel--;

                            EditorGUILayout.LabelField("Visuo-Spatial Processing", m_ScrollViewStyle);

                            EditorGUI.indentLevel++;
                                // Visuo-Spatial Processing //
                                // Right Turn
                                EditorGUILayout.LabelField($"Right Turn: {brainHQData.RightTurnScore} ", m_ScrollViewStyle);

                                // Mental Map
                                EditorGUILayout.LabelField($"Mental Map: {brainHQData.MentalMapScore} ", m_ScrollViewStyle);

                                // Target Tracker
                                EditorGUILayout.LabelField($"Target Tracker: {brainHQData.TargetTracker} ", m_ScrollViewStyle);
                            EditorGUI.indentLevel--;
                            
                            EditorGUI.indentLevel--;


                        }

                        EditorGUI.indentLevel--;

                    }

                    // Ends Vertical Scroll
                    EditorGUI.indentLevel = indentLevel;
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;


            }

            EditorGUI.indentLevel--;
            //GUILayout.EndArea();


        }

    }

}
