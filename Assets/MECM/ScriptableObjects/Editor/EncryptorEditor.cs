//using UnityEngine;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//namespace MECM
//{
//    [CustomEditor(typeof(Encryptor))]
//    public class EncryptorEditor : Editor
//    {
//#if UNITY_EDITOR
//        public override void OnInspectorGUI()
//        {
//            // Draws default behaviour
//            base.OnInspectorGUI();

//            // Draws button to encrypt a file
//            if (GUILayout.Button("Encrypt File"))
//            {
//                var encryptor = target as Encryptor;
//                encryptor.EncryptFile(encryptor.FilePath);
//            }
//        }

//#endif
//    }

//}
