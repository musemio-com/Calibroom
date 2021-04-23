using System.IO;
using UnityEditor;
using InteractML;

namespace MECM
{
    public class MECMBuildManager
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            // Copy authentication file when the build is created
            IMLBuildManager.OnBuildCallback += CopyFirebaseCredentialsToBuild;
        }

        /// <summary>
        /// To be called from the unity editor, copies firebase credentials to build
        /// </summary>
        public static void CopyFirebaseCredentialsToBuild(BuildPlayerOptions options)
        {
            string credentialsPath = Path.Combine(IMLDataSerialization.GetAssetsPath(), "Common/AuthCredentials/FirebaseUserCredentials.json.aes");
            string credentialsDirectory = "Common/AuthCredentials";
            string credentialsName =  "FirebaseUserCredentials.json.aes";
            string buildDataPath = IMLBuildManager.GetBuildDataPath(options);

            // If the credentials file exists in current project...
            if (File.Exists(credentialsPath))
            {
                string buildCredentialsDirectory = Path.Combine(buildDataPath, credentialsDirectory);
                string buildCredentialsPath = Path.Combine(buildDataPath, credentialsDirectory, credentialsName);
                // Attempt to create a directory to copy it in the app
                if (!Directory.Exists(buildCredentialsDirectory) )
                {
                    Directory.CreateDirectory(buildCredentialsDirectory);
                }

                // Attempt to copy credentials file
                if (Directory.Exists(buildCredentialsDirectory))
                {
                    File.Copy(credentialsPath, buildCredentialsPath, true);
                }
            }
        }

    }

}
