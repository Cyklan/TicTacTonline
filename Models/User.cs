using System;
using System.Security.Cryptography;
using System.Text;

namespace Models
{
    public class User
    {

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
            using SHA256 sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        public override string ToString()
        {
            string message = $"{Name}";
            if (!string.IsNullOrEmpty(IpAddress)) message += $" :[{IpAddress}:{Port}]";
            return $"{message}";
        }
    }
}
