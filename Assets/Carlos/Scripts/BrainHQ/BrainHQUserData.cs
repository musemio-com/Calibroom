namespace MECM
{
    /// <summary>
    /// Holds information regarding a BrainHQ user
    /// </summary>
    public struct BrainHQUserData
    {
        /// <summary>
        /// User ID 
        /// </summary>
        public int ID;

        // Speed/Processing Task // 

        public float DoubleDecisionScore;

        public float MindBenderScore;

        // Visuo-Spatial Processing //

        public float RightTurnScore;

        public int MentalMapScore;

        public float TargetTracker;

        public BrainHQUserData(int iD, float doubleDecisionScore, float mindBenderScore, float rightTurnScore, int mentalMapScore, float targetTracker)
        {
            ID = iD;
            DoubleDecisionScore = doubleDecisionScore;
            MindBenderScore = mindBenderScore;
            RightTurnScore = rightTurnScore;
            MentalMapScore = mentalMapScore;
            TargetTracker = targetTracker;
        }
    }

}
