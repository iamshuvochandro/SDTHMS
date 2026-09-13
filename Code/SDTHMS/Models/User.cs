using System;

namespace SDTHMS.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public byte[] UserImage { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public User(string username, string password, string mobile, string address, string role = "User")
        {
            Username = username;
            Password = password;
            Mobile = mobile;
            Address = address;
            Role = role;
            CreatedAtUtc = DateTime.UtcNow;
        }
        public User(int id, string username, string password, string mobile, string address, string role, byte[] userImage, DateTime createdAtUtc)
        {
            Id = id;
            Username = username;
            Password = password;
            Mobile = mobile;
            Address = address;
            Role = role;
            UserImage = userImage;
            CreatedAtUtc = createdAtUtc;
        }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Username)) return false;

            if (string.IsNullOrWhiteSpace(Password)) return false;

            if (string.IsNullOrWhiteSpace(Mobile)) return false;

            if (string.IsNullOrWhiteSpace(Address)) return false;

            if (string.IsNullOrWhiteSpace(Role)) return false;

            return true;
        }

        public bool IsAdmin()
        {
            return Role != null && Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsManager()
        {
            return Role != null && Role.Equals("Manager", StringComparison.OrdinalIgnoreCase);
        }
    }
}