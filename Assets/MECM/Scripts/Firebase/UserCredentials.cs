using System;



namespace MECM 
{
    /// <summary>
    /// Holds User credentials for firebase
    /// </summary>
    [Serializable]
    public class UserCredentials
    {
        public string email;
        public string password;

        public UserCredentials(string email, string password)
        {
            this.email = email;
            this.password = password;
        }
    }
}
