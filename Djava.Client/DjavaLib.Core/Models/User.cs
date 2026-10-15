using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DjavaLib.Models
{
    public class User
    {
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string ContactInfo { get; set; }
        public UserRole Role { get; set; }
    }
}
