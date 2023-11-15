using System;


namespace Task6_AWS.Models
{
    public class SecretCredentials
    {
        public string Host { get; set; }
        public string DbName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
