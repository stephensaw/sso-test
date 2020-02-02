using System;

namespace ClientB.Models
{
    public class UserModel
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime LastChanged { get; set; }
    }
}
