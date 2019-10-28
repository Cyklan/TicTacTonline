using System;
using System.Security.Cryptography;

namespace Models
{
    public class User
    {
        private const int SALTROUNDS = 5000;

        public string Name { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string PasswordHash { get; set; }
        public string IpPort
        {
            get { return $"{IpAddress}:{Port}"; }
            set 
            { 
                IpAddress = value.Split(':')[0];
                Port = Convert.ToInt32(value.Split(':')[1]);
            }
        }

        public User()
        {

        }

        public User(string ipPort)
        : this("", ipPort.Split(':')[0], Convert.ToInt32(ipPort.Split(':')[1]), "") { }

        public User(string name, string ipPort, string password)
        : this(name, ipPort.Split(':')[0], Convert.ToInt32(ipPort.Split(':')[1]), password) { }

        public User(string name, string ip, int port, string password)
        {
            Name = name;
            IpAddress = ip;
            Port = port;
            PasswordHash = HashPassword(password);
        }

        private string HashPassword(string password)
        {
            byte[] salt;
            byte[] hash;
            byte[] hashBytes = new byte[36];

            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(salt = new byte[16]);
            }
            using (var rfc = new Rfc2898DeriveBytes(password, salt, SALTROUNDS))
            {
                hash = rfc.GetBytes(20);
            }
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        public override string ToString()
        {
            string message = $"{Name}";
            if (!string.IsNullOrEmpty(IpAddress)) message += $" :[{IpAddress}:{Port}]";
            return $"{message}";
        }
    }
}
