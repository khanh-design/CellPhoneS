using System;
using System.Collections.Generic;

namespace CPhoneS.Models
{
    public partial class ApplicationUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
    }
}
