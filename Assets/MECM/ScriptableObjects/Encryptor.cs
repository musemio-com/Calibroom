//using UnityEngine;
//using System;

//namespace MECM
//{
//    /// <summary>
//    /// Scriptable object that can encrypt a file 
//    /// </summary>
//    [CreateAssetMenu(fileName = "Encryptor", menuName
//    = "MECM/Create Encryptor File")]
//    public class Encryptor : ScriptableObject
//    {
//        /// <summary>
//        /// Path to file we want to encrypt
//        /// </summary>
//        public string FilePath;

//        /// <summary>
//        /// Password needed to encrypt/decrypt files
//        /// </summary>
//        public string CryptoPassword;

//        /// <summary>
//        /// Encrypts a file on the specified path into "filePath.aes"
//        /// </summary>
//        /// <param name="filePath"></param>
//        public void EncryptFile(string filePath)
//        {
//            EncryptFilePrivate(filePath, CryptoPassword);
//        }

//        private void EncryptFilePrivate(string filePath, string password)
//        {
//            if (String.IsNullOrEmpty(filePath))
//            {
//                Debug.LogError("No file is specified for encryption");
//                return;
//            }
//            if (String.IsNullOrEmpty(password))
//            {
//                Debug.LogError("You need to specify a password for encryption!");
//                return;
//            }
//            // If we have both a password and a filepath, proceed to encrypt
//            AESEncryptor.FileEncryptAsync(filePath, password);
//        }

//    }

//}
